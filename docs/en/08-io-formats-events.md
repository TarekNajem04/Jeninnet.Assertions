# Chapter 8 — The Outside World: Streams, XML, JSON, HTTP, Events, and I/O

Everything outside process memory is a boundary with potential failure modes: networks drop, files get locked, streams close unexpectedly, and events may or may not fire at the right time. Testing these boundaries directly often means testing the failure condition itself rather than your code's logic.

The guiding principle is simple: **push boundaries inward**. Fake the transport, isolate the filesystem, assert **capabilities before content**, and verify both what **should happen** and what **must not happen**.

## 8.1 Streams: Capabilities before content

Before reading from or writing to a stream, assert its capabilities:

```csharp
stream.Should().BeReadable().And.BeWritable().And.NotBeNull();
stream.Should().BeSeekable();
stream.Should().HaveLength(1024);
buffered.Should().HaveBufferSize(8192);
```

Reading from a non-readable stream throws—an explicit assertion gives you a clear failure message at the right point, making diagnosis far easier.

Negatives, access modes, and positions round out the surface:

```csharp
stream.Should().NotBeReadable().And.NotBeWritable().And.NotBeSeekable();
readOnly.Should().BeReadOnly();
writeOnly.Should().BeWriteOnly();
stream.Should().NotHaveLength(0).And.HavePosition(2).And.NotHavePosition(0);
buffered.Should().NotHaveBufferSize(1024);
```

`HavePosition()` reads the seekable position (positions start at zero). Negative assertions pass for `null` or unreadable streams: absence cannot be disproven without a readable value.

For `BufferedStream`, you can also assert buffer size when that's part of the behavior you're testing.

## 8.2 XML and JSON: Navigate, then check

### XML

Both `System.Xml.XmlNode` and LINQ-to-XML (`XDocument`, `XElement`, `XAttribute`) are supported with a consistent vocabulary:

```csharp
doc.Should().HaveRoot("invoice").Which
    .Should().HaveElement("total").Which
    .Should().HaveValue("42.00");

element.Should().HaveAttribute("currency").Which
    .Should().HaveValue("USD");

element.Should().HaveAttributeWithValue("currency", "USD");
```

`HaveElement()` and `HaveAttribute()` return `.Which`, letting you navigate and assert in one fluent chain instead of breaking it into separate lookups. Qualified `XName` overloads match namespaces exactly:

```csharp
XNamespace ns = "urn:example";
element.Should().HaveElement(ns + "total", Exactly.Once());
element.Should().HaveElementWithValue("total", "42.00");
element.Should().HaveElementWithNamespace("total", "urn:example");
element.Should().HaveAttributeWithNamespace("currency", "urn:example");
element.Should().NotHaveElement("legacy");
element.Should().NotHaveAttribute("legacy");
element.Should().HaveInnerText("42.00");
element.Should().NotHaveInnerText("0.00");
```

For full documents, compare structurally rather than as raw text:

```csharp
doc.Should().BeEquivalentTo(expectedDoc);
```

This ignores irrelevant differences like attribute ordering or whitespace formatting.

### JSON

For `System.Text.Json.JsonNode` (and similar types), the same pattern applies:

```csharp
node.Should().HaveProperty("total").Which
    .Should().HaveValue<decimal>(42.00m);

node.Should().HaveCount(3);
node.Should().BeEquivalentTo(expectedNode);
node.Should().NotBeEquivalentTo(otherNode);
node.Should().BeString().Which.Should().Be("total");
node.Should().BeNumeric().Which.Should().Be("42.00");
node.Should().BeBool().Which.Should().BeTrue();
node.Should().BeAnArray().Which.Should().HaveCount(3);
node.Should().BeUtcDate();
node.Should().BeLocalDate();
```

`JsonArray.Should()` treats an array as a collection, while `Should<T>()` preserves the closed JSON node type for generic assertions.

For very large outputs, snapshots (Chapter 6) are often cleaner than asserting individual nodes.

### JSON paths

Reaching into nested documents with a mini path beats chaining `HaveProperty` calls: `$` is the root, `.name` is an object member (case-sensitive), and `[index]` is an array element — the resolved node comes back through `Which`:

```csharp
node.Should().HaveJsonPath("$.users[0].name").Which.Should().BeString().Which.Should().Be("ada");
node.Should().HaveJsonPath("$.user.middle").Which.Should().BeNull();   // a JSON null still counts as present
node.Should().NotHaveJsonPath("$.user.deleted");
```

A typical API test parses the payload once (`JsonNode.Parse` after `BeJsonSerializable`), then pins the fields that matter with paths. When a segment is missing, the failure names the breaking segment and lists the sibling properties that do exist (`(available: …)`, capped at ten), plus a nearest-name hint when the miss looks like a typo (`Did you mean 'name'?`); malformed paths (no `$`, empty segments, non-numeric indexes) throw `ArgumentException`. Wildcards, filters, quoted names, and recursive descent are deliberately out of scope.

### Round-trip serialization

To verify a type survives a serialize/deserialize cycle, assert the round trip directly. The value is serialized with its **runtime type**, deserialized back, and compared with the equivalency engine—any loss (dropped members, fidelity changes, or a throwing getter during comparison) fails with a clear message:

```csharp
order.Should().BeXmlSerializable();
order.Should().BeJsonSerializable();
order.Should().BeDataContractSerializable();
```

## 8.3 HTTP: Fake the transport, don't test the network

HTTP response assertions are available for `net6.0+`:

```csharp
response.Should().HaveStatusCode(HttpStatusCode.OK);
response.Should().BeSuccessful(); // any 2xx status
response.Should().HaveHeader("X-Request-Id");
response.Should().HaveHeader("Content-Type", "application/json");
```

**Hard rule:** always test against a fake `HttpMessageHandler`, never against a real network. The network is non-deterministic, slow, and environment-dependent—making tests flaky and hard to reproduce.

If `response` is `null`, every assertion fails with a clear null-specific message instead of throwing `NullReferenceException`. That keeps diagnostics clean.

## 8.4 Events: Assert what fires and what must stay silent

To monitor `event` occurrences, use `Monitor()` which attaches to public instance events and records them:

```csharp
using var monitor = publisher.Monitor();
publisher.Ship(order);

monitor.Should().Raise("OrderShipped")
    .Which.Sender.Should().BeSameAs(publisher);
```

`.Which` gives you access to the `OccurredEvent` (`EventName`, `Sender`, `Args`, `Timestamp`), so you can assert precisely—especially the sender. The right event name from the wrong sender is a real bug. Occurrence-level assertions chain directly:

```csharp
monitor.Should().Raise("OrderShipped")
    .WithSender(publisher)
    .And.WithArgs<OrderShippedArgs>(static args => args.OrderId == 42);
```

`WithArgs()` also takes one predicate per argument (`WithArgs(p1, p2)`); the predicate count must match the argument count. For `INotifyPropertyChanged` sources there is a shortcut (the property-name match is literal and case-sensitive):

```csharp
monitor.Should().RaisePropertyChangeFor("Total");
monitor.Should().NotRaisePropertyChangeFor("Discount");
```

The monitor itself is inspectable through the public `IMonitor<OccurredEvent>` contract (`Subject`, `MonitoredEvents`, `GetRecordingFor(name)`, `Clear()`), and `Monitor<T>()` names the source statically. Sources with throwing subscription accessors fail fast by default; opt into `EventMonitorOptions.IgnoringEventAccessorExceptions()` to skip those events instead:

```csharp
using var monitor = publisher.Monitor(new EventMonitorOptions().IgnoringEventAccessorExceptions());
```

Equally important is asserting **what must not fire**:

```csharp
monitor.Should().NotRaise("OrderCancelled");
```

Silent bugs often hide here: unwanted events firing unnoticed, or legacy events still being raised. Asserting silence is just as important as asserting the expected signal. The monitor is disposable (`using` takes care of detaching).

## 8.5 I/O: Isolated test environment

To work with the filesystem without touching your working tree, use `TestEnvironment`. It's disposable and creates a temporary, isolated directory per test:

```csharp
using var env = new TestEnvironment();
var dir = env.CreateDirectory("inbox");
env.CreateFile("inbox/order.json", "{\"id\":1}");
env.CreateTree(new Dictionary<string, string?> {
    ["a/b.txt"] = "x"
});
env.CreateFiles("one.txt", "two.txt");
```

Everything lives under `env.Root` and is automatically cleaned up when disposed.

### Simulating failure conditions

Sometimes you need to test paths where files are locked or directories are inaccessible. The environment helpers simulate these cases without requiring admin privileges:

```csharp
using var locked = env.CreateLockedFile("busy", "data.txt");
env.SetInaccessible("busy/data.txt");
env.CreateInaccessibleDirectory("vault");
env.AssertDirectoryInaccessible("vault");
```

`InaccessibleDirectorySimulator.CreateLockedDir` and `CreatePseudoInaccessibleDir` bridge platform differences (Windows ACLs vs Unix permissions), letting you test error paths consistently across development environments.

## 8.6 Recap and exercise

**Recap:**

- **Push boundaries inward.** Fake transports, isolate the filesystem. Never test real networks or live filesystem state in unit tests.
- **Capabilities before content.** Assert `BeReadable()`, `BeWritable()`, `BeSeekable()` before reading/writing.
- **Navigate, then assert.** Use `.Which` with `HaveElement()`, `HaveAttribute()`, and `HaveProperty()`.
- **Assert both presence and absence.** Check `Raise()` for expected events and `NotRaise()` for events that must never fire.
- **Isolate per test.** Use `TestEnvironment` and clean up with `using`.

**Exercise:**

1. Create a `TestEnvironment`, write a file, and assert it's readable.
2. Parse a simple XML document using `HaveRoot()`, `HaveElement()`, and `HaveValue()`.
3. Monitor an event with `Monitor()`, assert it fired from the correct sender, and assert a different event **did not** fire.
4. Try creating a locked/inaccessible file path using `CreateLockedFile()` or `SetInaccessible()` and assert the expected failure behavior.

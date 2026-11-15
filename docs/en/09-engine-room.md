# Chapter 9 — The Engine Room: Scopes, Culture, Formatting, and Configuration

Assertions themselves are only half the library. The other half is what happens around them: does it stop at the first failure or collect them all? Does a culture change in one parallel test leak to another? How does your custom type appear in a failure message? Where do default behaviors live?

This chapter covers the mechanisms that keep hundreds of assertions behaving as a coherent system.

## 9.1 AssertionScope: One report instead of the first throw

Outside an `AssertionScope`, the first failure throws immediately. Inside a scope, all failures are collected and thrown together when the scope is disposed:

```csharp
using (new AssertionScope()) {
    total.Should().BePositive();
    lines.Should().NotBeEmpty();
    customer.Should().NotBeNull();
}
```

If two assertions fail inside the scope, you'll see both in a single, comprehensive failure message. This saves many debugging iterations by showing the full picture at once.

**Important rule:** only collect failures for checks that are **logically independent**. Dependent checks—where step two only makes sense if step one passed—should remain sequential outside a scope. Collecting failures after a broken precondition creates noise rather than clarity.

### Nesting, naming, and discarding

Scopes can be nested: failures from inner scopes roll up into the outer scope's report. You can name a scope to label sections in the output:

```csharp
using (new AssertionScope("Pricing")) {
    // ...
}
```

### Discarding without throwing

Sometimes you want to inspect collected failures without failing the test. Use `Discard()`:

```csharp
var failures = scope.Discard();
failures.Should().Contain(f => f.Contains("total"));
```

`Discard()` returns the array of collected failure messages. This is useful for testing the assertion aggregation itself or for custom reporting.

External frameworks and tooling can observe the ambient scope without owning it: `AssertionScope.Current` returns the active scope (or `null` outside any scope), `scope.Name` returns the construction name, and `scope.HasFailures()` reports pending failures. These are read-only diagnostics; collecting still happens only through normal assertions.

## 9.2 Culture: Isolated per async flow

The library supports localized failure messages. By default, messages are in English. You can switch culture per async flow (`AsyncLocal`), so parallel tests don't interfere with each other:

```csharp
AssertionCulture.Current = new CultureInfo("ar");
flag.Should().BeTrue(); // Message appears in Arabic if translated
```

### Key guarantees

1. **Values stay invariant:** Numbers, dates, sizes, and formatted values always use `CultureInfo.InvariantCulture`. Only the prose is translated. This keeps failure messages **text-comparable** in tests.
2. **Unknown cultures fall back to English:** Safe and non-breaking if a culture isn't fully translated.
3. **`because` and scope headers are localized too:** Full integration with the translation system.

### Add a language in 10 minutes

Localization is a resource-file activity, never a code change:

1. Copy the neutral file and rename it by culture:

```text
src/Jeninnet.Assertions/Localization/Strings.resx
→ src/Jeninnet.Assertions/Localization/Strings.<culture>.resx
```

2. Translate the `<value>` entries. Shipped languages must be key-complete; `check-localization.ps1` enforces missing/extra/empty keys (the French reference satellite is explicitly allowlisted as partial). Keep the `{0}`, `{1}`, … placeholders and leave formatted values (`true`, numbers, dates) untouched, so failure messages stay byte-comparable.
3. Build. The SDK emits the satellite assembly (`Strings.<culture>.resources.dll`) with no further wiring.

Read translated templates for custom reporting through the public reader:

```csharp
AssertionCulture.Current = new CultureInfo("ar");
var template = AssertionCulture.GetMessage("BOOLEAN_BE_TRUE");       // localized template
var message = AssertionCulture.FormatMessage("BOOLEAN_BE_TRUE", "flag"); // invariant formatting
```

English is the neutral resource; Arabic (`ar`), Spanish (`es`), and German (`de`) ship complete. French (`fr`) is a partial reference satellite that demonstrates per-key fallback for the entries it does not translate. `qps-ploc` is a committed pseudo-localization used to detect hardcoded text. The completion gate is `pwsh scripts/docs/check-localization.ps1`; it fails on missing/extra/empty keys (the French reference satellite is explicitly allowlisted as partial), and [CONTRIBUTING.md](https://github.com/TarekNajem04/Jeninnet.Assertions/blob/main/CONTRIBUTING.md#translation-guide) carries the translator guide.

## 9.3 Formatters: Teach the library how to display your types

If a failure prints `Store.Money` instead of `USD 42.00`, the issue is **display**, not the assertion itself. The solution is a **value formatter** (`IValueFormatter`), not duplicating formatting in every test:

```csharp
public sealed class MoneyFormatter : IValueFormatter {
    public bool CanFormat(object? value) => value is Money;
    public string Format(object? value) => ((Money)value!).ToDisplayString();
}
```

### Registering formatters

Per scope:

```csharp
using (var scope = new AssertionScope()) {
    scope.Formatting.AddFormatter(new MoneyFormatter());
    // ...
}
```

Globally:

```csharp
AssertionConfiguration.Formatting.AddFormatter(new MoneyFormatter());
AssertionConfiguration.Formatting.RemoveFormatter<MoneyFormatter>();
AssertionConfiguration.Formatting.Clear();
```

**Critical rule:** formatters only affect **display**, never **comparison**. Assertions always compare actual values. Domain types deserve their own formatters rather than relying on a potentially unhelpful `ToString()` override.

### Formatting limits

`ValueFormatterOptions` also bounds how much text a failure may carry, so a deep graph or a huge string cannot drown the message. Limits apply after formatters run and are configurable per scope or process-wide:

```csharp
using (var scope = new AssertionScope()) {
    scope.Formatting.MaxDepth = 10;           // structural nesting depth
    scope.Formatting.MaxLines = 100;          // rendered lines
    scope.Formatting.MaxEnumerables = 100;    // items per enumerable
    scope.Formatting.MaxStringLength = 8_000; // characters
    scope.Formatting.UseLineBreaks = true;    // false collapses to one line
    scope.Formatting.RespectRuntimeTypes = true; // opt-in structural display
}
```

Defaults preserve the historical output: `MaxDepth` 10, `MaxLines` 100, `MaxEnumerables` 100, `MaxStringLength` 8,000, `UseLineBreaks` on, and `RespectRuntimeTypes` off. Truncation is never silent — the text ends with `…` and the hidden line/character count. Line endings are preserved by default; only `UseLineBreaks = false` collapses whitespace.

`RespectRuntimeTypes` is the switch that changes collection display: with it on, an array renders as `[1, 2, 3]` (recursively, up to `MaxDepth` and `MaxEnumerables`, with `… (truncated)`, `… (max depth)`, and `… (cycle)` markers). With it off, the engine keeps calling `ToString()`, exactly as before. `Copy()` and `RestoreFrom()` carry formatters **and** limits, so `AssertionIsolation.Isolate()` restores the whole formatting configuration.

Deliberate non-goal: this `IValueFormatter` contract (`CanFormat` + `Format`) is intentionally **not** compatible with the same-named interface in other fluent assertion libraries. Aligning them would break every existing formatter without adding capability.

## 9.4 Configuration and engine initializers

`AssertionConfiguration` exposes two main global settings:

- `Formatting` — value formatters (as above)
- `Equivalency` — defaults for `BeEquivalentTo()`: `StrictOrderingDefault`, a shared custom-step `Plan`, and an ambient template mutated with `Modify(o => { o.StrictOrdering = true; })`, snapshotted with `CloneDefaults()`, and cleared with `EquivalencyDefaults.ResetToDefaults()`

`Modify` overrides are isolated per async flow (`AsyncLocal`), exactly like scopes and culture: parallel tests never see each other's overrides. Per-assertion configuration always wins over the ambient template, which wins over the process-wide defaults. Failure routing is pluggable through the public `IAssertionStrategy` (`AssertionScope(IAssertionStrategy)`), with a default collecting strategy aggregating failures until a scope disposes. Deliberate non-goals, recorded here: no public `AssertionChain` (a `GivenSelector` chain covers conditional multi-step checks; otherwise extension goes through `IEquivalencyStep`, rules, `AssertionScope`, and formatters instead of a second assertion pipeline), and no auto-fix option anywhere (rewriting the subject inside an assertion would mutate user data).

### Isolating ambient state

Settings changed by a test should not leak into the next one. One line isolates the ambient knobs — culture, clock, formatting (formatters and limits), the active scope's expectation, and the ambient equivalency template — and restores them on dispose, even when the scope exits through an exception:

```csharp
using(AssertionIsolation.Isolate()) {
    AssertionCulture.Current = new CultureInfo("ar");
    AssertionClock.Current = new ManualClock(epoch);
    AssertionConfiguration.Formatting.AddFormatter(new MoneyFormatter());
    AssertionConfiguration.Formatting.MaxLines = 20;
    AssertionScope.Current?.WithExpectation("Expected the invoice");
    AssertionConfiguration.Equivalency.Modify(o => o.StrictOrdering = true);

    // ... a test that expects all of the above ...
}
// culture, clock, formatting, the scope expectation, and equivalency are back to their pre-scope values.
```

Nested scopes each restore their own snapshot, and the culture/clock/expectation/equivalency parts are per-async-flow (`AsyncLocal`), so parallel tests stay isolated. Two boundaries are deliberate: the formatter registry is process-wide (the scope prevents sequential leakage, but parallel tests that both mutate it still need coordination), and state outside this library — like the thread's `CultureInfo.CurrentCulture` — is not touched.

### Assembly-level initializers

For one-time setup, use `AssertionEngineInitializer` at the assembly level instead of registering in every test fixture:

```csharp
[assembly: AssertionEngineInitializer(typeof(TestSetup), nameof(TestSetup.Initialize))]

public static class TestSetup {
    public static void Initialize() {
        AssertionConfiguration.Formatting.AddFormatter(new MoneyFormatter());
        // other one-time setup
    }
}
```

Initializers run **once per engine load**, before any tests run. Keep them fast and side-effect free.

### For custom assertion authors

Custom assertion helpers can be marked with `[CustomAssertion]`, and custom assertion assemblies with `[CustomAssertionsAssembly]`, to clearly define assertion boundaries. Hosts and tooling consume these markers through `CustomAssertionDiscovery`: `FindMarkedAssemblies()` locates extension-only assemblies, `FindCustomAssertions(assembly)` validates their marked members (they must be public static), and `RunInitializers(assembly)` executes their engine setups — including assemblies loaded after the first assertion, which the automatic scan would otherwise miss. Day-to-day extension still goes through `AssertionScope` alone; the discovery API exists for composing those extensions.

`AssertionsBase` is the supported SDK base for a custom assertion class. Derive from it, use `Expression` for the captured subject, and call `Fail` with the fully formatted message plus the optional `because` clause; `Fail` appends the reason and routes the failure to the active scope (structured failures keep the expression, actual, and expected values for `SoftAssertions`):

```csharp
public sealed class OrderAssertions(Order order, string? expression = null)
    : AssertionsBase(expression, "order") {
    public AndConstraint<OrderAssertions> BePaid(string? because = null, params object?[] becauseArgs) {
        if(order.Status != "Paid") {
            Fail($"Expected {Expression} to be paid, but found {FormatValue(order.Status)}.", because, becauseArgs);
        }

        return new(this);
    }
}
```

`FormatValue` formats with the invariant culture and the active formatting limits, so custom messages stay byte-stable. `AssertionScope.Current` observes the ambient collector when a helper needs to inspect or add pre-formatted failures. To rename the subject for a block, set an expectation on the scope; nested scopes inherit it without mutating the parent, and `ClearExpectation()` restores the default:

```csharp
using (new AssertionScope().WithExpectation("Expected order #42")) {
    order.Should().BePaid(); // Expected order #42 to be paid, but found Pending.
}
```

## 9.5 Deterministic clocks: test time without sleeping

Time-relative verbs (`BeWithin`, `BeLessThan`, `BeMoreThan`, `BeAtLeast`) read the current moment from a single, injectable source: `AssertionClock.Current`, an `IClock` whose default is `SystemClock` (the machine clock). Injecting a fixed clock makes boundary tests deterministic — no `Thread.Sleep`, no wall-clock races:

```csharp
AssertionClock.Current = new ManualClock(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
try {
    var value = new DateTime(2026, 1, 1, 0, 5, 0, DateTimeKind.Utc);
    value.Should().BeWithin(TimeSpan.FromMinutes(5));      // passes, exactly at the edge
    value.Should().BeWithin(TimeSpan.FromMinutes(3));      // fails deterministically
}
finally {
    AssertionClock.Current = SystemClock.Instance;
}
```

`ManualClock` ships with the library: start it at a fixed instant and call `Advance`/`Set` exactly where production code would experience time passing — no `Thread.Sleep`. On .NET 8+, `TimeProviderClock` adapts any `TimeProvider` (including fake providers) to the same injection point, so existing fakes plug straight in; always reset in cleanup, as above.

`IClock` exposes `Now` (local) and `UtcNow`; the `DateTime` verbs pick `UtcNow` for `DateTimeKind.Utc` values and `Now` otherwise, while the `DateTimeOffset` verbs use `Now`. Like scopes and culture, `AssertionClock.Current` is isolated per async flow (`AsyncLocal`), so parallel tests never see each other's clock. `SystemClock.Instance` is used when nothing is injected.

## 9.6 Trimming and NativeAOT

The reflection-free core is safe under trimming and NativeAOT. The repository ships `samples/Jeninnet.Assertions.Aot`, a console smoke test that exercises `Be`, `BeTrue`, `Contain`, `HaveCount`, and `Throw`, publishes both trimmed and NativeAOT, and runs successfully:

```powershell
dotnet publish samples/Jeninnet.Assertions.Aot/Jeninnet.Assertions.Aot.csproj -c Release -p:TrimSample=true
dotnet publish samples/Jeninnet.Assertions.Aot/Jeninnet.Assertions.Aot.csproj -c Release -p:AotSample=true
```

| Surface | Trimming/NativeAOT | Notes |
|---|---|---|
| Values, strings, collections, dictionaries, numerics, time, actions/async, HTTP, IO, streams | Safe | No reflection over arbitrary type metadata. |
| Scopes, culture, formatters, validators, CI reports | Safe | Formatting is invariant and deterministic. |
| Type/member/assembly/parameter assertions, `AllTypes`, `TypeSelector` | `RequiresUnreferencedCode` | Preserve inspected types/members, or use non-reflection assertions. |
| Equivalency (`BeEquivalentTo`, `EquivalencyEvaluation.Evaluate`) | `RequiresUnreferencedCode` | Reflective member walk; no dynamic code generation. |
| Event monitoring | `RequiresUnreferencedCode` + `RequiresDynamicCode` | Reflects events and compiles delegate accessors. |
| Snapshots (`MatchSnapshot`) | `RequiresUnreferencedCode` | Caller stack metadata; object snapshots additionally need dynamic JSON serialization. |
| `BeJsonSerializable` / `BeDataContractSerializable` / object `MatchSnapshot` | `RequiresUnreferencedCode` + `RequiresDynamicCode` | Use source-generated JSON contracts for NativeAOT. |
| `BeXmlSerializable` | `RequiresUnreferencedCode` | `XmlSerializer` needs member metadata. |
| `JsonNode.HaveValue<T>` | `RequiresUnreferencedCode` + `RequiresDynamicCode` | Arbitrary scalar conversion. |
| `CustomAssertionDiscovery` | `RequiresUnreferencedCode` | Scans assemblies and members. |

The library deliberately does **not** set `IsAotCompatible=true`: doing so would turn the remaining linker suggestions for the reflection-heavy families into build errors. The annotations are on the public entry points, so an AOT consumer gets an explicit warning and can either preserve metadata (`DynamicallyAccessedMembers`) or suppress at the call site with a documented reason. The sample itself sets `IsAotCompatible=true` to prove the safe surface stays warning-free.

## 9.7 Performance and release verification

The success path is designed to stay cheap: hot verbs capture the subject expression and compare, but format a message only when they fail. `PerformanceInvariantTests` locks that in with allocation thresholds (never wall-clock bounds), and the `benchmarks/Jeninnet.Assertions.Benchmarks` project publishes repeatable timings:

```powershell
dotnet run --project benchmarks/Jeninnet.Assertions.Benchmarks/Jeninnet.Assertions.Benchmarks.csproj -c Release -- --filter '*' --exporters github
```

The weekly `Benchmarks` workflow runs the same command on Linux and Windows and uploads `BenchmarkDotNet.Artifacts`; it is advisory and never blocks a pull request.

Measured on Windows 11, Intel Core i7-8850H (2.60 GHz, 6 physical/12 logical cores), .NET SDK 10.0.404, .NET 10.0.15, X64 RyuJIT, BenchmarkDotNet 0.15.8, 3 warmup + 5 iterations. Treat the numbers as order-of-magnitude, not a cross-machine comparison:

| Scenario | Mean | Allocated |
|---|---:|---:|
| `Be` (int) success / failure | 4.3 ns / 2.8 μs | 32 B / 664 B |
| `Be` (string) success / failure | 5.0 ns / 3.2 μs | 32 B / 1,160 B |
| `BeTrue` success / failure | 4.0 ns / 2.5 μs | 32 B / 592 B |
| `Contain` (string) success | 14.7 ns | 32 B |
| `Contain` (collection) success | 10.9 ns | 32 B |
| `HaveCount` success | 24.4 ns | 104 B |
| `Throw` success | 2.4 μs | 416 B |
| `AssertionScope` success | 116–120 ns | 488 B |
| `AssertionScope` with 1 / 10 failures | 522 ns / 3.4 μs | 1,056 B / 5,496 B |
| `BeEquivalentTo` small graph | 1.9 μs | 3.66 KB |
| `BeEquivalentTo` 500 objects | 1.1 ms | 2.25 MB |
| `MatchSnapshot` + scrubber (file IO) | 330 μs | 14.9 KB |

The peer comparison was measured in the same session. Each cell is **mean / allocated**:

| Scenario | Jeninnet | AwesomeAssertions 9.6 | Shouldly 4.3 | NFluent 3.1 |
|---|---:|---:|---:|---:|
| `Be` (int) success | **4.3 ns / 32 B** | 139.7 ns / 808 B | 60.5 ns / 304 B | 155.1 ns / 800 B |
| `Be` (int) failure | **2.8 μs / 664 B** | 139.4 μs / 38.9 KB | 170.2 μs / 41.7 KB | 5.8 μs / 5.4 KB |
| `Be` (string) success | **5.0 ns / 32 B** | 154.0 ns / 888 B | 719.0 ns / 992 B | 947.5 ns / 1,856 B |
| `Be` (string) failure | **3.2 μs / 1.16 KB** | 175.5 μs / 42.2 KB | 185.6 μs / 67.0 KB | 7.4 μs / 8.6 KB |
| `BeTrue` success | 4.0 ns / 32 B | 121.0 ns / 664 B | **0.9 ns / 0 B** | 89.9 ns / 568 B |
| `Contain` (string) success | **14.7 ns / 32 B** | 114.3 ns / 624 B | 31.2 ns / 96 B | 359.5 ns / 1,112 B |
| `HaveCount` success | **24.4 ns / 104 B** | 186.9 ns / 1,048 B | 61.8 ns / 304 B | 262.4 ns / 896 B |
| `AssertionScope` success | **116 ns / 488 B** | 360 ns / 1,480 B | 125 ns / 496 B¹ | 471 ns / 1,880 B² |
| `AssertionScope` failure | **522 ns / 1.06 KB** | 128.5 μs / 44.9 KB | 515.2 μs / 136.2 KB | 6.4 μs / 5.4 KB² |

¹ Shouldly has no aggregating scope; the row uses `ShouldSatisfyAllConditions`, which evaluates both conditions and aggregates failures.
² NFluent has no aggregating scope; the row measures two sequential fail-fast `Check.That` statements, so it is not a direct semantic match.

In this run Jeninnet had the lowest mean and allocation in most scenarios. The exception is Shouldly's allocation-free `ShouldBeTrue` (0.9 ns), which is faster than Jeninnet's 4.0 ns. Failures are dominated by message formatting in every library; the numbers are order-of-magnitude, not a cross-machine verdict. `Throw`, equivalency, and snapshot rows are from the earlier engine run; the peer table is the 2026-11-15 run.

Snapshot timing is deliberately included as a separate line: it performs file IO and is not a hot verb.

The release path is verified without publishing:

```powershell
dotnet pack src/Jeninnet.Assertions/Jeninnet.Assertions.csproj -c Release --output artifacts/packages
pwsh scripts/release/verify-package.ps1
```

The script checks the four `lib/<tfm>` assemblies with their XML docs, README/icon, the MIT license and repository commit in the nuspec, the absence of test-framework dependencies, PDBs in the sibling `.snupkg`, and that no source, test, or private files leak into either archive. `docs/release-checklist.md` lists the full pre-release sequence; the CI `package-validation` job runs the same verification.

## 9.8 Recap and exercise

**Recap:**

- **Inside `AssertionScope`, collect independent failures**; outside, throw on first failure. Use `Discard()` to inspect aggregation without failing.
- **Culture is isolated per async flow** via `AsyncLocal`. Values remain in `InvariantCulture`. Add languages via `.resx` files only.
- **Formatters change display, not comparison.** Register per-scope or globally for clean domain-type rendering.
- **Lift defaults to configuration.** Use `AssertionConfiguration` and assembly-level `AssertionEngineInitializer` to avoid repetition.
- **Inject a clock for time-relative assertions.** `AssertionClock.Current` (isolated per async flow) makes window boundaries deterministic without sleeps.
- **Measure allocations, not wall time, in tests.** The success path formats nothing; benchmarks publish the order of magnitude, and the package is verified before release.

**Exercise:**

1. Put three independent checks inside an `AssertionScope` and make two fail—observe the combined report.
2. Experiment with culture isolation: set `AssertionCulture.Current = new CultureInfo("ar")` in a test and see the localized failure message if translations exist.
3. Create a formatter for a custom `Money` type and register it globally. Make a failing assertion and see how it renders.
4. Move the formatter registration to an `AssertionEngineInitializer` at the assembly level.

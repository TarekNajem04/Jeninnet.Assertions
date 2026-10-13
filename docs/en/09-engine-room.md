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

2. Translate the `<value>` entries. Translate any subset — untranslated keys fall back to the neutral English text automatically, so a satellite can grow over time. Keep the `{0}`, `{1}`, … placeholders and leave formatted values (`true`, numbers, dates) untouched, so failure messages stay byte-comparable.
3. Build. The SDK emits the satellite assembly (`Strings.<culture>.resources.dll`) with no further wiring.

Read translated templates for custom reporting through the public reader:

```csharp
AssertionCulture.Current = new CultureInfo("ar");
var template = AssertionCulture.GetMessage("BOOLEAN_BE_TRUE");       // localized template
var message = AssertionCulture.FormatMessage("BOOLEAN_BE_TRUE", "flag"); // invariant formatting
```

English is the neutral resource; Arabic (`ar`) ships complete, and French (`fr`) is a partial reference satellite that demonstrates per-key fallback for the entries it does not translate.

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

Deliberate non-goal: this `IValueFormatter` contract (`CanFormat` + `Format`) is intentionally **not** compatible with the same-named interface in other fluent assertion libraries. Aligning them would break every existing formatter without adding capability.

## 9.4 Configuration and engine initializers

`AssertionConfiguration` exposes two main global settings:

- `Formatting` — value formatters (as above)
- `Equivalency` — defaults for `BeEquivalentTo()`: `StrictOrderingDefault`, a shared custom-step `Plan`, and an ambient template mutated with `Modify(o => { o.StrictOrdering = true; })`, snapshotted with `CloneDefaults()`, and cleared with `EquivalencyDefaults.ResetToDefaults()`

`Modify` overrides are isolated per async flow (`AsyncLocal`), exactly like scopes and culture: parallel tests never see each other's overrides. Per-assertion configuration always wins over the ambient template, which wins over the process-wide defaults. Failure routing is pluggable through the public `IAssertionStrategy` (`AssertionScope(IAssertionStrategy)`), with a default collecting strategy aggregating failures until a scope disposes. Deliberate non-goals, recorded here: no public `AssertionChain` (a `GivenSelector` chain covers conditional multi-step checks; otherwise extension goes through `IEquivalencyStep`, rules, `AssertionScope`, and formatters instead of a second assertion pipeline), and no auto-fix option anywhere (rewriting the subject inside an assertion would mutate user data).

### Isolating ambient state

Settings changed by a test should not leak into the next one. One line isolates all four ambient knobs — culture, clock, the global formatter registry, and the ambient equivalency template — and restores them on dispose, even when the scope exits through an exception:

```csharp
using(AssertionIsolation.Isolate()) {
    AssertionCulture.Current = new CultureInfo("ar");
    AssertionClock.Current = new ManualClock(epoch);
    AssertionConfiguration.Formatting.AddFormatter(new MoneyFormatter());
    AssertionConfiguration.Equivalency.Modify(o => o.StrictOrdering = true);

    // ... a test that expects all of the above ...
}
// culture, clock, formatters, and equivalency are back to their pre-scope values.
```

Nested scopes each restore their own snapshot, and the culture/clock/equivalency parts are per-async-flow (`AsyncLocal`), so parallel tests stay isolated. Two boundaries are deliberate: the formatter registry is process-wide (the scope prevents sequential leakage, but parallel tests that both mutate it still need coordination), and state outside this library — like the thread's `CultureInfo.CurrentCulture` — is not touched.

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

## 9.6 Recap and exercise

**Recap:**

- **Inside `AssertionScope`, collect independent failures**; outside, throw on first failure. Use `Discard()` to inspect aggregation without failing.
- **Culture is isolated per async flow** via `AsyncLocal`. Values remain in `InvariantCulture`. Add languages via `.resx` files only.
- **Formatters change display, not comparison.** Register per-scope or globally for clean domain-type rendering.
- **Lift defaults to configuration.** Use `AssertionConfiguration` and assembly-level `AssertionEngineInitializer` to avoid repetition.
- **Inject a clock for time-relative assertions.** `AssertionClock.Current` (isolated per async flow) makes window boundaries deterministic without sleeps.

**Exercise:**

1. Put three independent checks inside an `AssertionScope` and make two fail—observe the combined report.
2. Experiment with culture isolation: set `AssertionCulture.Current = new CultureInfo("ar")` in a test and see the localized failure message if translations exist.
3. Create a formatter for a custom `Money` type and register it globally. Make a failing assertion and see how it renders.
4. Move the formatter registration to an `AssertionEngineInitializer` at the assembly level.

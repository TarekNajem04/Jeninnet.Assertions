# Chapter 6 — Shape, Not Identity: Equivalency and Snapshots

Often, a value passes through multiple transformations: database entity → domain model → DTO for the API response. The types are different, the references are different, but **the data is the same**. If we use `Be()`, it fails because it's comparing references or exact types—despite the data being correct.

This is where we move from "equal" to "equivalent by shape". We compare **structure**, not identity; we exclude what's naturally different (like IDs or timestamps); and we capture large outputs as **snapshots** instead of writing dozens of individual assertions.

## 6.1 `BeEquivalentTo()`: Comparing by structure

`BeEquivalentTo()` compares objects based on their members, regardless of type or reference:

```csharp
order.Should().BeEquivalentTo(expected);
order.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.Id));
order.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.Address.City));
order.Should().BeEquivalentTo(expected, options =>
    options.Excluding(x => x.Address).Including(x => x.Address.City));
```

**Practical rule:** exclude what's different by nature, not what you're afraid might fail. Common examples are `Id`, `CreatedAt`, `UpdatedAt`, and sequence numbers. Everything else is compared strictly by default.

Configuration options include:

- `Excluding()` / `Including()` for member paths (nested paths are supported).
- `WithStrictOrdering()` / `WithoutStrictOrdering()` to control ordering in nested collections.
- `ComparingByValue<T>()` for types that should be compared by their value semantics.
- `AllowingInfiniteRecursion()` for rare cyclic graphs (the default cycle handling is safe and prevents infinite recursion).

You can set global defaults via `AssertionConfiguration.Equivalency` (like `StrictOrderingDefault`) to enforce consistent policies across your test suite.

Note: excluding a non-member expression like `x => 42` throws `ArgumentException`—the message will be clear, but it's worth keeping in mind.

### 6.1.1 Choosing which members take part

```csharp
// by name, anywhere in the graph
dto.Should().BeEquivalentTo(entity, o => o.ExcludingMembersNamed("RowVersion", "ETag"));
// by the member's value type (derived types included; open generics work too)
dto.Should().BeEquivalentTo(entity, o => o.Excluding<Money>().Excluding(typeof(ImmutableArray<>)));
// by predicate over the reflection member
dto.Should().BeEquivalentTo(entity, o => o.Excluding((MemberInfo m) => m.Name.StartsWith("Internal")));
dto.Should().BeEquivalentTo(entity, o => o.Including((MemberInfo m) => m.Name is "Id" or "Total"));
```

| Option | Effect |
| --- | --- |
| `IncludingProperties()` / `IncludingFields()` | compare public properties / public fields only |
| `IncludingInternalProperties()` / `IncludingInternalFields()` | also compare non-public properties / fields |
| `ExcludingProperties()` / `ExcludingFields()` | stop comparing properties / fields |
| `IncludingAllDeclaredProperties()` | only the properties **declared** on the runtime type (inherited ones are skipped) |
| `IncludingAllRuntimeProperties()` | the properties visible on the runtime type, inherited ones included |
| `ExcludingExplicitlyImplementedProperties()` | skip explicit interface implementations (the runtime exposes them as non-public dotted members) |
| `ExcludingNonBrowsableMembers()` | skip expectation members marked `[EditorBrowsable(Never)]` |
| `IgnoringNonBrowsableMembersOnSubject()` | treat a non-browsable subject member as if it did not exist |
| `ExcludingMissingMembers()` | ignore expectation members the subject does not declare |
| `ThrowingOnMissingMembers()` | require them again (the default) |

**Precedence: the last call wins.** `IncludingProperties().IncludingFields()` compares fields only, and the reverse order compares properties only. Selection rules (`Excluding`/`Including` by path, name, type or predicate) *accumulate* instead—so `IncludingAllDeclaredProperties()` and `IncludingAllRuntimeProperties()` **clear** them to restore pure visibility-based selection. Mapping rules accumulate independently of selection.

### 6.1.2 Renaming members with `WithMapping`

The expectation and the subject may name the same thing differently:

```csharp
dto.Should().BeEquivalentTo(entity, o => o.WithMapping("Total", "Amount"));   // by path
dto.Should().BeEquivalentTo(entity, o => o.WithMapping("Address.City", "Address.Town"));
dto.Should().BeEquivalentTo(entity, o => o.WithMapping(e => e.Id, s => s.OrderId));   // by expression
// scoped to a pair of nested types, wherever they appear in the graph
dto.Should().BeEquivalentTo(entity, o => o.WithMapping<MoneyDto, Money>(o => o.Amount, m => m.Value));
```

Index segments in collection paths are ignored by mapping rules, so one rule covers every element. The two names are independent—no literal match is forced between them.

### 6.1.3 Nested collections with `For(...)`

`For(...)` selects a collection and hands you a builder, so rules for deeply nested elements stay in a single fluent expression:

```csharp
order.Should().BeEquivalentTo(expected, o => o
    .For(x => x.Lines)
    .Exclude(x => x.UnitPrice));                    // one member of every line

order.Should().BeEquivalentTo(expected, o => o
    .For(x => x.Lines)
    .For(x => x.Options)
    .Exclude(x => x.Code));                         // chained into a nested collection

order.Should().BeEquivalentTo(expected, o => o
    .For(x => x.Lines)
    .ExcludeAll());                                 // the collection and everything under it
```

The rule applies only to the selected path: excluding `Lines.UnitPrice` leaves a sibling `LinesDiscount` untouched, and it never leaks to a same-named member at another level.

### 6.1.4 Meaning options: strictness, conversion, and shape

Selection decides *which* members compare; meaning options decide *how*. The last call wins, with one documented priority: **strict typing beats auto-conversion** on the same member, in either call order.

```csharp
// int vs long members fail by default; opt into conversion...
actual.Should().BeEquivalentTo(expected, o => o.WithAutoConversion());
// ...or require exact runtime types everywhere (duck-typing off).
actual.Should().BeEquivalentTo(expected, o => o.WithStrictTyping());
// Scope either rule to one member edge:
actual.Should().BeEquivalentTo(expected, o => o.WithAutoConversionFor(m => m.Name == "Total"));
```

Related pairs: `WithoutStrictTyping[For]` / `WithoutAutoConversion[For]` switch the rules back off; `PreferringDeclaredMemberTypes()` compares the graph root against the compile-time expectation type (`PreferringRuntimeMemberTypes()` / `PreferringDefaultMemberTypes()` restore the default); `ComparingEnumsByValue()` matches equal numbers across enum types while `ComparingEnumsByName()` matches identifiers; `ComparingByMembers<T>()` forces memberwise comparison for value-compared types; `ComparingRecordsByValue()` / `ComparingRecordsByMembers()` switch record comparison; `ComparingNullStringsAsEmpty()` / `ComparingNullCollectionsAsEmpty()` equate null with empty; `IgnoringCyclicReferences()` cuts cycles off like `AllowingInfiniteRecursion()` (last call wins); `WithoutRecursing()` compares nested values with `Equals` (`IncludingNestedObjects()` restores depth); `WithStrictOrderingFor(x => x.Tags)` scopes ordering to one collection; `IgnoringJsonPropertyCasing()` matches string dictionary keys ignoring case. There is deliberately no auto-fix option: rewriting the subject inside an assertion would mutate user data.

### 6.1.5 Extending the comparison: steps, plans, and rules

When options are not enough, plug in your own rule. A custom `IEquivalencyStep` sees every node before the built-in comparison does:

```csharp
public sealed class MoneyStep : EquivalencyStep {
    public override bool Handles(Comparands comparands) => comparands.Expectation is Money;
    public override bool TryHandle(Comparands comparands, IObjectGraph graph, IEquivalencyStepContext context) {
        var subject = (Money?)comparands.Subject;
        var expectation = (Money)comparands.Expectation!;
        if(subject is null || subject.Amount != expectation.Amount) {
            context.Fail($"{comparands.FormattedPath} was {subject?.Amount} instead of {expectation.Amount}.");
        }
        return true; // owned: skip the built-in comparison for this node
    }
}

order.Should().BeEquivalentTo(expected, o => o.Using(new MoneyStep()));
```

The lifecycle is `Handles` → `TryHandle` (return `false` to fall through to `AssertEquivalency` for validation-style steps). Steps live in an `EquivalencyPlan` with `Add`, `Insert`, `AddBefore`/`AddAfter`, `Remove`, `Reset`, and `Clear`; register per assertion with `Using(...)` or process-wide with `AssertionConfiguration.Equivalency.Plan`. Finer hooks exist too: `UsingSelectionRule` (`IMemberSelectionRule`), `UsingMatchingRule` (`IMemberMatchingRule`), and `UsingOrderingRule` (`IOrderingRule`), each with a `Without*Rules()` off-switch. For one-off value checks without a graph, use `Value.ThatMatches(value, x => x > 0)` or `Value.ThatSatisfies(value, v => v.Should().BePositive())`.

Two diagnostics help when a graph still differs. `WithTracing(new StringBuilderTraceWriter())` records every node and step decision; `WithFullDump()` appends both object trees plus a `With configuration:` summary to the failure message. Tracing and dumps are invariant diagnostics prose, never localized. **Security:** the dump prints every member value, so enable it only for non-sensitive data and never persist the output.

### 6.1.6 Tolerances: clock skew and rounding

Real systems rarely echo timestamps and floating-point values exactly. Two options absorb that noise graph-wide: `WithDateTimeTolerance(TimeSpan)` compares every `DateTime`/`DateTimeOffset` as an instant within the given window, and `WithNumericTolerance(epsilon)` does the same for `float`/`double`/`decimal` by absolute difference. Integers never become fuzzy:

```csharp
actual.Should().BeEquivalentTo(expected, o => o
    .WithDateTimeTolerance(TimeSpan.FromSeconds(2))
    .WithNumericTolerance(0.01));
```

Scope a wider or narrower window to one member with the `...For` forms (a scoped rule overrides the graph-wide one on its path):

```csharp
actual.Should().BeEquivalentTo(expected, o => o
    .WithDateTimeTolerance(TimeSpan.FromSeconds(1))
    .WithDateTimeToleranceFor(TimeSpan.FromMinutes(5), x => x.SessionExpiresAt));
```

A value outside its tolerance fails with the usual difference plus a note naming the actual difference and the allowed tolerance (`The difference 00:00:05 exceeds the allowed tolerance 00:00:01.`). Custom steps run before the tolerance check, so `Using(...)` keeps ownership wherever it handles a node.

## 6.2 Collections and strings in equivalency

By default, collections in `BeEquivalentTo()` compare as **multisets**: **order is not required, and duplicates are counted**. This fits most real-world scenarios:

```csharp
codes.Should().BeEquivalentTo(["A", "B"], options => options.IgnoringCase());
codes.Should().BeEquivalentTo([1, 2, 3], options => options.WithStrictOrdering());
```

If ordering is truly part of the contract, make it explicit with `WithStrictOrdering()`. For strings, additional graph-wide options are available:

```csharp
order.Should().BeEquivalentTo(expected, o => o.IgnoringCase().IgnoringNewlineStyle());
order.Should().BeEquivalentTo(expected, o => o.IgnoringLeadingWhitespace().IgnoringTrailingWhitespace());
```

Note: `IgnoringCase()` on a non-string type throws `InvalidOperationException`—it only makes sense for text.

## 6.3 Snapshots: Pinning large outputs

When output is large (invoices, full JSON responses, long reports...), describing every detail in assertions is tedious and brittle. A small formatting change can break dozens of individual assertions. Snapshots solve this: **review once, pin the baseline, then let diffs reveal future changes**.

```csharp
invoiceText.Should().MatchSnapshot();                    // invoice.verified.txt next to the test
invoiceText.Should().MatchSnapshot("feb-07");            // explicit name
invoiceText.Should().MatchSnapshot("id", directory, options =>
    options.Scrub(s => s.Replace("42", "<id>")));
```

### Strict, safe workflow

A snapshot is never accepted on your behalf. When no baseline exists — or when the produced output differs from it — the assertion writes a `.received` file and fails, showing you either the received content or a unified diff. Accepting is an action you take deliberately: review the evidence, then rename the file to `.verified`. An exact match passes and deletes any stale `.received` file, so the pair always reflects a decision someone made rather than whatever the code last produced. Volatile content that changes between runs — GUIDs, timestamps, request IDs — is masked with explicit `Scrub()` rules before the baseline is pinned, and baselines are committed with `LF` line endings so a checkout on any OS compares byte for byte. [Chapter 12](12-snapshots.md) develops the operational details, including the one switch that re-baselines in bulk.

## 6.4 Recap and exercise

**Recap:**

- **Shape, not identity.** Use `BeEquivalentTo()` to compare objects of different types with the same data.
- **Exclude what's naturally different.** Focus on what matters to the contract—exclude IDs, timestamps, and similar volatile fields.
- **Collections are multisets by default.** Only enforce ordering with `WithStrictOrdering()` when it's truly part of the contract.
- **Snapshots are for large outputs.** Review once, pin the baseline, and let unified diffs show future changes clearly.

**Exercise:**

1. Take an entity and a DTO with the same values, compare them with `BeEquivalentTo()` excluding at least one field.
2. Create a snapshot of a longer text (or small JSON). Run it the first time, inspect `.received`, then accept it by renaming to `.verified`.
3. Make a small change to that text and run again—observe the unified diff clearly showing what changed.

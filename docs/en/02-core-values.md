# Chapter 2 — Core Values: `bool`, `object`, Numbers, and Strings

In Chapter 1, we focused on reading failure messages. Now we turn to the everyday values we check most often: booleans, objects, numbers, and strings.

The goal remains the same: write assertions that read like natural statements and produce clear, self-explanatory failure messages when they fail.

## 2.1 Booleans: `BeTrue()` and `BeFalse()`

The simplest checks are the clearest:

```csharp
isReady.Should().BeTrue();
hasErrors.Should().BeFalse();
```

On failure:

```text
Expected isReady to be true, but found false.
```

For `bool?`, the same rule applies. If the value is `null`, `BeTrue()` or `BeFalse()` will fail—`null` is neither true nor false.

## 2.2 Objects: `Be()`, `BeSameAs()`, and `BeOfType()`

When comparing an object to an expected value, start with `Be()`:

```csharp
name.Should().Be("alice");
id.Should().Be(42);
```

`Be()` uses `EqualityComparer<T>.Default`, which means it compares **by value**, not by reference identity.

When you need to verify it's the **exact same instance**, use `BeSameAs()`:

```csharp
var x = new User("alice");
var y = x;

y.Should().BeSameAs(x); // same reference
```

If we had `var z = new User("alice")` with identical values, `BeSameAs(z)` would fail even though values are equal. That distinction matters, and the failure message makes it clear.

To check the runtime type:

```csharp
result.Should().BeOfType<Success>();
result.Should().NotBeOfType<Failure>();
```

Sometimes you want to assert the object is assignable to a base type—in that case, prefer `BeAssignableTo<T>()`.

When the default equality is not what you need, pass a custom comparer. The subject stays typed as-is; the comparer decides what "equal" means:

```csharp
object name = "Alice";
name.Should().Be("ALICE", StringComparer.OrdinalIgnoreCase);
name.Should().NotBe("Bob", StringComparer.OrdinalIgnoreCase);
name.Should().BeOneOf(["alice", "bob"], StringComparer.OrdinalIgnoreCase);
```

Passing `null` keeps `EqualityComparer<T>.Default`. A non-generic `IEqualityComparer` is rejected at compile time because it cannot compare the subject safely.

## 2.3 Numbers and nullable types

As we saw in Chapter 1, the assertion class differs based on nullability. For nullable numeric types like `int?`, you get `BeNull()` and `NotBeNull()` alongside value comparisons:

```csharp
age.Should().NotBeNull().And.BeGreaterThan(18);
discount.Should().BeInRange(0, 10);
```

Common numeric assertions:

- `BeGreaterThan()` / `BeLessThan()`
- `BeGreaterOrEqualTo()` / `BeLessOrEqualTo()`
- `BeInRange(min, max)`
- `BePositive()` / `BeNegative()`
- `BeApproximately(precision)` for decimals or floating point
- `BeOneOf(params ...)` to check membership

```csharp
price.Should().BeApproximately(9.99m, precision: 0.01m);
score.Should().BeOneOf(0, 1, 2, 3);
```

A practical note: `BeApproximately()` is best for measurements or approximate calculations. For monetary values, prefer explicit ranges with `BeInRange()` to avoid ambiguity.

## 2.4 Strings: equality, length, containment, and patterns

We can build string checks progressively, expressing exactly what we're looking for:

```csharp
name.Should().Be("ada");
name.Should().HaveLength(3);
code.Should().StartWith("USR");
code.Should().EndWith("01");
email.Should().Contain("@");
email.Should().MatchRegex(@"^[^@]+@[^@]+\.[^@]+$");
```

These can be chained naturally:

```csharp
email.Should().Contain("@").And.MatchRegex(@"^[^@]+@[^@]+\.[^@]+$");
```

For casing and whitespace:

```csharp
code.Should().BeUpperCased();
name.Should().NotBeNullOrWhiteSpace();
comment.Should().BeNullOrWhiteSpace();
```

Multiline text is asserted line by line — matching is whole-line, never partial:

```csharp
report.Should().ContainLine("total: 42").Which.Should().Be("total: 42");
report.Should().NotContainLine("total: 43");
report.Should().HaveLineCount(3).And.NotHaveLineCount(4);
```

Lines split on `\n`: `"a\nb"` has 2 lines, `"a\nb\n"` has 3, and `""` has 0.

The `*EquivalentOf` family compares ignoring casing by default; every verb also takes options for whitespace and newline style (a `null` config selects the defaults):

```csharp
name.Should().ContainEquivalentOf("ada", static o => o.IgnoringLeadingWhitespace());
name.Should().StartWithEquivalentOf("a", static o => o.IgnoringNewlineStyle());
name.Should().MatchEquivalentOf("a*", static o => o.IgnoringTrailingWhitespace());
```

When a long string assertion fails, the library shows a clear, unified diff between expected and actual, rather than dumping two large blocks of text with no indication of what changed.

### Regex groups

A `Match` result asserts directly — success, the whole match text, named or numbered capture groups, and the group count (group 0 is the whole match; requires net6.0+):

```csharp
var logLine = Regex.Match("2026-01-02 ERROR disk full", @"(?<date>\d{4}-\d{2}-\d{2}) (?<level>\w+)");
logLine.Should().Succeed();
logLine.Should().HaveGroup("date", "2026-01-02");
logLine.Should().HaveGroup("level", "ERROR");
logLine.Should().HaveGroupCount(3);
```

A failed or `null` match fails these verbs with a dedicated message, a capture group that did not participate (an unmatched optional group) counts as the empty string, and a name that does not exist lists the available group names.

## 2.5 Checking multiple independent conditions

Sometimes one value needs to satisfy several independent rules. Instead of stopping at the first failure, `SatisfyAllConditions()` reports every failure at once:

```csharp
order.SatisfyAllConditions(
    static o => o.Total.Should().BePositive(),
    static o => o.Lines.Should().NotBeEmpty(),
    static o => o.Customer.Should().NotBeNull());
```

Failure output:

```text
Expected value to satisfy all conditions, but the following failed:
1. Expected o.Total to be positive, but found -5.
2. Expected o.Lines not to be empty, but it was empty.
```

This is especially useful when diagnosing complex preconditions in one pass. Only use it for independent checks—dependent checks should remain sequential.

## 2.6 Recap and exercise

**Recap:**

- `BeTrue()` and `BeFalse()` are for explicit boolean checks. `null` fails both.
- `Be()` compares by value; `BeSameAs()` compares by reference identity. `BeOfType()` enforces the exact runtime type.
- For nullable types, start with `BeNull()`/`NotBeNull()` before asserting values.
- String assertions can be composed naturally. Long comparisons show a unified diff on failure.
- `SatisfyAllConditions()` collects all independent failures instead of stopping at the first.

**Exercise:**

1. Create variables of types `bool`, `string`, and `int?`.
2. Write assertions covering true/false, value/null cases.
3. Chain at least two string assertions with `.And` and add a short `because`.
4. Make one fail and observe the failure message—does it explain itself clearly?

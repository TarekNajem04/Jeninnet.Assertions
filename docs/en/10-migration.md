# Migration Playbook — From Classic Asserts to `Should()`

This playbook is the companion to [Chapter 10](10-ecosystem.md). It walks the optional Roslyn analyzer (`Jeninnet.Assertions.Analyzer`) through every framework it understands, shows the exact rewrite it produces, and lists what it deliberately leaves for you. The intent is a mechanical, reviewable migration — not a blind rewrite.

## 1. Turn the analyzer on

The analyzer ships as a separate, `netstandard2.0` package that loads in every Roslyn host (Visual Studio, VS Code with C#, JetBrains Rider, `dotnet build`):

```shell
dotnet add package Jeninnet.Assertions.Analyzer
```

It reports two `Info` diagnostics and offers a code fix for each (the `JNA` prefix stands for Jeninnet Assertions):

- **`JNA1001`** — equality: `Assert.AreEqual` / `AreNotEqual`, xUnit `Assert.Equal` / `NotEqual`, NUnit `Assert.That(actual, Is.EqualTo(...))`.
- **`JNA1002`** — conditions: `Assert.IsTrue` / `IsFalse` / `IsNull` / `IsNotNull`, xUnit `Assert.True` / `False` / `Null` / `NotNull`, NUnit `Assert.That(actual, Is.True / False / Null / Not.Null)`.

`Info` never fails a build, so the migration is always your decision.

## 2. Apply the fix

- **One call at a time:** put the caret on the call and accept the lightbulb fix (`Convert to 'Be'`). The receiver becomes `Should()` and the reason argument is carried over as `because`.
- **A whole file or project:** use **Fix All** in the fix scope menu (Document / Project / Solution). Fixes are batched and applied together.
- **`using` is added for you:** the code fix inserts `using Jeninnet.Assertions;` when it is missing.

## 3. Cheat sheet

### MSTest (`Microsoft.VisualStudio.TestTools.UnitTesting.Assert`)

| Before | After |
|---|---|
| `Assert.AreEqual(expected, actual)` | `actual.Should().Be(expected)` |
| `Assert.AreNotEqual(expected, actual)` | `actual.Should().NotBe(expected)` |
| `Assert.AreEqual(true, flag)` | `flag.Should().BeTrue()` |
| `Assert.IsTrue(flag)` | `flag.Should().BeTrue()` |
| `Assert.IsFalse(flag)` | `flag.Should().BeFalse()` |
| `Assert.IsNull(value)` | `value.Should().BeNull()` |
| `Assert.IsNotNull(value)` | `value.Should().NotBeNull()` |

### xUnit (`Xunit.Assert`)

| Before | After |
|---|---|
| `Assert.Equal(expected, actual)` | `actual.Should().Be(expected)` |
| `Assert.NotEqual(expected, actual)` | `actual.Should().NotBe(expected)` |
| `Assert.True(flag)` | `flag.Should().BeTrue()` |
| `Assert.False(flag)` | `flag.Should().BeFalse()` |
| `Assert.Null(value)` | `value.Should().BeNull()` |
| `Assert.NotNull(value)` | `value.Should().NotBeNull()` |
| `Assert.Contains(fragment, actual)` | `actual.Should().Contain(fragment)` |

### NUnit classic (`NUnit.Framework.Assert`)

`AreEqual`, `AreNotEqual`, `IsTrue`, `IsFalse`, `IsNull`, and `IsNotNull` map exactly as the MSTest table above.

### NUnit constraint model (`Assert.That`)

| Before | After |
|---|---|
| `Assert.That(actual, Is.EqualTo(expected))` | `actual.Should().Be(expected)` |
| `Assert.That(actual, Is.Not.EqualTo(expected))` | `actual.Should().NotBe(expected)` |
| `Assert.That(flag, Is.True)` | `flag.Should().BeTrue()` |
| `Assert.That(flag, Is.False)` | `flag.Should().BeFalse()` |
| `Assert.That(value, Is.Null)` | `value.Should().BeNull()` |
| `Assert.That(value, Is.Not.Null)` | `value.Should().NotBeNull()` |

Named arguments (`expected:`, `actual:`, `message:`) are preserved; the message becomes `because`.

## 4. Deliberately not converted

The analyzer only offers a fix when the generated call is guaranteed to compile and to mean the same thing. It stays silent for:

- **Tolerance / delta equality** — `Assert.AreEqual(5.0, value, 0.1)` and xUnit precision overloads.
- **Case- or culture-sensitive equality** — `ignoreCase` flags and `CultureInfo` arguments.
- **Custom comparers** — `IEqualityComparer` overloads.
- **Numeric and enum equality** — `Assert.AreEqual` on plain numbers or enums is left alone so you can choose the right numeric verb (`BeApproximately`, `BeGreaterThan`, …).
- **Complex NUnit constraints** — anything beyond `Is.EqualTo`, `Is.True/False`, `Is.Null`, and their `Is.Not` forms (e.g. `Is.GreaterThan`, `Is.InRange`, `Has.Count`).
- **Literal receivers** — `Assert.AreEqual("a", "b")` has no subject expression to anchor `Should()`.
- **Value-returning asserts** — conversions that would need to change a value's type are never proposed.

For those, migrate by hand so the intent stays explicit.

## 5. After the rewrite

1. **Build** — the code fix only proposes what compiles, but your local `using`s and overloads are authoritative.
2. **Run the tests** — the fluent message is richer, not identical; snapshot-free tests should stay green.
3. **Read the new messages** — if a failure is unclear, add a `because` reason: `actual.Should().Be(expected, "the total must match the order")`.
4. **Delete the old framework reference** once a project has no remaining `Assert` calls.

Legacy migration from other assertion libraries (same verbs, renamed a few) is covered separately in [Chapter 10](10-ecosystem.md).

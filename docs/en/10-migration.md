# Migration Playbook — From Classic Asserts to `Should()`

This playbook is the companion to [Chapter 10](10-ecosystem.md). It walks the optional Roslyn analyzer (`Jeninnet.Assertions.Analyzer`) through every framework it understands, shows the exact rewrite it produces, and lists what it deliberately leaves for you. The intent is a mechanical, reviewable migration — not a blind rewrite. The later sections map Shouldly, NFluent, and AwesomeAssertions/FluentAssertions by hand, including the shapes that must not be translated one-to-one.

## 1. Turn the analyzer on

The analyzer ships as a separate, `netstandard2.0` package that loads in every Roslyn host (Visual Studio, VS Code with C#, JetBrains Rider, `dotnet build`):

```shell
dotnet add package Jeninnet.Assertions.Analyzer
```

It reports nine `Info` diagnostics and offers a code fix for each (the `JNA` prefix stands for Jeninnet Assertions):

- **`JNA1001`** — equality: `Assert.AreEqual` / `AreNotEqual`, xUnit `Assert.Equal` / `NotEqual`, NUnit `Assert.That(actual, Is.EqualTo(...))`.
- **`JNA1002`** — conditions: `Assert.IsTrue` / `IsFalse` / `IsNull` / `IsNotNull`, xUnit `Assert.True` / `False` / `Null` / `NotNull`, NUnit `Assert.That(actual, Is.True / False / Null / Not.Null)`.
- **`JNA1003`** — strings: `StringAssert.Contains` / `StartsWith` / `EndsWith`, xUnit `Assert.Contains` (string) / `StartsWith` / `EndsWith`.
- **`JNA1004`** — collections: `Assert.Empty` / `NotEmpty` / `Single` / `Contains` / `DoesNotContain`, `Assert.IsEmpty` / `IsNotEmpty`, and `CollectionAssert.Contains`.
- **`JNA1005`** — exceptions: `Assert.Throws` / `ThrowsAsync` / `ThrowsException` / `ThrowsExceptionAsync` in statement position.
- **`JNA1006`** — types: `Assert.IsInstanceOfType` / `IsType` / `IsAssignableFrom` in statement position.
- **`JNA1007`** — task state: `Assert.True(task.IsCompleted)` / `Assert.False(task.IsCompleted)`.
- **`JNA2001`** — simple Shouldly verbs: `ShouldBe`, `ShouldNotBe`, `ShouldBeNull`/`ShouldNotBeNull`, `ShouldBeTrue`/`ShouldBeFalse`, `ShouldContain`/`ShouldNotContain`, `ShouldBeEmpty`/`ShouldNotBeEmpty`, and statement-position `ShouldThrow<T>`/`ShouldNotThrow`.
- **`JNA2002`** — simple NFluent chains: `Check.That(x).IsEqualTo`/`IsNotEqualTo`/`IsNull`/`IsNotNull`/`IsTrue`/`IsFalse`/`Contains`/`IsEmpty`/`IsNotEmpty`/`HasSize`, and `Check.ThatCode(code).Throws<T>`, all in statement position.

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
| `StringAssert.Contains(actual, "frag")` | `actual.Should().Contain("frag")` |
| `StringAssert.StartsWith(actual, "start")` | `actual.Should().StartWith("start")` |
| `StringAssert.EndsWith(actual, "end")` | `actual.Should().EndWith("end")` |
| `CollectionAssert.Contains(items, item)` | `items.Should().Contain(item)` |
| `Assert.IsInstanceOfType(value, typeof(Order))` | `value.Should().BeAssignableTo<Order>()` |
| `Assert.IsExactInstanceOfType(value, typeof(Order))` | `value.Should().BeOfType<Order>()` |
| `Assert.Throws<T>(() => Act())` | `new Action(() => Act()).Should().Throw<T>()` |
| `Assert.IsTrue(task.IsCompleted)` | `task.Should().BeCompleted()` |

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
| `Assert.StartsWith(prefix, actual)` | `actual.Should().StartWith(prefix)` |
| `Assert.EndsWith(suffix, actual)` | `actual.Should().EndWith(suffix)` |
| `Assert.Empty(items)` | `items.Should().BeEmpty()` |
| `Assert.NotEmpty(items)` | `items.Should().NotBeEmpty()` |
| `Assert.Single(items)` | `items.Should().ContainSingle()` |
| `Assert.Contains(item, items)` | `items.Should().Contain(item)` |
| `Assert.IsType<Order>(value)` | `value.Should().BeOfType<Order>()` |
| `Assert.IsAssignableFrom<Entity>(value)` | `value.Should().BeAssignableTo<Entity>()` |
| `Assert.Throws<T>(() => Act())` | `new Action(() => Act()).Should().ThrowExactly<T>()` |
| `Assert.True(task.IsCompleted)` | `task.Should().BeCompleted()` |

### NUnit classic (`NUnit.Framework.Assert`)

`AreEqual`, `AreNotEqual`, `IsTrue`, `IsFalse`, `IsNull`, and `IsNotNull` map exactly as the MSTest table above. `StringAssert.Contains`/`StartsWith`/`EndsWith` use the same `Contain`/`StartWith`/`EndWith` targets, `Assert.IsEmpty`/`IsNotEmpty`/`Contains`/`DoesNotContain` map to the collection verbs, `Assert.IsInstanceOf`/`IsAssignableFrom` map to `BeAssignableTo` (derived types allowed), and `Assert.Throws<T>` maps to `ThrowExactly<T>` (NUnit requires the exact exception type).

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
- **String case/culture overloads** — `StringAssert.Contains(actual, text, StringComparison...)` stays manual.
- **Consumed results** — `Assert.Single`, `Assert.IsType`/`IsAssignableFrom`, and exception assertions are converted only when their result is discarded; the fluent calls return assertion objects, not the item/exception/typed value.
- **Plain `Exception`** — `Assert.Throws<Exception>` is left manual because the intended exception contract is unclear.
- **Dictionary containment** — dictionary `Contains` needs `ContainKey`/`ContainValue`, so it is not mapped to `Contain`.
- **Snapshot tools** — `ShouldMatchApproved`/`Verify` have different approval semantics and are never converted.
- **Peer chaining and composite shapes** — `Check.That(x).IsEqualTo(y).And...`, NFluent `Which` chains, and Shouldly `ShouldBeOneOf`/`ShouldBeOfType<T>` are left manual because overload selection, result type, or short-circuit behavior changes.
- **Peer custom messages and tolerances** — Shouldly/NFluent message, tolerance, comparer, and culture overloads are not rewritten; add `because:` only after reviewing the contract.
- **Literal receivers** — `Assert.AreEqual("a", "b")` has no subject expression to anchor `Should()`.
- **Value-returning asserts** — conversions that would need to change a value's type are never proposed.

For those, migrate by hand so the intent stays explicit.

## 5. Coming from Shouldly

Shouldly exposes assertions as extension methods on the subject (`ShouldBe`, `ShouldThrow`, …), so most of the migration is a rename onto `Should().*`. Shouldly is BSD-3-Clause; the table maps intent, not every overload.

`JNA2001` automates the simple subset: equality, null/boolean, containment, emptiness, and statement-position `ShouldThrow<T>`/`ShouldNotThrow`. It leaves `ShouldBeOneOf`, `ShouldBeOfType<T>`, tolerance/comparer/culture overloads, custom messages, `ShouldMatchApproved`, and `DynamicShould` manual.

| Shouldly | Jeninnet | Notes |
|---|---|---|
| `value.ShouldBe(x)` | `value.Should().Be(x)` | Value and reference equality follow the subject type. |
| `value.ShouldNotBe(x)` | `value.Should().NotBe(x)` | |
| `value.ShouldBeNull()` / `ShouldNotBeNull()` | `value.Should().BeNull()` / `NotBeNull()` | |
| `value.ShouldBeTrue()` / `ShouldBeFalse()` | `value.Should().BeTrue()` / `BeFalse()` | |
| `value.ShouldBeOfType<T>()` | `value.Should().BeOfType<T>()` | Use `BeAssignableTo<T>()` when derived types are acceptable. |
| `value.ShouldBeAssignableTo<T>()` | `value.Should().BeAssignableTo<T>()` | |
| `value.ShouldBeOneOf(a, b)` | `value.Should().BeOneOf([a, b])` | `ObjectAssertions.BeOneOf` takes a sequence; string assertions also have a `params` overload. |
| `value.ShouldBeInRange(a, b)` | `value.Should().BeInRange(a, b)` | Available where the subject is comparable. |
| `value.ShouldBeCloseTo(x, tolerance)` | `value.Should().BeApproximately(x, tolerance)` | `tolerance` is the maximum absolute difference, not a number of decimal places. |
| `value.ShouldMatch(regex)` | `value.Should().MatchRegex(regex)` | Shouldly matches a regex; `Match` is wildcard matching, so do not substitute it. |
| `action.ShouldThrow<T>()` | `action.Should().Throw<T>()` | `Throw` accepts derived exceptions; use `ThrowExactly<T>()` for the exact type. |
| `action.ShouldNotThrow()` | `action.Should().NotThrow()` | |
| `task.ShouldCompleteIn(timeout)` | `await task.Should().CompleteWithinAsync(timeout)` | The Jeninnet verb is asynchronous. |
| `value.ShouldMatchApproved()` | `value.Should().MatchSnapshot()` | Different approval semantics; review the snapshot section instead of translating one-to-one. |
| `DynamicShould...` | No counterpart | Assert on `dynamic` values explicitly so the expected type is visible. |

## 6. Coming from NFluent

NFluent centers on `Check.That(subject).*`; replace the root with `subject.Should()` and keep the verb. NFluent is Apache-2.0.

`JNA2002` automates the simple statement forms: equality, null/boolean, string/collection containment, emptiness, `HasSize`, and `Check.ThatCode(...).Throws<T>`. Composite chains (`.And...`), `Which`, tolerance/comparer/culture overloads, and custom messages stay manual.

| NFluent | Jeninnet | Notes |
|---|---|---|
| `Check.That(x).IsEqualTo(y)` | `x.Should().Be(y)` | |
| `Check.That(x).IsNotEqualTo(y)` | `x.Should().NotBe(y)` | |
| `Check.That(x).IsNull()` | `x.Should().BeNull()` | |
| `Check.That(x).IsInstanceOf<T>()` | `x.Should().BeOfType<T>()` | Use `BeAssignableTo<T>()` when derived types are acceptable. |
| `Check.That(x).IsGreaterThan(y)` | `x.Should().BeGreaterThan(y)` | |
| `Check.That(x).IsInRange(a, b)` | `x.Should().BeInRange(a, b)` | |
| `Check.That(x).Contains(...)` | `x.Should().Contain(...)` | Works for strings and collections; dictionaries use `ContainKey`/`ContainValue`. |
| `Check.That(x).ContainsExactly(...)` | `x.Should().Equal(...)` | `Equal` keeps order; use `BeEquivalentTo` only when order is irrelevant. |
| `Check.That(x).IsOnlyMadeOf(...)` | `x.Should().OnlyContain(item => allowed.Contains(item))` | If the exact multiset is the contract, use `BeEquivalentTo`; `OnlyContain` keeps the membership meaning. |
| `Check.That(x).IsInAscendingOrder()` | `x.Should().BeInAscendingOrder()` | |
| `Check.That(x).HasSize(n)` | `x.Should().HaveCount(n)` | |
| `Check.That(x).IsEmpty()` | `x.Should().BeEmpty()` | |
| `Check.That(code).Throws<T>()` | `code.Should().Throw<T>()` | Use `ThrowExactly<T>()` when the exact type is the contract. |
| `Check.That(code).LastsLessThan(ms)` | `Action action = code; action.ExecutionTime().Should().BeLessThan(...)` | Measure the action explicitly; no timing is hidden in a verb. |
| `Check.That(x).Which` | `.Which` where the verb returns it, otherwise `.And` | Chaining shape depends on the verb's return type. |
| `Check.ThatEnum(e)` | `e.Should()` | Enum assertions expose the enum-specific verbs. |
| `Check.That(x).Extracting(...)` | `x.Select(...).Should()` | Or use the keyed `OnlyHaveUniqueItems(selector)` when uniqueness is the intent. |

## 7. Coming from AwesomeAssertions or FluentAssertions

AwesomeAssertions (AA) is a community fork of FluentAssertions (FA). AA is Apache-2.0. FA 7.x and earlier are also Apache-2.0; FA 8 and later use the Xceed Community License — free for non-commercial use, with commercial use requiring a paid Xceed license. Jeninnet is MIT and independent of both.

The AA API follows FA 7.x closely, so the FA-oriented migration material in [Chapter 10](10-ecosystem.md) still applies. The mechanical differences are:

- **Package and namespace.** AA 8.x was a drop-in that kept the `FluentAssertions` namespace and assembly name. AA 9 renamed both to `AwesomeAssertions`. In either case, replace the `using` with `using Jeninnet.Assertions;`.
- **Extension metadata.** AA has `[CustomAssertion]` on methods, `[CustomAssertions]` on classes, and `[CustomAssertionsAssembly]` on assemblies. Jeninnet has `[CustomAssertion]` and `[CustomAssertionsAssembly]`; there is no class-level equivalent yet, so mark each custom method.
- **Parameter metadata.** Jeninnet exposes `ParameterInfo.Should()` with name, exact type, optional/required, default value, `out`/`ref`/`in`/`params`, position, and attribute verbs. AA's attribute-only `ParameterInfoAssertions` maps directly for decoration checks; the broader metadata verbs are Jeninnet-specific.
- **Jeninnet-specific areas.** Localization, the deterministic clock, IO test infrastructure, allocation assertions, and snapshot v2 have no FA/AA counterpart to migrate from.
- **Licensing.** Keep the license of the library you remove in review notes: Shouldly BSD-3-Clause, NFluent and AA Apache-2.0, FA 7.x and earlier Apache-2.0, FA 8+ Xceed Community/commercial, Jeninnet MIT.

## 8. What is never a one-to-one rewrite

Some shapes look mechanical but change meaning when translated. Migrate them by hand and keep the original intent in the test.

| Pattern | Why it is not automatic | Alternative |
|---|---|---|
| `ShouldMatchApproved` / `Verify` | Approval tools snapshot files and naming differently, and often auto-accept on first run. | `MatchSnapshot` with an explicit verified file; review the diff. |
| `DynamicShould...` | `dynamic` calls bind at runtime, so no static rewrite is provable. | Assert on typed locals or explicit `object` casts. |
| Tolerance, comparer, culture, and `StringComparison` overloads | The fluent verb may select a different comparison. | Choose `BeApproximately`, `Contain(..., StringComparison...)`, or an explicit comparer, then review the result. |
| NFluent `Which` and composite constraints | The result type and short-circuit behavior differ. | Use `.Which` only when the Jeninnet verb returns it; otherwise split the assertions. |
| NUnit constraint chains (`Is.GreaterThan`, `Has.Count`, …) | Constraints compose differently from fluent calls. | Write the equivalent verb sequence explicitly. |
| AA class-level `[CustomAssertions]` | Jeninnet has no class-level attribute. | Mark each custom method with `[CustomAssertion]`, or the assembly with `[CustomAssertionsAssembly]`. |
| FA 8+ packages | The license is not a code difference. | Decide the license before migrating; do not copy FA 8 source into an MIT project. |
| `Assert.That(..., Is.Unique)` and set-based membership | Set semantics and multiplicity are easy to mix up. | `OnlyHaveUniqueItems()` for uniqueness; `OnlyContain` for membership; `BeEquivalentTo` for an exact multiset. |

## 9. After the rewrite

1. **Build** — the code fix only proposes what compiles, but your local `using`s and overloads are authoritative.
2. **Run the tests** — the fluent message is richer, not identical; snapshot-free tests should stay green.
3. **Read the new messages** — if a failure is unclear, add a `because` reason: `actual.Should().Be(expected, "the total must match the order")`.
4. **Delete the old framework reference** once a project has no remaining `Assert` calls.

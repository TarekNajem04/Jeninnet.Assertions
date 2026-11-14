# Jeninnet.Assertions.Analyzer

Roslyn analyzer with code fixes that suggests fluent `Should()` assertions over convertible
MSTest, xUnit, and NUnit calls, easing migration to
[Jeninnet.Assertions](https://github.com/TarekNajem04/Jeninnet.Assertions).

## Installation

```shell
dotnet add package Jeninnet.Assertions.Analyzer
```

The analyzer targets `netstandard2.0` (loads in every Roslyn host) and pulls the core
`Jeninnet.Assertions` package automatically, so fixed code always resolves `Should()`.

Compiler requirement: the analyzer is built against Roslyn `5.9.0`, so consuming projects
must build with a .NET SDK whose compiler is `5.9.0` or newer — in practice, a current
.NET 10 SDK (this repository builds with the newest installed `10.0.x` via the
`latestFeature` roll-forward in `global.json`). An older compiler fails the build with
`CS9057`.

## Rules

Rule IDs use the `JNA` prefix (Jeninnet Assertions).

| ID | Severity | Converts |
|---|---|---|
| `JNA1001` | Info | `Assert.AreEqual`/`AreNotEqual`, xUnit `Assert.Equal`/`NotEqual`, NUnit `Is.EqualTo`/`Is.Not.EqualTo` → `Be` / `NotBe` |
| `JNA1002` | Info | `Assert.IsTrue`/`IsFalse`/`IsNull`/`IsNotNull`, xUnit `Assert.True`/`False`/`Null`/`NotNull`, NUnit `Is.True`/`False`/`Null`/`Is.Not.Null` → `BeTrue` / `BeFalse` / `BeNull` / `NotBeNull` |
| `JNA1003` | Info | String assertions: MSTest/NUnit `StringAssert.Contains`/`StartsWith`/`EndsWith`, xUnit `Assert.Contains` (string)/`StartsWith`/`EndsWith` → `Contain` / `StartWith` / `EndWith` |
| `JNA1004` | Info | Collection emptiness/single/containment: xUnit `Assert.Empty`/`NotEmpty`/`Single`/`Contains`/`DoesNotContain`, NUnit `Assert.IsEmpty`/`IsNotEmpty`/`Contains`/`DoesNotContain`, MSTest `CollectionAssert.Contains` → `BeEmpty` / `NotBeEmpty` / `ContainSingle` / `Contain` / `NotContain` |
| `JNA1005` | Info | MSTest `Assert.Throws`/`ThrowsException`[Async], xUnit/NUnit `Assert.Throws`/`ThrowsAsync` in statement position → `Throw`/`ThrowExactly` and async variants |
| `JNA1006` | Info | MSTest `Assert.IsInstanceOfType`, xUnit `Assert.IsType`/`IsAssignableFrom`, NUnit `Assert.IsInstanceOf`/`IsAssignableFrom` in statement position → `BeOfType` / `BeAssignableTo` (and negatives) |
| `JNA1007` | Info | `Assert.True(task.IsCompleted)` / `Assert.False(task.IsCompleted)` → `BeCompleted` / `NotBeCompleted` |

Suggestions appear only when the generated code is guaranteed to compile (verified by speculative
binding); a `message:` argument becomes `because:`. Info severity never breaks
`TreatWarningsAsErrors` builds.

Safety rules worth knowing:

- Tolerance, comparer, culture, and `StringComparison` overloads are left manual.
- Exception and type assertions are converted only in statement position: the framework call returns the
  exception/typed value, while the fluent call returns an assertion object. Exact-throw frameworks
  (xUnit/NUnit `Throws`, MSTest `ThrowsException`) map to `ThrowExactly`; MSTest `Throws` keeps its
  non-strict semantics with `Throw`.
- `Assert.Single` and `Assert.IsType`/`IsAssignableFrom` are skipped when their result is consumed.
- Dictionary `Contains` is left manual (needs `ContainKey`/`ContainValue`).
- Snapshot tools (`ShouldMatchApproved`, `Verify`) are intentionally not converted (different semantics).

## License

MIT — see [LICENSE](https://github.com/TarekNajem04/Jeninnet.Assertions/blob/main/LICENSE).

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

## Rules

Rule IDs use the `JNA` prefix (Jeninnet Assertions).

| ID | Severity | Converts |
|---|---|---|
| `JNA1001` | Info | `Assert.AreEqual`/`AreNotEqual`, xUnit `Assert.Equal`/`NotEqual`, `Assert.Contains`, NUnit `Is.EqualTo`/`Is.Not.EqualTo` → `Be` / `NotBe` / `Contain` |
| `JNA1002` | Info | `Assert.IsTrue`/`IsFalse`/`IsNull`/`IsNotNull`, xUnit `Assert.True`/`False`/`Null`/`NotNull`, NUnit `Is.True`/`False`/`Null`/`Is.Not.Null` → `BeTrue` / `BeFalse` / `BeNull` / `NotBeNull` |

Suggestions appear only when the generated code is guaranteed to compile (verified by speculative
binding); a `message:` argument becomes `because:`. Info severity never breaks
`TreatWarningsAsErrors` builds. Tolerance, comparer, and culture overloads are deliberately left
manual.

## License

MIT — see [LICENSE](https://github.com/TarekNajem04/Jeninnet.Assertions/blob/main/LICENSE).

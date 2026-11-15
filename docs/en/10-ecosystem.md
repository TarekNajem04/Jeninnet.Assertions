# Chapter 10 — The Ecosystem: Framework Adapters, Analyzer, Consumer Template, and Migration

The library alone isn't enough. Teams need practical answers to three questions: How do we integrate with our test framework's reporting? How do we convert thousands of legacy assertion calls? Where does a new developer start?

This chapter covers the surrounding pieces: deliberate framework adapters, a Roslyn analyzer that only suggests safe conversions, a copyable `Consumer` template, and a nearly-mechanical migration map from other assertion libraries.

The common thread is **explicit boundaries, no hidden magic**.

## 10.1 Framework adapters (optional)

The core package throws its own `AssertionFailedException`. Some runners classify it as an **error** rather than a **failure**, which loses an important distinction in test reports. Optional adapter packages solve that explicitly, each translating only `AssertionFailedException` into the framework's native assertion exception while preserving the message and inner exception.

| Package | Framework | TFMs |
|---|---|---|
| `Jeninnet.Assertions.MSTest` | MSTest | `net6.0+` |
| `Jeninnet.Assertions.xUnit` | xUnit v2 | `net6.0+` |
| `Jeninnet.Assertions.xUnit.v3` | xUnit v3 | `net8.0+` |
| `Jeninnet.Assertions.NUnit` | NUnit 4/5 | `net6.0+` |
| `Jeninnet.Assertions.TUnit` | TUnit | `net8.0+` |

### MSTest

```csharp
using Jeninnet.Assertions.MSTest;

MSTestAssertions.That(() => order.Should().Be("shipped"));
await MSTestAssertions.ThatAsync(() => action.Should().NotThrowAsync());
```

Failures surface as MSTest's `AssertFailedException`.

### xUnit (v2 and v3)

```csharp
using Jeninnet.Assertions.Xunit;
using Jeninnet.Assertions.Xunit.V3;

XunitAssertions.That(() => order.Should().Be("shipped"));       // xUnit v2
XunitV3Assertions.That(() => order.Should().Be("shipped"));     // xUnit v3
await XunitV3Assertions.ThatAsync(() => action.Should().NotThrowAsync());
```

The two adapters target different xUnit assertion assemblies, so they are separate packages and namespaces and can coexist during a staged migration. Both surface `Xunit.Sdk.XunitException`.

### NUnit

```csharp
using Jeninnet.Assertions.NUnit;

NUnitAssertions.That(() => order.Should().Be("shipped"));
await NUnitAssertions.ThatAsync(() => action.Should().NotThrowAsync());
```

Failures surface as `NUnit.Framework.AssertionException`. The adapter supports NUnit 4 and 5; the `net6.0` target compiles against NUnit 4.3.2 because NUnit 5 dropped that TFM.

### TUnit

```csharp
using Jeninnet.Assertions.TUnit;

TUnitAssertions.That(() => order.Should().Be("shipped"));
await TUnitAssertions.ThatAsync(() => action.Should().NotThrowAsync());
```

Failures surface as `TUnit.Assertions.Exceptions.AssertionException`.

**Why explicit?** Because the core package must stay framework-agnostic, and a silent integration would contradict that. Wrapping at each call site makes the choice visible in the code that depends on it, and every adapter follows one pattern instead of several. The adapters are exercised by the `FrameworkZoo` samples and by dedicated test projects; the core never references a test framework.

## 10.2 Roslyn analyzer (optional)

The `Jeninnet.Assertions.Analyzer` package provides Roslyn analyzers with code fixes to convert eligible MSTest, xUnit, and NUnit assertion calls. Diagnostics are reported at **Info** severity so they never break builds with `TreatWarningsAsErrors` enabled.

| ID | Converts |
|---|---|
| `JNA1001` | `Assert.AreEqual` / `AreNotEqual`, xUnit `Assert.Equal` / `NotEqual`, NUnit `Is.EqualTo` / `Is.Not.EqualTo` → `Should().Be(...)` / `NotBe(...)` |
| `JNA1002` | `Assert.IsTrue` / `IsFalse` / `IsNull` / `IsNotNull`, xUnit `Assert.True` / `False` / `Null` / `NotNull`, NUnit `Is.True` / `False` / `Null` / `Is.Not.Null` → `BeTrue()` / `BeFalse()` / `BeNull()` / `NotBeNull()` |
| `JNA1003` | String assertions (`StringAssert.Contains`/`StartsWith`/`EndsWith`, xUnit string `Contains`/`StartsWith`/`EndsWith`) → `Contain()` / `StartWith()` / `EndWith()` |
| `JNA1004` | Collection emptiness/single/containment → `BeEmpty()` / `NotBeEmpty()` / `ContainSingle()` / `Contain()` / `NotContain()` |
| `JNA1005` | Exception assertions in statement position → `Throw<T>()` / `ThrowExactly<T>()` and async variants |
| `JNA1006` | Type assertions in statement position → `BeOfType<T>()` / `BeAssignableTo<T>()` |
| `JNA1007` | `Assert.True/False(task.IsCompleted)` → `BeCompleted()` / `NotBeCompleted()` |

The `JNA` prefix stands for Jeninnet Assertions.

**Safety guarantees:**

- **Compile-safe suggestions:** Fixes only appear when the generated code is proven to compile via speculative binding.
- **Conservative by design:** Overloads involving tolerance, custom comparers, or culture are never converted, since meaning can change subtly.
- **Statement-position only:** Exception and type assertions are converted only when their result is discarded; `Single`/`IsType` results that are consumed are skipped.
- **Exact-throw semantics preserved:** xUnit/NUnit `Throws` and MSTest `ThrowsException` map to `ThrowExactly`; MSTest `Throws` keeps its non-strict semantics with `Throw`.
- **Smart message mapping:** `message:` parameters are automatically converted to `because:` while preserving format arguments.
- **Fix All supported:** Document, Project, and Solution scopes batch the same safe rewrites.

The principle is simple: **an analyzer that suggests unsafe conversions is worse than no analyzer at all.** The framework-by-framework before/after tables and the explicit list of unconverted forms live in the [Migration Playbook](10-migration.md).

## 10.3 The Consumer template

The repository contains `consumers/Jeninnet.Assertions.Consumer`, a canonical example that models an order-processing pipeline, together with `consumers/Jeninnet.Assertions.Consumer.Tests`, an MSTest suite that drives it through this library end to end. The suite prices orders and validates them, watches a simulated payment run against a time budget and decline over a threshold, mocks the shipping provider through a fake `HttpMessageHandler`, pins the rendered invoice as a snapshot, asserts the placed-order event and its payload, checks the JSON export by path, and enforces two architectural rules with reflection.

The decisive detail is what the projects reference: the **published NuGet package**, not the source code in the solution. That is what a consumer actually experiences — versions, multi-targeting, package restore — and it keeps the template honest about the packaging story. When you start your own suite, copy the structure rather than the snippets: a folder per concern, assertions named for the capability they check, and one-time configuration lifted to an `AssertionEngineInitializer` instead of repeated per fixture. If you want the same behaviors written for several runners at once, `samples/Jeninnet.Assertions.FrameworkZoo` runs one shared suite on xUnit, NUnit, MSTest, Fixie, Expecto, and TUnit.

## 10.4 Migrating from other assertion libraries

The syntax deliberately stays close to the established fluent assertion vocabulary so migration is nearly mechanical via search/replace and the analyzer.

| Other library | Jeninnet.Assertions |
|---|---|
| `x.Should().Be(y)` | `x.Should().Be(y)` |
| `x.Should().NotBe(y)` | `x.Should().NotBe(y)` |
| `x.Should().BeEquivalentTo(y, o => o.Excluding(...))` | `x.Should().BeEquivalentTo(y, o => o.Excluding(...))` — prefer `static` lambdas for options |
| `act.Should().Throw<T>()` | `act.Should().Throw<T>()` |
| `act.Should().Throw<T>().WithMessage(...)` | `act.Should().Throw<T>().WithMessage(...)` |
| `using (new AssertionScope()) { ... }` | `using (new AssertionScope()) { ... }` |
| `x.Should().SatisfyAllConditions(a,b,c)` | `x.SatisfyAllConditions(a,b,c)` — direct extension on the value |
| `x.Should().BeApproved()` / ApprovalTests snapshots | `x.Should().MatchSnapshot()` — explicit `.received` → review → `.verified` workflow |

**Key differences to internalize.** The library prefers deliberate breaks over silent decay: there are no `[Obsolete]` shims, so a breaking change forces a correction at the call site instead of letting deprecated forms accumulate. Cancellation carries meaning — `NotThrowAsync()` fails when a task is canceled rather than treating cancellation as quiet success. Several capabilities have no counterpart in the libraries above: allocation assertions, HTTP assertions, `TestEnvironment` for I/O, and reflection-based architecture rules. Failure messages are fully localizable through resource files rather than locked to English. And aggregation follows one rule everywhere: independent failures are collected in a scope, while dependent checks remain sequential.

## 10.5 FAQ

**Which target should I use for my tests?**

The library supports `netstandard2.0;net6.0;net8.0;net10.0;net11.0`. New tests are best written for `net11.0` (like this repository's tests, which use MSTest). You can use it with any of the supported targets.

**Is this library meant for production code?**

No. It's for tests only. Its exceptions are diagnostic tools, not control flow mechanisms. Using it in production is not the intended use case.

**What if a failure message isn't clear?**

Treat unclear messages as a library issue, not a test issue. Failure messages are covered by precise string tests; improving them means updating message templates, translations (including Arabic), and the corresponding tests together. The goal is always that a failure message explains itself without opening the code.

## 10.6 Conclusion: From reading to ownership

We've covered all the layers: core values → collections → time and identity → actions and async → shape/equivalency/snapshots → reflection and architecture → the outside world → the engine room → the ecosystem.

The next step isn't another chapter—it's **practical ownership**. Take a real module you own, write tests using what you've learned here, and read the failure messages. Do they tell you **what was checked**, **what was found**, and **why it was expected**?

When you encounter a message that doesn't teach you anything, you'll know exactly where to improve it: the message template, a translation, or your assertion itself. That's the moment this book stops being a reference and becomes part of how you think when you write tests.

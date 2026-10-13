# Chapter 10 — The Ecosystem: MSTest Adapter, Analyzer, Consumer Template, and Migration

The library alone isn't enough. Teams need practical answers to three questions: How do we integrate with our test framework's reporting? How do we convert thousands of legacy assertion calls? Where does a new developer start?

This chapter covers the surrounding pieces: a deliberate MSTest adapter, a Roslyn analyzer that only suggests safe conversions, a copyable `Consumer` template, and a nearly-mechanical migration map from other assertion libraries.

The common thread is **explicit boundaries, no hidden magic**.

## 10.1 MSTest adapter (optional)

The core package throws its own `AssertionFailedException`. Some MSTest reporters classify it as an **error** rather than a **failure**, which loses an important distinction in test reports.

The optional `Jeninnet.Assertions.MSTest` package (`net6.0+`) solves this explicitly:

```csharp
using Jeninnet.Assertions.MSTest;

MSTestAssertions.That(() => order.Should().Be("shipped"));
await MSTestAssertions.ThatAsync(() => action.Should().NotThrowAsync());
```

This wraps the assertions so failures surface as MSTest's `AssertFailedException` while preserving the message and inner exception. Non-failure exceptions propagate untouched.

**Why explicit?** Because the core package must stay framework-agnostic, and a silent integration would contradict that. Wrapping at each call site makes the choice visible in the code that depends on it, and it gives future adapters — for xUnit or NUnit, should they exist — one pattern to follow instead of several.

## 10.2 Roslyn analyzer (optional)

The `Jeninnet.Assertions.Analyzer` package provides Roslyn analyzers with code fixes to convert eligible MSTest, xUnit, and NUnit assertion calls. Diagnostics are reported at **Info** severity so they never break builds with `TreatWarningsAsErrors` enabled.

| ID | Converts |
|---|---|
| `JNA1001` | `Assert.AreEqual` / `AreNotEqual`, xUnit `Assert.Equal` / `NotEqual` / `Contains`, NUnit `Is.EqualTo` / `Is.Not.EqualTo` → `Should().Be(...)` / `NotBe(...)` / `Contain(...)` |
| `JNA1002` | `Assert.IsTrue` / `IsFalse` / `IsNull` / `IsNotNull`, xUnit `Assert.True` / `False` / `Null` / `NotNull`, NUnit `Is.True` / `False` / `Null` / `Is.Not.Null` → `BeTrue()` / `BeFalse()` / `BeNull()` / `NotBeNull()` |

The `JNA` prefix stands for Jeninnet Assertions.

**Safety guarantees:**

- **Compile-safe suggestions:** Fixes only appear when the generated code is proven to compile via speculative binding.
- **Conservative by design:** Overloads involving tolerance, custom comparers, or culture are never converted, since meaning can change subtly.
- **Smart message mapping:** `message:` parameters are automatically converted to `because:` while preserving format arguments.

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

The library supports `netstandard2.0;net6.0;net8.0;net10.0`. New tests are best written for `net10.0` (like this repository's tests, which use MSTest). You can use it with any of the supported targets.

**Is this library meant for production code?**

No. It's for tests only. Its exceptions are diagnostic tools, not control flow mechanisms. Using it in production is not the intended use case.

**What if a failure message isn't clear?**

Treat unclear messages as a library issue, not a test issue. Failure messages are covered by precise string tests; improving them means updating message templates, translations (including Arabic), and the corresponding tests together. The goal is always that a failure message explains itself without opening the code.

## 10.6 Conclusion: From reading to ownership

We've covered all the layers: core values → collections → time and identity → actions and async → shape/equivalency/snapshots → reflection and architecture → the outside world → the engine room → the ecosystem.

The next step isn't another chapter—it's **practical ownership**. Take a real module you own, write tests using what you've learned here, and read the failure messages. Do they tell you **what was checked**, **what was found**, and **why it was expected**?

When you encounter a message that doesn't teach you anything, you'll know exactly where to improve it: the message template, a translation, or your assertion itself. That's the moment this book stops being a reference and becomes part of how you think when you write tests.

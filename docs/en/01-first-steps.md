# Chapter 1 — First Steps: Installation, Your First Assertion, and Reading Failures

Most assertion libraries give you a terse comparison when something fails. A message like `Assert.AreEqual failed. Expected:<x>. Actual:<y>` gives the two values, but loses the context: which variable, in which test, and most importantly, *why* was that value expected in the first place?

A good failure message should answer those questions before you have to ask them. This chapter is about writing assertions that read like clear statements and fail with messages that explain themselves.

## 1.1 Installation: One package, no dependencies

Add the package to your test project (or any project where you want to write assertions):

```shell
dotnet add package Jeninnet.Assertions
```

This is a single MIT-licensed package with zero test-framework dependencies. It works with MSTest, xUnit, NUnit, or even a plain console application. It multi-targets `netstandard2.0;net6.0;net8.0;net10.0` (HTTP assertions require `net6.0+` only).

The lack of framework coupling is intentional. The library depends only on the BCL (with `System.Text.Json` and `System.Xml.XDocument` for `netstandard2.0`), so it won't pull test framework dependencies into your projects.

## 1.2 Your first assertion

Start by adding the namespace:

```csharp
using Jeninnet.Assertions;
```

Then write your first assertion:

```csharp
string username = "jonas";
username.Should().Be("jonas");
```

The `Should()` extension wraps the value in an assertion object. `Be()` performs the actual check. If it fails, you'll see something like this:

```text
Expected username to be "ada", but found "jonas".
```

The most important detail here is that the message names the expression you wrote: `username`. The library captures the expression automatically at the call site—no hard-coded string names to go stale when you rename variables.

All values are formatted using an invariant culture (`CultureInfo.InvariantCulture`). As a result, failure messages are byte-for-byte identical across machines and cultures, making them safe to compare textually in tests themselves. We'll explore this further in Chapter 9.

## 1.3 Chaining multiple checks with `.And`

It's often useful to verify several things at once. Instead of writing separate assertions, chain them with `.And`:

```csharp
"hello".Should().Contain("ell").And.Be("hello");
order.Total.Should().BePositive().And.BeLessThan(1000);
```

Each assertion returns a constraint that allows `.And`, forming a natural, readable sentence.

The general rule is straightforward:

- **Inside an assertion scope:** all failures are collected and thrown together as one comprehensive report.
- **Outside an assertion scope:** the first failure is thrown immediately.

Independent checks are best chained together. Dependent checks—where the second check only makes sense if the first passes—are better kept as separate, sequential assertions. We'll revisit assertion scopes (`AssertionScope`) in Chapter 9.

## 1.4 Explaining intent with `because`

Not every expectation is obvious six months later. The `because` clause explains *why* the expectation holds, not what it asserts:

```csharp
username.Should().Be("jonas", "the user was renamed during migration {0}", ticketId);
```

If it fails:

```text
Expected username to be "jonas", but found "ada", because the user was renamed during migration 42.
```

Every assertion accepts an optional `because` string with format arguments. As a rule of thumb, add `because` to any non-obvious expectation. The reader of that failure message months from now—likely you—will need the reason far more than the two values.

## 1.5 Nullable types: the type chooses its assertion class

The library selects a different assertion class depending on whether a type can be null. This prevents nonsensical calls and gives you the right assertions for the job:

```csharp
string? nickname = null;
nickname.Should().BeNull();

nickname = "jo";
nickname.Should().NotBeNull().And.Be("jo");
```

Since `nickname` is `string?`, `BeNull()` and `NotBeNull()` are available alongside value assertions like `Be()` and `HaveLength()`. For non-nullable types like `string` or `int`, `BeNull()` isn't available at all—catching logical errors at compile time.

This pattern applies to `bool?`, `int?`, `DateTime?`, `Guid?`, nullable enums, and `TimeSpan?`.

## 1.6 Recap and exercise

**Recap:**

- Every assertion starts with `Should()`, and the library captures the expression name to produce clear failure messages.
- Values are formatted invariantly, making messages stable and text-comparable.
- Chain related checks with `.And`. Inside a scope, failures are collected; outside, the first failure throws immediately.
- Use `because` to explain the reasoning behind an expectation, not the assertion itself.
- Nullable types get null-aware assertion classes with `BeNull()`/`NotBeNull()` only when they make sense.

**Exercise:**

1. Create a test with three variables.
2. Write a single assertion that chains at least two checks using `.And`.
3. Add a clear `because` clause that explains the intent.
4. Make it fail temporarily and read the full failure message. Then fix it.

The quality of your failure messages is a feature of your tests—treat it as such.

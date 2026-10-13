# Chapter 11 — Validators: Assertions Without Throwing

Assertions throw by design. That is exactly what a test needs, and exactly what a long-running service does not. This chapter turns the same fluent verbs into a validation engine for production code — no second API, no duplicated logic.

## 11.1 The problem

Consider a payment endpoint that must reject a malformed request:

```csharp
if (order.Total <= 0) { return BadRequest("total must be positive"); }
if (order.Customer is null) { return BadRequest("customer is required"); }
if (order.Items.Count == 0) { return BadRequest("items cannot be empty"); }
```

Three handwritten `if`s, three hand-written messages, zero consistency with the failure messages the rest of the team already reads in tests. The assertion library already has the verbs — `BeGreaterThan(0)`, `NotBeNull()`, `NotBeEmpty()` — and already produces precisely worded failures. It just throws.

## 11.2 Collecting a result

`AssertionResult` runs a block of ordinary assertions and returns a result instead of throwing:

```csharp
var result = AssertionResult.Create(() => {
    order.Total.Should().BeGreaterThan(0);
    order.Customer.Should().NotBeNull();
    order.Items.Should().NotBeEmpty();
});

if (!result.IsSuccess) {
    return BadRequest(result.ToReportJson());
}
```

Every assertion in the block is still evaluated; failures are collected rather than aborting at the first one. `IsSuccess` is `true` only when the block produced no failures. The verbs inside are the exact same `Should()` verbs you use in tests — there is no parallel validator API to learn.

Async flows get the matching `CreateAsync`:

```csharp
var result = await AssertionResult.CreateAsync(async () => {
    var quote = await pricing.QuoteAsync(order);
    quote.Total.Should().BeGreaterThan(0);
    quote.Currency.Should().Be("USD");
});
```

## 11.3 Inspecting and reporting

Every collected failure is a structured `AssertionFailure`:

```csharp
foreach (var failure in result.Failures) {
    Console.WriteLine($"{failure.Expression} ({failure.Category}): {failure.Message}");
}
```

Each failure carries the subject `expression`, the `category` (the concrete assertion class that reported it), the formatted `message`, and — where the verb supplies them — `actual` and `expected` values.

Two arrows point back from a validator result into the rest of the library:

- `result.ToReportJson()` serializes the failures (`count` plus a `failures` array) for logs, dashboards, or CI attachments. The JSON shape matches `SoftAssertions.ToReportJson()`.
- `result.ThrowIfFailed()` rethrows the collected failures as one `AssertionFailedException`, so a validator can also double as a test assertion — same aggregate report as a scope.

## 11.4 Graph verdicts without throwing

When the check is one object-graph comparison, `EquivalencyEvaluation.Evaluate` returns the verdict directly — same engine and difference texts as `BeEquivalentTo`, never a thrown failure:

```csharp
var verdict = EquivalencyEvaluation.Evaluate(actualResponse, expectedResponse, o => o
    .WithDateTimeTolerance(TimeSpan.FromSeconds(2)));

if(!verdict.IsEquivalent) {
    logger.Warn(verdict.Message);                          // unified, invariantly formatted text
    attachments.Add("diff.json", verdict.ToReportJson());  // path, message, expected, actual per difference
}
```

Each `EquivalencyDifference` carries the formatted `Path` (`Address.City`, `Tags[0]`, or `<root>`), the identical `Message` the assertion would print, and the formatted `Expected`/`Actual` where the difference site reports them (missing members and cyclic references are path-only). Difference order follows the comparison and is part of the contract because CI reads the JSON.

## 11.5 What still throws

Exactly one family of verbs cannot be collected: assertions that must **return a value** to be useful — `Throw*` (they return an `ExceptionAssertions` for further chaining) and collection verbs like `ContainSingle` (they return the found element). They throw immediately inside a validator block and **abort the rest of the block**, just as they do inside a scope. Keep value-returning verbs out of validator blocks, or guard them before the validation block runs.

Everything else — `Be*`, `Have*`, `Contain*`, ordering, equivalency, string matching, and so on — collects cleanly.

## 11.6 Where it fits

Three collect-without-throwing layers now exist, each with a different contract:

- **`AssertionScope`** batches on the way *out*: failures surface when the scope disposes, as one exception. Best for tests that want the full picture in a single failure.
- **`SoftAssertions`** collects on *demand*: `Collect()`, run assertions, then `ThenReport()` when you choose. No mandatory `using`.
- **`AssertionResult`** never throws: it is the production-facing endpoint. The default path is untouched — outside a validator or scope, the first failure still throws immediately.

## 11.7 Recap and exercise

**Recap:**

- **`AssertionResult.Create(...)` and `CreateAsync(...)`** run the same `Should()` verbs without throwing and return a structured result.
- **`IsSuccess` and `Failures`** inspect the outcome; **`ToReportJson()`** serializes it; **`ThrowIfFailed()`** converts it back into one aggregate assertion failure.
- **Value-returning verbs still throw** and abort the block; keep them out of validator blocks.

**Exercise:**

1. Write a validator that checks three independent properties of an object and make two fail — confirm both failures are in the result, in order.
2. Serialize the result with `ToReportJson()` and validate the output by parsing it back.
3. Hoist the same block into an `AssertionResult.Create`, then turn it into a test assertion with `ThrowIfFailed()` and observe the aggregate message.
# Chapter 5 — Actions and Async: Exceptions, Waiting, and Budgets

The quickest wrong fix in tests is `Thread.Sleep(2000)`. We reach for it when a result doesn't appear immediately—like a balance updating after a few hundred milliseconds or an event firing asynchronously. The problem is that sleeping **guesses at time**, not **waits for a condition**. Tests become slower for no reason, failures become less diagnostic, and the failure message tells you nothing about what actually happened while you were "waiting".

This chapter tackles three places where people default to guessing: exceptions (treat them as values), deferred results (wait for the condition, not the clock), and budgets (use generous tripwires, not tight performance thresholds).

## 5.1 Exceptions: Type first, content second

When a method is expected to throw, wrap the call in a delegate and interrogate what was thrown:

```csharp
Action act = () => cart.Checkout();
act.Should().Throw<InvalidOperationException>();
act.Should().ThrowExactly<InvalidOperationException>();
act.Should().NotThrow();
```

The distinction matters: `Throw<T>()` accepts the exception type or any derived type. `ThrowExactly<T>()` requires the **exact runtime type** only. Use `ThrowExactly<T>()` when downstream `catch` blocks distinguish by the exact type.

Once you've asserted the type, chain into content checks via the returned constraint:

```csharp
act.Should().Throw<ArgumentException>()
    .WithMessage("Quantity *")
    .WithoutMessage("*success*")
    .WithParameterName("qty")
    .WithInnerException<FormatException>()
    .WithInnerExceptionExactly(typeof(FormatException))
    .Where(static ex => ex.ParamName == "qty");
```

`WithMessage()` supports `*` wildcards, letting you assert the essence of the message without over-specifying exact wording. This keeps tests resilient while still catching regressions. `WithoutMessage()` asserts the opposite: the message must not match. `WithInnerException<T>()` accepts derived types, while `WithInnerExceptionExactly<T>()` requires the exact runtime type. The caught exception is also available as `.Which` for continued assertions.

Single-statement tests can use `FluentActions`, including deferred iterators:

```csharp
FluentActions.Invoking(() => cart.Checkout(null)).Should().Throw<ArgumentNullException>();
await FluentActions.Awaiting(() => gateway.ChargeAsync(order)).Should().ThrowAsync<PaymentException>();

Func<IEnumerable<int>> rows = () => report.Rows(account);
FluentActions.Enumerating(rows).Should().Throw<InvalidOperationException>();
```

`Enumerating()` forces full iteration when the exception only appears while consuming a `yield` sequence. Deferred iterators may support only one enumeration, so invoke the returned action once. A checked cast is available as `value.As<string>()`; unlike a direct cast, a failed cast produces an assertion failure instead of an `InvalidCastException`.

The same API works for delegates returning values (`Func<T>`), and they can continue asserting if nothing is thrown.

## 5.2 Asynchronous operations

For every synchronous exception assertion, there's an async twin:

```csharp
Func<Task> call = () => gateway.ChargeAsync(order);
await call.Should().ThrowAsync<PaymentException>();
await call.Should().ThrowExactlyAsync<PaymentException>();
await call.Should().NotThrowAsync();
```

An important rule: **cancellation is a result that needs interpretation, not success to be ignored.** A canceled task is not "successfully completing without throwing"—it's a cancellation outcome. By design, `NotThrowAsync()` fails if the task is canceled, expressing the behavior clearly.

When an operation should throw soon rather than eventually succeed, use a time budget instead of polling:

```csharp
await call.Should().ThrowWithinAsync(TimeSpan.FromSeconds(5));
await call.Should().ThrowWithinAsync<PaymentException>(TimeSpan.FromSeconds(5));
```

`ThrowWithinAsync()` invokes the delegate once and waits only for that invocation to settle. A different exception, quiet completion, or an exceeded budget all fail with the corresponding diagnostic.

`Task<T>`-returning delegates support the same assertions and can be combined with the polling assertions below.

Already-started tasks and task completion sources use completion assertions rather than delegate assertions:

```csharp
await task.Should().CompleteWithinAsync(TimeSpan.FromSeconds(5)).WithResultAsync(42);
await source.Should().CompleteWithinAsync(TimeSpan.FromSeconds(5)).WithResultAsync(42);
await pending.Should().NotCompleteWithinAsync(TimeSpan.FromMilliseconds(50));
```

Completion means the task settled in any terminal state: a faulted or canceled task still counts as complete for timing, then fails with the original exception preserved. `WithResultAsync()` continues from a completed value without re-awaiting the operation.

### Honoring cancellation

A delegate that accepts a `CancellationToken` makes a promise: when the token is already canceled, stop before doing the work. Assert that promise directly — the verb hands over a pre-canceled token and requires `OperationCanceledException` carrying that same token, with no wall-clock waiting:

```csharp
Func<CancellationToken, Task> shipment = token => gateway.ShipAsync(order, token);
await shipment.Should().HonorCancellationAsync();

Action<CancellationToken> load = token => cache.Load(token);
load.Should().HonorCancellationAsync();
```

Use `CompleteDespiteCancellationAsync()` for the inverse contract — critical sections and cleanup that must finish even when cancellation is requested:

```csharp
Func<CancellationToken, Task> flush = token => audit.FlushAsync(token);
await flush.Should().CompleteDespiteCancellationAsync();
```

A delegate that returns quietly fails with "did not honor cancellation"; one that throws a different exception (or an `OperationCanceledException` carrying an unrelated token) fails naming the offender. These verbs check the throwing contract only — they do not judge how promptly cancellation was noticed.

### Async streams

`IAsyncEnumerable<T>` sequences assert like collections, but asynchronously and cancellation-aware — every verb takes a token (handed to the source's enumerator), and failures name the index where a deviation appeared:

```csharp
await stream.Should().BeEmptyAsync();
await stream.Should().NotBeEmptyAsync();
await stream.Should().HaveCountAsync(3);
await stream.Should().ContainAsync(item);
await stream.Should().NotContainAsync(item);                 // failure names the index
await stream.Should().ContainInOrderAsync([first, third]);   // gaps allowed; failure names the search window
await stream.Should().BeEquivalentToAsync([a, b, a]);        // multiset: order ignored, duplicates counted
var items = (await stream.Should().CompleteAsync()).Which;   // drains once and exposes the items
```

A token already canceled before the assertion throws `OperationCanceledException` immediately — cancellation is never swallowed into a failure. Count and equivalence checks drain the sequence, so infinite sources are out of scope (deliberate).

### Thread-safety under load

Concurrency bugs hide behind the first exception. `BeThreadSafeAsync()` starts the action simultaneously on several thread-pool copies behind one barrier and reports one aggregated failure — grouped by exception type and message with the failing copy numbers — or names how many copies never finished:

```csharp
Action refresh = () => cache.Refresh();
await refresh.Should().BeThreadSafeAsync(copies: 8, timeout: TimeSpan.FromSeconds(5));
```

Copies that do not complete within the timeout are reported as a possible deadlock; note that their threads cannot be interrupted, so a deadlocked action keeps them until the process exits. This is a diagnostic stress check, not a proof of thread safety.

## 5.3 Polling, not sleeping: `SucceedWithin()` and `EventuallySatisfy()`

"Not yet" isn't an error—it's a timing condition. The correct approach is **polling**: check the condition repeatedly until it becomes true or the timeout elapses.

```csharp
Action sync = () => stock.Reserve(line);
sync.Should().SucceedWithin(TimeSpan.FromSeconds(5));
```

`SucceedWithin()` asserts the action **succeeds before the timeout expires**. If it fails, the message includes the **last exception** and the state at timeout—giving you useful diagnostics instead of just "timed out".

`NotThrowAfter()` exists only for compatibility with polling-style APIs that expect quiet behavior. It retries until the action stops throwing, but it cannot distinguish a transient failure from a permanently broken action as clearly as `SucceedWithin()` does. Prefer `SucceedWithin()`; use `NotThrowAfter()` when matching an external polling contract.

```csharp
sync.Should().NotThrowAfter(TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(100));
await call.Should().NotThrowAfterAsync(TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(100));
```

For values that change over time:

```csharp
Func<int> read = () => ledger.Balance(account);
read.Should().EventuallySatisfy(static b => b == 100, TimeSpan.FromSeconds(5));
```

For async operations:

```csharp
Func<Task<int>> readAsync = () => ledger.BalanceAsync(account);
await readAsync.Should().EventuallySatisfyAsync(
    static b => b == 100,
    TimeSpan.FromSeconds(5),
    cancellationToken);
```

The key difference from `Thread.Sleep`: we're waiting for the **condition**, not the clock. If the condition is met after 200ms, the test passes immediately and saves time. If it never is, we get the last observed value or error to help diagnose why.

### Curved backoff with `PollingBackoff`

By default the polling assertions retry at a constant interval. For long waits, pass a `PollingBackoff` so the pauses grow instead of hammering the subject:

```csharp
var backoff = PollingBackoff.Exponential(
    TimeSpan.FromMilliseconds(50),
    factor: 2.0,
    maxInterval: TimeSpan.FromSeconds(1));

read.Should().EventuallySatisfy(static b => b == 100, backoff, TimeSpan.FromSeconds(30));
sync.Should().SucceedWithin(backoff, TimeSpan.FromSeconds(30));
```

`PollingBackoff.Constant(interval)` reproduces the default fixed interval; `PollingBackoff.Exponential` starts fast and multiplies the pause by `factor` after every attempt, capped at `maxInterval`. The delay is always clamped to the remaining timeout, and the last observed value and error are still reported on failure.

## 5.4 Budgets: tripwires, not benchmarks

Allocation and timing assertions here are meant to catch **sudden regressions** (regression smoke tests), not to measure precise performance.

```csharp
Action price = () => catalog.PriceAll(lines);
price.ExecutionTime().Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));

Action hot = () => pricing.Total(lines);
hot.Should().AllocateAtMost(64.Kilobytes());
hot.Should().AllocateLessThan(1.Megabytes());
await hot.Should().AllocateAtMostAsync(64.Kilobytes());
```

`ExecutionTime()` measures a single execution and returns an `ExecutionTime` value you can assert on directly, including closeness with a tolerance:

```csharp
price.ExecutionTime().Should().BeLessThan(TimeSpan.FromSeconds(1));
price.ExecutionTime().Should().BeCloseTo(TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(50));
```

Overloads also accept an async delegate (`ExecutionTime(async () => await work())`) and a member expression (`ExecutionTimeOf(obj, x => x.Method())`) to measure one invocation with a readable subject name. Allocation assertions measure current-thread bytes.

**Golden rule:** use **very generous ceilings** that only catch serious regressions. True performance is measured with dedicated benchmarks on controlled hardware—not in unit tests running in varied CI environments.

Note: allocation tracking is not available on `netstandard2.0` (the methods exist, but that runtime cannot observe per-thread allocations). Keep allocation tests on modern TFMs (`net6.0+`).

## 5.5 Recap and exercise

**Recap:**

- **Treat exceptions as values.** Assert type first (`Throw` vs `ThrowExactly`), then content (`WithMessage`, `WithParameterName`, `WithInnerException`).
- **Cancellation is meaningful.** `NotThrowAsync()` fails on cancellation because cancellation needs interpretation.
- **Wait for the condition, not the clock.** Use `SucceedWithin()` and `EventuallySatisfy()` instead of `Thread.Sleep`—faster on average and more diagnostic on failure.
- **Budgets are tripwires.** Set generous ceilings to catch regressions, not tight thresholds for benchmarking.

**Exercise:**

1. Write a test expecting `ArgumentException` with parameter `qty` and inner `FormatException`. Assert all three together.
2. Take a test that uses `Thread.Sleep()` and convert it to use `EventuallySatisfy()`.
3. Intentionally add an absurdly tight allocation assertion (`AllocateAtMost(1.Byte())`) and observe the failure, then revert to a reasonable value.

# Chapter 4 — Time and Identity: Dates, `Guid`, and Enums

A common source of brittle tests is time. A delivery SLA states "same day", but the system records timestamps down to milliseconds. If we compare instants with exact equality, the test fails over a few minutes that don't matter to the business rule.

The fix is simple: **match the precision of the assertion to the precision of the rule.** Is the rule about the calendar day? Compare dates only. Is it about "close enough"? Use tolerance. Is it about a duration? Compare the duration.

## 4.1 Moments: Ordering and closeness

For instants (`DateTime`, `DateTimeOffset`), use clear ordering assertions:

```csharp
eta.Should().BeBefore(deadline);
eta.Should().BeOnOrBefore(deadline);
eta.Should().BeAfter(shipped);
eta.Should().BeOnOrAfter(shipped);
eta.Should().NotBeBefore(shipped);
eta.Should().NotBeAfter(deadline);
```

The inclusive forms (`BeOnOrBefore`/`BeOnOrAfter`) are useful when the boundary moment is allowed. For scheduled-versus-actual comparisons, exact equality is almost always too brittle. Use `BeCloseTo()` instead:

```csharp
eta.Should().BeCloseTo(actual, TimeSpan.FromMinutes(5));
eta.Should().NotBeCloseTo(stale, TimeSpan.FromHours(1));
```

This isn't relaxing correctness—it's being precise about what "close enough" actually means. If five minutes of tolerance is acceptable, write five minutes explicitly.

## 4.2 Calendar days: `BeSameDateAs()`

When the rule is "same day delivery", the time component is noise. 08:00 and 23:59 on the same calendar day are equivalent to the business rule but completely different under exact `Be()` comparison.

`BeSameDateAs()` compares the date part only, ignoring the time:

```csharp
deliveredAt.Should().BeSameDateAs(promisedDay);
deliveredAt.Should().NotBeSameDateAs(holiday);
```

This works for `DateTime`, `DateTimeOffset`, and `DateOnly`. Nullable variants follow the same nullability rules from Chapter 1.

## 4.3 Components and breakdown

Sometimes you only care about part of a value. Assert components directly:

```csharp
eta.Should().HaveYear(2026).And.HaveMonth(5).And.HaveDay(10);
eta.Should().HaveHour(8).And.HaveMinute(30).And.HaveSecond(0);
```

For `TimeOnly` and `TimeSpan`:

```csharp
window.Should().BePositive().And.BeLessThan(TimeSpan.FromHours(24));
window.Should().HaveHours(2).And.HaveMinutes(30);
```

**Hard rule:** never use `DateTime.Now` directly in assertions. The current time is non-deterministic. Instead, inject a time source (even a `Func<DateTime>`), freeze it in your test, and assert against that controlled value. Deterministic tests don't depend on the wall clock.

### Fluent dates and durations

Readable literals beat constructor noise when the exact value is the point:

```csharp
1.March(2010).At(22, 15).Should().Be(new DateTime(2010, 3, 1, 22, 15, 0));
2.Hours().Before(deadline).Be(deadline.AddHours(-2));
3.Seconds().Should().Be(TimeSpan.FromSeconds(3));
100.Nanoseconds().Should().Be(TimeSpan.FromTicks(1));
```

Available builders: month factories (`1.March(2010)`), `At(hour, minute[, second])`, `AsUtc()`/`AsLocal()` (kind labels only), `WithOffset()`, `AddMicroseconds()`; durations (`Seconds()`, `Minutes()`, `Hours()`, `Milliseconds()`, `Microseconds()`, `Nanoseconds()`, `Ticks()`), totals (`TotalMicroseconds()`, `TotalNanoseconds()`), and `Before()`/`After()` date assertions. A tick is 100 nanoseconds, so sub-tick remainders in `Nanoseconds()` are truncated. `AsUtc()`/`AsLocal()` only label the kind — never assert on conversions through the machine time zone.

## 4.4 Identity: `Guid` and enums

### Guids

`Guid.Empty` is commonly used to mean "not assigned yet". It's important to distinguish explicitly between "empty" and "assigned":

```csharp
orderId.Should().NotBeEmpty();
orderId.Should().Be(expected);
orderId.Should().NotBe(Guid.Empty);
```

This helps catch forgotten initialization paths early. Guids also compare from text (invalid text throws instead of failing silently):

```csharp
orderId.Should().Be("11111111-1111-1111-1111-111111111111");
orderId.Should().NotBe("00000000-0000-0000-0000-000000000000");
```

Nullable values use `HaveValue()` / `NotHaveValue()` instead of null checks, which reads better in a nullable context:

```csharp
eta.Should().HaveValue();
missing.Should().NotHaveValue();
```

### Enums

A cast like `(OrderStatus)42` produces an integer disguised as an enum value. `BeDefined()` rejects undefined enum values:

```csharp
status.Should().Be(OrderStatus.Shipped);
status.Should().NotBe(OrderStatus.Cancelled);
status.Should().BeDefined();
```

### Flags enums

For `[Flags]` enums, use the dedicated flag assertions instead of manual bit manipulation:

```csharp
perms.Should().HaveFlag(Permission.Read);
perms.Should().NotHaveFlag(Permission.Admin);
```

These read naturally and produce clear failure messages.

## 4.5 Recap and exercise

**Recap:**

- **Match precision to the rule.** Instants: ordering and closeness. Calendar days: `BeSameDateAs()`. Durations: components.
- **Time is non-deterministic.** Never assert against `DateTime.Now` directly—instead inject and freeze it in tests.
- **Treat special values explicitly.** Check for `Guid.Empty` and undefined enum values when they matter to your domain.
- **Use the right tools.** `HaveFlag()` for flag enums, `BeSameDateAs()` for calendar-day comparisons.

**Exercise:**

1. Write an assertion ensuring the delivery date is the **same calendar day** as the promised date (ignoring time).
2. Write an assertion ensuring the actual arrival is within **30 minutes** of the estimated arrival using `BeCloseTo()`.
3. Assert that `orderId` is **not empty** and not `Guid.Empty`.
4. Assert that the order status is a **defined** enum value.

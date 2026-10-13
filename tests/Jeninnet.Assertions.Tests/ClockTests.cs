//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class ClockTests {
    private static readonly DateTimeOffset _epoch = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    [TestCleanup]
    public void Reset() => AssertionClock.Current = SystemClock.Instance;

    [TestMethod]
    public void Default_Current_IsSystemClock() {
        Assert.IsInstanceOfType<SystemClock>(AssertionClock.Current);
        var drift = (AssertionClock.Current.UtcNow - DateTimeOffset.UtcNow).Duration();
        Assert.IsLessThan(TimeSpan.FromMinutes(1), drift);
    }

    [TestMethod]
    public void Current_Null_ThrowsArgumentNull() =>
        Assert.ThrowsExactly<ArgumentNullException>(() => AssertionClock.Current = null!);

    [TestMethod]
    public void DateTime_UtcKind_UsesUtcNow() {
        AssertionClock.Current = new ManualClock(_epoch, TimeSpan.FromHours(12));

        var value = AssertionClock.Current.UtcNow.UtcDateTime;
        value.Should().BeWithin(TimeSpan.FromSeconds(1));
        value.Should().BeLessThan(TimeSpan.FromHours(1));
    }

    [TestMethod]
    public void DateTime_LocalKind_UsesNow() {
        AssertionClock.Current = new ManualClock(_epoch, TimeSpan.FromHours(12));

        var value = AssertionClock.Current.Now.LocalDateTime;
        value.Should().BeWithin(TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void DateTime_BeWithin_BoundaryIsDeterministic() {
        AssertionClock.Current = new ManualClock(_epoch, TimeSpan.Zero);
        var value = new DateTime(2026, 1, 1, 0, 5, 0, DateTimeKind.Utc);

        value.Should().BeWithin(TimeSpan.FromMinutes(5));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => value.Should().BeWithin(TimeSpan.FromMinutes(3)));
        Assert.AreEqual("Expected value to be within 00:03:00 of the current moment, but found 01/01/2026 00:05:00.", ex.Message);
    }

    [TestMethod]
    public void DateTime_LessThanMoreThanAtLeast_Boundaries() {
        AssertionClock.Current = new ManualClock(_epoch, TimeSpan.Zero);
        var value = new DateTime(2026, 1, 1, 0, 10, 0, DateTimeKind.Utc);

        value.Should().BeAtLeast(TimeSpan.FromMinutes(10));

        var less = Assert.ThrowsExactly<AssertionFailedException>(() => value.Should().BeLessThan(TimeSpan.FromMinutes(10)));
        Assert.Contains("to be less than 00:10:00 from the current moment", less.Message);

        var more = Assert.ThrowsExactly<AssertionFailedException>(() => value.Should().BeMoreThan(TimeSpan.FromMinutes(10)));
        Assert.Contains("to be more than 00:10:00 from the current moment", more.Message);
    }

    [TestMethod]
    public void DateTimeOffset_BeWithin_UsesInjectedClock() {
        AssertionClock.Current = new ManualClock(_epoch, TimeSpan.FromHours(2));
        var value = new DateTimeOffset(2026, 1, 1, 2, 5, 0, TimeSpan.FromHours(2));

        value.Should().BeWithin(TimeSpan.FromMinutes(5));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => value.Should().BeWithin(TimeSpan.FromMinutes(3)));
        Assert.AreEqual("Expected value to be within 00:03:00 of the current moment, but found 01/01/2026 02:05:00 +02:00.", ex.Message);
    }

    [TestMethod]
    public void NullableDateTime_BeWithin_UsesInjectedClock() {
        AssertionClock.Current = new ManualClock(_epoch, TimeSpan.Zero);
        DateTime? value = new DateTime(2026, 1, 1, 0, 5, 0, DateTimeKind.Utc);

        value.Should().BeWithin(TimeSpan.FromMinutes(5));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => value.Should().BeWithin(TimeSpan.FromMinutes(3)));
        Assert.Contains("of the current moment, but found 01/01/2026 00:05:00", ex.Message);
    }

    [TestMethod]
    public void NullableDateTimeOffset_BeWithin_UsesInjectedClock() {
        AssertionClock.Current = new ManualClock(_epoch, TimeSpan.FromHours(2));
        DateTimeOffset? value = new DateTimeOffset(2026, 1, 1, 2, 5, 0, TimeSpan.FromHours(2));

        value.Should().BeWithin(TimeSpan.FromMinutes(5));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => value.Should().BeWithin(TimeSpan.FromMinutes(3)));
        Assert.Contains("of the current moment, but found 01/01/2026 02:05:00 +02:00", ex.Message);
    }

    [TestMethod]
    public void Advance_MovesTheWindow_WithoutSleeping() {
        var clock = new ManualClock(_epoch, TimeSpan.Zero);
        AssertionClock.Current = clock;
        var value = new DateTime(2026, 1, 1, 0, 5, 0, DateTimeKind.Utc);

        Assert.ThrowsExactly<AssertionFailedException>(() => value.Should().BeWithin(TimeSpan.FromMinutes(4)));

        clock.Advance(TimeSpan.FromMinutes(2));
        value.Should().BeWithin(TimeSpan.FromMinutes(4));
    }

    [TestMethod]
    public async Task Current_IsIsolatedPerAsyncFlow() {
        var parent = new ManualClock(_epoch, TimeSpan.Zero);
        AssertionClock.Current = parent;

        var inherited = await Task.Run(() => AssertionClock.Current);
        var replaced = await Task.Run(() => {
            AssertionClock.Current = new ManualClock(_epoch, TimeSpan.FromHours(5));
            return AssertionClock.Current;
        });

        Assert.AreSame(parent, inherited);
        Assert.AreNotSame(parent, replaced);
        Assert.AreSame(parent, AssertionClock.Current);
    }
}

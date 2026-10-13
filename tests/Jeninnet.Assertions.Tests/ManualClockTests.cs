//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class ManualClockTests {
    private static readonly DateTimeOffset _epoch = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    [TestCleanup]
    public void Reset() => AssertionClock.Current = SystemClock.Instance;

    [TestMethod]
    public void Advance_MovesForward_KeepingTheConfiguredOffset() {
        var clock = new ManualClock(_epoch, TimeSpan.FromHours(2));

        clock.Advance(TimeSpan.FromMinutes(5));

        Assert.AreEqual(new DateTimeOffset(2026, 1, 1, 2, 5, 0, TimeSpan.FromHours(2)), clock.Now);
        Assert.AreEqual(_epoch.AddMinutes(5), clock.UtcNow);
    }

    [TestMethod]
    public void Advance_NegativeDelta_MovesBackwards() {
        var clock = new ManualClock(_epoch);

        clock.Advance(TimeSpan.FromMinutes(-3));

        Assert.AreEqual(_epoch.AddMinutes(-3), clock.UtcNow);
        Assert.AreEqual(_epoch.AddMinutes(-3), clock.Now);
    }

    [TestMethod]
    public void Set_ReplacesTheInstant_KeepingTheConfiguredOffset() {
        var clock = new ManualClock(_epoch, TimeSpan.FromHours(2));

        clock.Set(new DateTimeOffset(2030, 6, 1, 12, 0, 0, TimeSpan.Zero));

        Assert.AreEqual(new DateTimeOffset(2030, 6, 1, 14, 0, 0, TimeSpan.FromHours(2)), clock.Now);
        Assert.AreEqual(new DateTimeOffset(2030, 6, 1, 12, 0, 0, TimeSpan.Zero), clock.UtcNow);
    }

    [TestMethod]
    public void Constructor_WithoutExplicitOffset_UsesTheValueOffsetForLocalTime() {
        var clock = new ManualClock(new DateTimeOffset(2026, 1, 1, 2, 0, 0, TimeSpan.FromHours(2)));

        Assert.AreEqual(TimeSpan.FromHours(2), clock.Now.Offset);
        Assert.AreEqual(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero), clock.UtcNow);
    }

    [TestMethod]
    public void UtcNow_IsAlwaysUtc_EvenWhenConstructedWithAnOffset() {
        var clock = new ManualClock(new DateTimeOffset(2026, 1, 1, 2, 0, 0, TimeSpan.FromHours(5)));

        Assert.AreEqual(TimeSpan.Zero, clock.UtcNow.Offset);
        Assert.AreEqual(TimeSpan.FromHours(5), clock.Now.Offset);
        Assert.AreEqual(new DateTimeOffset(2026, 1, 1, 2, 0, 0, TimeSpan.FromHours(5)).UtcDateTime, clock.UtcNow.UtcDateTime);
    }

    [TestMethod]
    public async Task ConcurrentAdvance_DoesNotLoseUpdates() {
        var clock = new ManualClock(_epoch);
        var tasks = new Task[8];
        for(var index = 0; index < tasks.Length; index++) {
            tasks[index] = Task.Run(() => {
                for(var step = 0; step < 125; step++) {
                    clock.Advance(TimeSpan.FromTicks(1));
                }
            });
        }

        await Task.WhenAll(tasks);

        Assert.AreEqual(_epoch.AddTicks(1000), clock.UtcNow);
    }

    [TestMethod]
    public void InjectedClock_DrivesBoundaryAssertions_AndAdvanceMovesTheWindow() {
        AssertionClock.Current = new ManualClock(_epoch);
        var value = new DateTime(2026, 1, 1, 0, 5, 0, DateTimeKind.Utc);

        value.Should().BeWithin(TimeSpan.FromMinutes(5));
        Assert.ThrowsExactly<AssertionFailedException>(() => value.Should().BeWithin(TimeSpan.FromMinutes(4)));

        ((ManualClock)AssertionClock.Current).Advance(TimeSpan.FromMinutes(2));
        value.Should().BeWithin(TimeSpan.FromMinutes(4));
    }
}

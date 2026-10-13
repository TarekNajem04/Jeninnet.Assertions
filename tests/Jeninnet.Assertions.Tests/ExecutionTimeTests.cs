//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class ExecutionTimeTests {
    [TestMethod]
    public void BeLessThan_PassForFastAction() {
        Action fast = () => { };

        var timed = fast.ExecutionTime();
        Assert.IsGreaterThanOrEqualTo(TimeSpan.Zero, timed.Elapsed);
        timed.Should().BeLessThan(TimeSpan.FromMinutes(1));
    }

    [TestMethod]
    public void BeLessThan_FailsForSlowAction() {
        Action slow = static () => SpinWait.SpinUntil(static () => false, 150);
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => slow.ExecutionTime().Should().BeLessThan(TimeSpan.FromMilliseconds(20)));
        Assert.Contains("Expected execution time to be less than", ex.Message);
        Assert.Contains("but found", ex.Message);
    }

    [TestMethod]
    public void BeGreaterThan_PassForSlowAction() {
        Action slow = static () => SpinWait.SpinUntil(static () => false, 60);
        var timed = slow.ExecutionTime();
        Assert.IsGreaterThan(TimeSpan.Zero, timed.Elapsed);
        timed.Should().BeGreaterThan(TimeSpan.FromMilliseconds(10)).And.BeGreaterThanOrEqualTo(TimeSpan.FromMilliseconds(10));
    }

    [TestMethod]
    public void BeLessThanOrEqualTo_FailsAboveBound() {
        Action slow = static () => SpinWait.SpinUntil(static () => false, 60);
        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => slow.ExecutionTime().Should().BeLessThanOrEqualTo(TimeSpan.FromMilliseconds(5)));
        Assert.Contains("Expected execution time to be less than or equal to", ex.Message);
    }

    [TestMethod]
    public void ExecutionTime_NullAction_ThrowsArgumentNull() {
        Action? action = null;
        Assert.ThrowsExactly<ArgumentNullException>(() => action!.ExecutionTime());
    }

    [TestMethod]
    public void ExecutionTime_AsyncAction_MeasuresDuration() {
        Func<Task> work = static () => Task.Delay(50);

        var timed = work.ExecutionTime();

        Assert.IsGreaterThanOrEqualTo(TimeSpan.Zero, timed.Elapsed);
        timed.Should().BeGreaterThan(TimeSpan.FromMilliseconds(10));
    }

    [TestMethod]
    public void ExecutionTime_NullAsyncAction_ThrowsArgumentNull() {
        Func<Task>? action = null;
        Assert.ThrowsExactly<ArgumentNullException>(() => action!.ExecutionTime());
    }

    [TestMethod]
    public void BeCloseTo_PassAndFail() {
        var elapsed = new ExecutionTime(TimeSpan.FromMilliseconds(100));
        elapsed.Should().BeCloseTo(TimeSpan.FromMilliseconds(105), TimeSpan.FromMilliseconds(10));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => elapsed.Should().BeCloseTo(TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual("Expected execution time to be close to 00:00:00.2000000 (±00:00:00.0100000), but found 00:00:00.1000000.", ex.Message);

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => elapsed.Should().BeCloseTo(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(-1)));
    }

    [TestMethod]
    public void ExecutionTimeOf_Member_MeasuresInvocation() {
        var clock = new StopwatchProbe();
        var timed = clock.ExecutionTimeOf(static c => c.Elapsed);
        Assert.IsGreaterThanOrEqualTo(TimeSpan.Zero, timed.Elapsed);
        timed.Should().BeLessThan(TimeSpan.FromMinutes(1));

        Assert.ThrowsExactly<ArgumentNullException>(() => ((StopwatchProbe)null!).ExecutionTimeOf(static c => c.Elapsed));
        Assert.ThrowsExactly<ArgumentNullException>(() => clock.ExecutionTimeOf((Expression<Func<StopwatchProbe, object>>)null!));
    }

    private sealed class StopwatchProbe {
        private readonly TimeSpan _elapsed = TimeSpan.FromMilliseconds(1);

        public TimeSpan Elapsed => _elapsed;
    }
}

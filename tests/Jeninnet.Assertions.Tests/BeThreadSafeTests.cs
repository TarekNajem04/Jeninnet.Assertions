//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class BeThreadSafeTests {
    [TestMethod]
    public async Task Copies_RunSimultaneously_AndAllComplete() {
        var inside = 0;
        var maxInside = 0;
        Action action = () => {
            var current = Interlocked.Increment(ref inside);
            InterlockedMax(ref maxInside, current);
            // Every copy waits (generously bounded: thread-pool scheduling under CI load is not a deadlock)
            // until all copies are inside at once, proving true simultaneity.
            SpinWait.SpinUntil(() => Volatile.Read(ref inside) >= 4, TimeSpan.FromSeconds(15));
            Interlocked.Decrement(ref inside);
        };

        await action.Should().BeThreadSafeAsync(copies: 4, timeout: TimeSpan.FromSeconds(60));

        Assert.AreEqual(4, Volatile.Read(ref maxInside));
    }

    [TestMethod]
    public async Task AllCopiesFail_AggregatedIntoSingleGroupedFailure() {
        Action action = static () => throw new InvalidOperationException("boom");

        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => action.Should().BeThreadSafeAsync(copies: 4, timeout: TimeSpan.FromSeconds(60)));
        Assert.AreEqual("Expected action to be thread-safe, but 4 of 4 copies failed:\n1. InvalidOperationException: boom (copies: 1, 2, 3, 4).", ex.Message);
    }

    [TestMethod]
    public async Task PartialFailure_CarriesTheFailingCopyNumber() {
        var ordinal = 0;
        Action action = () => {
            if(Interlocked.Increment(ref ordinal) == 2) {
                throw new InvalidOperationException("boom");
            }
        };

        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => action.Should().BeThreadSafeAsync(copies: 3, timeout: TimeSpan.FromSeconds(60)));
        Assert.Contains("Expected action to be thread-safe, but 1 of 3 copies failed:", ex.Message);
        Assert.Contains("1. InvalidOperationException: boom (copies: ", ex.Message);
    }

    /// <summary>
    /// The two copies deterministically deadlock: each grabs one lock, both meet at the countdown, then each
    /// waits forever for the other's lock. The stuck threads cannot be interrupted and are intentionally abandoned.
    /// </summary>
    [TestMethod]
    public async Task DeadlockedCopies_AreReportedAsAPossibleDeadlock() {
        var gateA = new object();
        var gateB = new object();
        using var firstLocked = new CountdownEvent(2);
        var ordinal = 0;
        Action action = () => {
            if(Interlocked.Increment(ref ordinal) % 2 == 1) {
                lock(gateA) {
                    firstLocked.Signal();
                    firstLocked.Wait();
                    lock(gateB) {
                        // Unreachable: the other copy holds gateB while waiting for gateA.
                    }
                }
            } else {
                lock(gateB) {
                    firstLocked.Signal();
                    firstLocked.Wait();
                    lock(gateA) {
                        // Unreachable: the other copy holds gateA while waiting for gateB.
                    }
                }
            }
        };

        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => action.Should().BeThreadSafeAsync(copies: 2, timeout: TimeSpan.FromMilliseconds(300)));
        Assert.AreEqual("Expected action to be thread-safe, but 2 of 2 copies did not complete within 00:00:00.3000000 (a deadlock or an unbounded wait).", ex.Message);
    }

    [TestMethod]
    public async Task Because_IsAppendedToTheAggregatedFailure() {
        Action action = static () => throw new InvalidOperationException("boom");

        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => action.Should().BeThreadSafeAsync(copies: 2, because: "the cache is shared"));
        Assert.Contains("because the cache is shared", ex.Message);
    }

    [TestMethod]
    public async Task Guards_RejectTooFewCopiesAndNegativeTimeout() {
        Action action = static () => { };

        var copiesEx = await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() => action.Should().BeThreadSafeAsync(copies: 1));
        Assert.AreEqual("copies", copiesEx.ParamName);

        var timeoutEx = await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() => action.Should().BeThreadSafeAsync(copies: 2, timeout: TimeSpan.FromSeconds(-1)));
        Assert.AreEqual("timeout", timeoutEx.ParamName);
    }

    private static void InterlockedMax(ref int target, int value) {
        int current;
        while(value > (current = Volatile.Read(ref target))) {
            Interlocked.CompareExchange(ref target, value, current);
        }
    }
}

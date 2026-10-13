//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class PollingBackoffTests {
    [TestMethod]
    public void Constant_RepeatsSameDelay() {
        var backoff = PollingBackoff.Constant(TimeSpan.FromMilliseconds(25));

        Assert.AreEqual(TimeSpan.FromMilliseconds(25), backoff.DelayFor(0));
        Assert.AreEqual(TimeSpan.FromMilliseconds(25), backoff.DelayFor(1));
        Assert.AreEqual(TimeSpan.FromMilliseconds(25), backoff.DelayFor(10));
        Assert.AreEqual(1.0, backoff.Factor);
    }

    [TestMethod]
    public void Exponential_GrowsByFactor() {
        var backoff = PollingBackoff.Exponential(TimeSpan.FromMilliseconds(10), 2.0, TimeSpan.FromSeconds(1));

        Assert.AreEqual(TimeSpan.FromMilliseconds(10), backoff.DelayFor(0));
        Assert.AreEqual(TimeSpan.FromMilliseconds(20), backoff.DelayFor(1));
        Assert.AreEqual(TimeSpan.FromMilliseconds(40), backoff.DelayFor(2));
        Assert.AreEqual(TimeSpan.FromMilliseconds(80), backoff.DelayFor(3));
    }

    [TestMethod]
    public void Exponential_CapsAtMaxInterval() {
        var backoff = PollingBackoff.Exponential(TimeSpan.FromMilliseconds(10), 2.0, TimeSpan.FromMilliseconds(100));

        Assert.AreEqual(TimeSpan.FromMilliseconds(100), backoff.DelayFor(4));
        Assert.AreEqual(TimeSpan.FromMilliseconds(100), backoff.DelayFor(50));
    }

    [TestMethod]
    public void Exponential_NoCap_DoesNotOverflow() {
        var backoff = PollingBackoff.Exponential(TimeSpan.FromMilliseconds(10));

        Assert.AreEqual(TimeSpan.MaxValue, backoff.MaxInterval);
        Assert.AreEqual(TimeSpan.MaxValue, backoff.DelayFor(1000));
    }

    [TestMethod]
    public void Exponential_DefaultFactorIsTwo() {
        var backoff = PollingBackoff.Exponential(TimeSpan.FromMilliseconds(10));

        Assert.AreEqual(2.0, backoff.Factor);
    }

    [TestMethod]
    public void Constant_NegativeInterval_Throws() =>
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => PollingBackoff.Constant(TimeSpan.FromMilliseconds(-1)));

    [TestMethod]
    public void Exponential_InvalidArguments_Throw() {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => PollingBackoff.Exponential(TimeSpan.FromMilliseconds(-1)));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => PollingBackoff.Exponential(TimeSpan.FromMilliseconds(10), 0.5));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => PollingBackoff.Exponential(TimeSpan.FromMilliseconds(10), 2.0, TimeSpan.FromMilliseconds(5)));
    }
}

//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class ActionAssertionsCoverageTests {
    [TestMethod]
    public void Throw_NoException_Fails() {
        Func<int> ok = static () => 1;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => ok.Should().Throw<InvalidOperationException>());
        Assert.AreEqual("Expected ok to throw InvalidOperationException, but no exception was thrown.", ex.Message);
    }

    [TestMethod]
    public void ThrowExactly_WrongTypeAndNone_Fail() {
        Func<int> boom = static () => throw new ArgumentException("x");
        var wrong = Assert.ThrowsExactly<AssertionFailedException>(() => boom.Should().ThrowExactly<InvalidOperationException>());
        Assert.AreEqual("Expected boom to throw exactly InvalidOperationException, but ArgumentException was thrown.", wrong.Message);

        Func<int> ok = static () => 1;
        var none = Assert.ThrowsExactly<AssertionFailedException>(() => ok.Should().ThrowExactly<InvalidOperationException>());
        Assert.AreEqual("Expected ok to throw exactly InvalidOperationException, but no exception was thrown.", none.Message);
    }

    [TestMethod]
    public void NotThrow_PassAndFail() {
        Func<int> ok = static () => 7;
        ok.Should().NotThrow();

        Func<int> boom = static () => throw new InvalidOperationException("x");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => boom.Should().NotThrow());
        Assert.AreEqual("Expected boom not to throw, but InvalidOperationException was thrown.", ex.Message);
    }

    [TestMethod]
    public void NotThrowAfter_Guards_ThrowArgumentOutOfRange() {
        Func<int> ok = static () => 1;
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ok.Should().NotThrowAfter(TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(10)));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ok.Should().NotThrowAfter(TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(-1)));
    }

    [TestMethod]
    public void SucceedWithin_WrapperGuardsAndFailure() {
        var attempts = 0;
        Func<int> flaky = () => {
            attempts++;
            return attempts < 2 ? throw new InvalidOperationException("not yet") : attempts;
        };
        flaky.Should().SucceedWithin(TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(10));
        Assert.AreEqual(2, attempts);

        Func<int> ok = static () => 1;
        Assert.ThrowsExactly<ArgumentNullException>(() => ok.Should().SucceedWithin(null!, TimeSpan.FromSeconds(1)));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ok.Should().SucceedWithin(TimeSpan.FromMilliseconds(-1)));

        Func<int> failing = static () => throw new InvalidOperationException("always");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => failing.Should().SucceedWithin(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual("Expected failing to succeed within 00:00:00.1000000, but it kept failing: always.", ex.Message);
    }

    [TestMethod]
    public void EventuallySatisfy_FailurePathsAndGuards() {
        Func<int> five = static () => 5;
        using(var scope = new AssertionScope()) {
            five.Should().EventuallySatisfy(static x => x > 10, TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(10));
            var failures = scope.Discard();
            Assert.HasCount(1, failures);
            Assert.AreEqual("Expected five to satisfy the condition within 00:00:00.0500000, but the last value was 5.", failures[0]);
        }

        Func<int> throwing = static () => throw new InvalidOperationException("boom");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => throwing.Should().EventuallySatisfy(static x => x > 10, TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual("Expected throwing to satisfy the condition within 00:00:00.0500000, but the last value was <null> (last error: boom).", ex.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => five.Should().EventuallySatisfy(null!, TimeSpan.FromSeconds(1)));
        Assert.ThrowsExactly<ArgumentNullException>(() => five.Should().EventuallySatisfy(static x => x > 0, null!, TimeSpan.FromSeconds(1)));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => five.Should().EventuallySatisfy(static x => x > 0, TimeSpan.FromMilliseconds(-1)));
    }

    [TestMethod]
    public void AllocateAtMost_GuardsThrewAndExceeded() {
        Func<int> ok = static () => 1;
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ok.Should().AllocateAtMost(-1));

        Func<int> boom = static () => throw new InvalidOperationException("x");
        var threw = Assert.ThrowsExactly<AssertionFailedException>(() => boom.Should().AllocateAtMost(1024));
        Assert.AreEqual("Expected boom to allocate at most 1 KB, but it threw InvalidOperationException.", threw.Message);

        Func<int> alloc = static () => new byte[50_000].Length;
        using var scope = new AssertionScope();
        alloc.Should().AllocateAtMost(1024);
        var failures = scope.Discard();
        Assert.HasCount(1, failures);
        Assert.Contains("to allocate at most 1 KB, but allocated", failures[0]);
    }

    [TestMethod]
    public void AllocateLessThan_GuardsThrewAndExceeded() {
        Func<int> ok = static () => 1;
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ok.Should().AllocateLessThan(-1));

        Func<int> boom = static () => throw new InvalidOperationException("x");
        var threw = Assert.ThrowsExactly<AssertionFailedException>(() => boom.Should().AllocateLessThan(1024));
        Assert.AreEqual("Expected boom to allocate less than 1 KB, but it threw InvalidOperationException.", threw.Message);

        Func<int> alloc = static () => new byte[50_000].Length;
        using var scope = new AssertionScope();
        alloc.Should().AllocateLessThan(1024);
        var failures = scope.Discard();
        Assert.HasCount(1, failures);
        Assert.Contains("to allocate less than 1 KB, but allocated", failures[0]);
    }
}

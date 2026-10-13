//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class AllocationTests {
    [TestMethod]
    public void ByteSizeSugar_Converts() {
        Assert.AreEqual(512L, 512.Bytes());
        Assert.AreEqual(2048L, 2.Kilobytes());
        Assert.AreEqual(3145728L, 3.Megabytes());
    }

    [TestMethod]
    public void AllocateAtMost_PassForSmallAction() {
        Action empty = () => { };

        empty.Should().AllocateAtMost(64.Kilobytes());
    }

    [TestMethod]
    public void AllocateAtMost_FailsReportingSizes() {
        Action alloc = () => {
            var buffer = new byte[100_000];
            GC.KeepAlive(buffer);
        };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => alloc.Should().AllocateAtMost(1.Kilobytes()));
        Assert.Contains("Expected alloc to allocate at most 1 KB, but allocated", ex.Message);
    }

    [TestMethod]
    public void AllocateLessThan_FailsWhenEqual() {
        Action empty = () => { };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => empty.Should().AllocateLessThan(0));
        Assert.Contains("Expected empty to allocate less than 0 B, but allocated", ex.Message);
    }

    [TestMethod]
    public void AllocateAtMost_NegativeBound_ThrowsArgumentOutOfRange() {
        Action empty = () => { };

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => empty.Should().AllocateAtMost(-1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => empty.Should().AllocateLessThan(-1));
    }

    [TestMethod]
    public void AllocateAtMost_FailsWhenActionThrows() {
        Action action = () => throw new InvalidOperationException("boom");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => action.Should().AllocateAtMost(1.Megabytes()));
        Assert.AreEqual("Expected action to allocate at most 1 MB, but it threw InvalidOperationException.", ex.Message);
        Assert.IsInstanceOfType<InvalidOperationException>(ex.InnerException);
    }

    [TestMethod]
    public async Task AllocateAtMostAsync_PassAndFail_Async() {
        Func<Task> empty = () => Task.CompletedTask;
        await empty.Should().AllocateAtMostAsync(64.Kilobytes());

        Func<Task> alloc = () => {
            var buffer = new byte[100_000];
            GC.KeepAlive(buffer);
            return Task.CompletedTask;
        };

        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => alloc.Should().AllocateAtMostAsync(1.Kilobytes()));
        Assert.Contains("Expected alloc to allocate at most 1 KB, but allocated", ex.Message);
    }
}

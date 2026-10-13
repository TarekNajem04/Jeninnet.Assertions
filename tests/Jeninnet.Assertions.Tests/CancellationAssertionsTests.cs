//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class CancellationAssertionsTests {
    [TestMethod]
    public async Task HonorCancellation_AsyncDelegate_ThrowsWhenCanceled() {
        Func<CancellationToken, Task> work = static token => {
            token.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        };

        await work.Should().HonorCancellationAsync();
    }

    [TestMethod]
    public async Task HonorCancellation_SyncAction_ThrowsWhenCanceled() {
        Action<CancellationToken> work = static token => token.ThrowIfCancellationRequested();

        await work.Should().HonorCancellationAsync();
    }

    [TestMethod]
    public async Task HonorCancellation_ValueProducingDelegate_ThrowsWhenCanceled() {
        Func<CancellationToken, int> compute = static token => {
            token.ThrowIfCancellationRequested();
            return 42;
        };

        await compute.Should().HonorCancellationAsync();
    }

    [TestMethod]
    public async Task HonorCancellation_TaskCanceledException_CarriesTheToken() {
        Func<CancellationToken, Task> work = static token => Task.FromCanceled(token);

        await work.Should().HonorCancellationAsync();
    }

    [TestMethod]
    public async Task HonorCancellation_IgnoringDelegate_Fails() {
        Func<CancellationToken, Task> work = static _ => Task.CompletedTask;

        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => work.Should().HonorCancellationAsync());
        Assert.AreEqual("Expected work to honor cancellation, but it completed without throwing OperationCanceledException.", ex.Message);
    }

    [TestMethod]
    public async Task HonorCancellation_WrongToken_Fails() {
        Func<CancellationToken, Task> work = static _ => Task.FromException(new OperationCanceledException());

        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => work.Should().HonorCancellationAsync());
        Assert.AreEqual("Expected work to honor cancellation, but it threw OperationCanceledException with a different cancellation token.", ex.Message);
    }

    [TestMethod]
    public async Task HonorCancellation_WrongException_Fails() {
        Func<CancellationToken, Task> work = static _ => Task.FromException(new InvalidOperationException("boom"));

        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => work.Should().HonorCancellationAsync());
        Assert.AreEqual("Expected work to honor cancellation, but it threw InvalidOperationException instead of OperationCanceledException.", ex.Message);
    }

    [TestMethod]
    public async Task HonorCancellation_Because_IsAppended() {
        Func<CancellationToken, Task> work = static _ => Task.CompletedTask;

        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => work.Should().HonorCancellationAsync("the gateway must stop on shutdown"));
        Assert.AreEqual("Expected work to honor cancellation, but it completed without throwing OperationCanceledException because the gateway must stop on shutdown.", ex.Message);
    }

    [TestMethod]
    public async Task CompleteDespiteCancellation_AsyncDelegate_Passes() {
        Func<CancellationToken, Task> flush = static _ => Task.CompletedTask;

        await flush.Should().CompleteDespiteCancellationAsync();
    }

    [TestMethod]
    public async Task CompleteDespiteCancellation_SyncAction_Passes() {
        var flushed = false;
        Action<CancellationToken> cleanup = _ => flushed = true;

        await cleanup.Should().CompleteDespiteCancellationAsync();

        Assert.IsTrue(flushed);
    }

    [TestMethod]
    public async Task CompleteDespiteCancellation_ValueProducingDelegate_Passes() {
        Func<CancellationToken, int> compute = static _ => 42;

        await compute.Should().CompleteDespiteCancellationAsync();
    }

    [TestMethod]
    public async Task CompleteDespiteCancellation_CanceledDelegate_Fails() {
        Func<CancellationToken, Task> flush = static token => {
            token.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        };

        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => flush.Should().CompleteDespiteCancellationAsync());
        Assert.AreEqual("Expected flush to complete despite cancellation, but it was canceled.", ex.Message);
    }

    [TestMethod]
    public async Task CompleteDespiteCancellation_OtherException_Fails() {
        Func<CancellationToken, Task> flush = static _ => Task.FromException(new InvalidOperationException("boom"));

        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => flush.Should().CompleteDespiteCancellationAsync());
        Assert.AreEqual("Expected flush to complete despite cancellation, but it threw InvalidOperationException.", ex.Message);
    }

    [TestMethod]
    public void NullSubjects_ThrowArgumentNull() {
        var asyncEx = Assert.ThrowsExactly<ArgumentNullException>(() => ((Func<CancellationToken, Task>)null!).Should());
        Assert.AreEqual("action", asyncEx.ParamName);

        var actionEx = Assert.ThrowsExactly<ArgumentNullException>(() => ((Action<CancellationToken>)null!).Should());
        Assert.AreEqual("action", actionEx.ParamName);

        var funcEx = Assert.ThrowsExactly<ArgumentNullException>(() => ((Func<CancellationToken, int>)null!).Should());
        Assert.AreEqual("action", funcEx.ParamName);
    }

    [TestMethod]
    public async Task HonorCancellation_SoftMode_CollectsFailure() {
        var soft = new SoftAssertions().Collect();
        Func<CancellationToken, Task> work = static _ => Task.CompletedTask;

        await work.Should().HonorCancellationAsync();

        Assert.HasCount(1, soft.Failures);
        Assert.AreEqual("work", soft.Failures[0].Expression);
        Assert.AreEqual("CancellationAssertions", soft.Failures[0].Category);
        Assert.AreEqual("Expected work to honor cancellation, but it completed without throwing OperationCanceledException.", soft.Failures[0].Message);
    }
}

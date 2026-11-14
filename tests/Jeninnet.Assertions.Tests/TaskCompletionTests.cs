//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class TaskCompletionTests {
    [TestMethod]
    public void BeCompleted_AndNotBeCompleted() {
        var completed = Task.CompletedTask;
        completed.Should().BeCompleted().And.BeCompletedSuccessfully();

        var pending = new TaskCompletionSource().Task;
        pending.Should().NotBeCompleted();

        var completedEx = Assert.ThrowsExactly<AssertionFailedException>(() => pending.Should().BeCompleted());
        Assert.AreEqual("Expected pending to be completed, but it was WaitingForActivation.", completedEx.Message);

        var notCompletedEx = Assert.ThrowsExactly<AssertionFailedException>(() => completed.Should().NotBeCompleted());
        Assert.AreEqual("Expected completed not to be completed, but it was RanToCompletion.", notCompletedEx.Message);
    }

    [TestMethod]
    public void BeCompletedSuccessfully_FaultAndCancel() {
        var faulted = Task.FromException(new InvalidOperationException("boom"));
        var successEx = Assert.ThrowsExactly<AssertionFailedException>(() => faulted.Should().BeCompletedSuccessfully());
        Assert.AreEqual("Expected faulted to complete successfully, but it was Faulted (InvalidOperationException).", successEx.Message);

        var canceled = Task.FromCanceled(new CancellationToken(canceled: true));
        canceled.Should().BeCanceled();
        var canceledEx = Assert.ThrowsExactly<AssertionFailedException>(() => canceled.Should().BeCompletedSuccessfully());
        Assert.AreEqual("Expected canceled to complete successfully, but it was Canceled.", canceledEx.Message);
    }

    [TestMethod]
    public void BeFaulted_ExposesBaseException() {
        var faulted = Task.FromException(new InvalidOperationException("boom"));
        var which = faulted.Should().BeFaulted();
        Assert.IsInstanceOfType<InvalidOperationException>(which.Which);

        var completed = Task.CompletedTask;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => completed.Should().BeFaulted());
        Assert.AreEqual("Expected completed to be faulted, but it was RanToCompletion.", ex.Message);
    }

    [TestMethod]
    public void BeCanceled_FailsForCompletedTask() {
        var completed = Task.CompletedTask;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => completed.Should().BeCanceled());
        Assert.AreEqual("Expected completed to be canceled, but it was RanToCompletion.", ex.Message);
    }

    [TestMethod]
    public void TaskOfT_StateAndResult() {
        var result = Task.FromResult(7);
        result.Should().BeCompleted()
            .And.BeCompletedSuccessfully().Which.Should().Be(7);
        result.Should().HaveResult().Which.Should().Be(7);
        result.Should().HaveResult(7);
        result.Should().HaveResult(static value => value == 7);

        var mismatch = Assert.ThrowsExactly<AssertionFailedException>(() => result.Should().HaveResult(8));
        Assert.AreEqual("Expected result to have result 8, but found 7.", mismatch.Message);

        var predicate = Assert.ThrowsExactly<AssertionFailedException>(() => result.Should().HaveResult(static value => value > 10));
        Assert.AreEqual("Expected result to have a result matching the predicate, but found 7.", predicate.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => result.Should().HaveResult((Func<int, bool>)null!));
    }

    [TestMethod]
    public void TaskOfT_HaveResult_FailsImmediatelyWhenNotSuccessful() {
        var pending = new TaskCompletionSource<int>().Task;
        var pendingEx = Assert.ThrowsExactly<AssertionFailedException>(() => pending.Should().HaveResult(1));
        Assert.AreEqual("Expected pending to have a result, but it was WaitingForActivation.", pendingEx.Message);

        var faulted = Task.FromException<int>(new InvalidOperationException("boom"));
        var faultedWhich = faulted.Should().BeFaulted();
        Assert.IsInstanceOfType<InvalidOperationException>(faultedWhich.Which);
        var faultedEx = Assert.ThrowsExactly<AssertionFailedException>(() => faulted.Should().HaveResult(1));
        Assert.Contains("to have a result, but it was Faulted (InvalidOperationException)", faultedEx.Message);

        var canceled = Task.FromCanceled<int>(new CancellationToken(canceled: true));
        canceled.Should().BeCanceled();
        var canceledEx = Assert.ThrowsExactly<AssertionFailedException>(() => canceled.Should().BeCompletedSuccessfully());
        Assert.Contains("to complete successfully, but it was Canceled", canceledEx.Message);
    }

    [TestMethod]
    public async Task CompleteWithinAsync_PassForCompletedSource_Async() {
        var source = new TaskCompletionSource();
        source.SetResult();
        await source.Should().CompleteWithinAsync(TimeSpan.FromSeconds(5));
    }

    [TestMethod]
    public async Task CompleteWithinAsync_FailsOnTimeout_Async() {
        var pending = new TaskCompletionSource();
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => pending.Should().CompleteWithinAsync(TimeSpan.FromMilliseconds(50)));
        Assert.AreEqual("Expected pending to complete within 00:00:00.0500000, but it did not.", ex.Message);
        Assert.IsInstanceOfType<TimeoutException>(ex.InnerException);
    }

    [TestMethod]
    public async Task CompleteWithinAsync_FailsForFaultedSource_Async() {
        var source = new TaskCompletionSource();
        source.SetException(new InvalidOperationException("tcs"));
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => source.Should().CompleteWithinAsync(TimeSpan.FromSeconds(5)));
        Assert.AreEqual("Expected source to complete successfully within 00:00:05, but InvalidOperationException was thrown.", ex.Message);
        Assert.IsInstanceOfType<InvalidOperationException>(ex.InnerException);
    }

    [TestMethod]
    public async Task CompleteWithinAsync_Generic_PassAndFail_Async() {
        var source = new TaskCompletionSource<int>();
        source.SetResult(7);
        await source.Should().CompleteWithinAsync(TimeSpan.FromSeconds(5));

        var pending = new TaskCompletionSource<int>();
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => pending.Should().CompleteWithinAsync(TimeSpan.FromMilliseconds(50)));
        Assert.AreEqual("Expected pending to complete within 00:00:00.0500000, but it did not.", ex.Message);
    }

    [TestMethod]
    public async Task CompleteWithinAsync_NegativeTimeout_ThrowsArgumentOutOfRange_Async() {
        var source = new TaskCompletionSource();
        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() => source.Should().CompleteWithinAsync(TimeSpan.FromMilliseconds(-1)));
    }

    [TestMethod]
    public void TaskCompletionSource_NullSource_ThrowsArgumentNull() {
        TaskCompletionSource? source = null;
        Assert.ThrowsExactly<ArgumentNullException>(() => source!.Should());
    }

    [TestMethod]
    public async Task NotCompleteWithinAsync_PassAndFail_Async() {
        var pending = new TaskCompletionSource();
        await pending.Should().NotCompleteWithinAsync(TimeSpan.FromMilliseconds(50));

        var completed = new TaskCompletionSource();
        completed.SetResult();
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => completed.Should().NotCompleteWithinAsync(TimeSpan.FromMilliseconds(50)));
        Assert.AreEqual("Expected completed not to complete within 00:00:00.0500000, but it completed.", ex.Message);

        var genericPending = new TaskCompletionSource<int>();
        await genericPending.Should().NotCompleteWithinAsync(TimeSpan.FromMilliseconds(50));
    }

    [TestMethod]
    public async Task Task_CompleteAndNotCompleteWithinAsync_Async() {
        var completed = Task.FromResult(7);
        await completed.Should().CompleteWithinAsync(TimeSpan.FromSeconds(5));

        var pending = new TaskCompletionSource<int>();
        await pending.Task.Should().NotCompleteWithinAsync(TimeSpan.FromMilliseconds(50));

        var alreadyCompleted = Task.FromResult(7);
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => alreadyCompleted.Should().NotCompleteWithinAsync(TimeSpan.FromMilliseconds(50)));
        Assert.AreEqual("Expected alreadyCompleted not to complete within 00:00:00.0500000, but it completed.", ex.Message);

        Task<int>? missing = null;
        Assert.ThrowsExactly<ArgumentNullException>(() => missing!.Should());
    }

    [TestMethod]
    public async Task CompleteWithinAsync_WithResult_Async() {
        var task = Task.FromResult(7);
        await task.Should().CompleteWithinAsync(TimeSpan.FromSeconds(5)).WithResultAsync(7);

        var source = new TaskCompletionSource<int>();
        source.SetResult(8);
        await source.Should().CompleteWithinAsync(TimeSpan.FromSeconds(5)).WithResultAsync(8);

        var wrong = Task.FromResult(7);
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => wrong.Should().CompleteWithinAsync(TimeSpan.FromSeconds(5)).WithResultAsync(8));
        Assert.AreEqual("Expected wrong to have result 8, but found 7.", ex.Message);
    }
}

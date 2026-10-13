//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class AsyncEnumerableAssertionsTests {
    [TestMethod]
    public async Task BeEmpty_PassFailAndNull() {
        await Sequence().Should().BeEmptyAsync();

        var items = Sequence(1, 2);
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => items.Should().BeEmptyAsync());
        Assert.AreEqual("Expected items to be empty, but found 2 item(s).", ex.Message);

        IAsyncEnumerable<int>? missing = null;
        var nullEx = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => missing.Should().BeEmptyAsync());
        Assert.AreEqual("Expected missing to be a non-null async sequence, but it was null.", nullEx.Message);
    }

    [TestMethod]
    public async Task NotBeEmpty_PassAndFail() {
        await Sequence(1).Should().NotBeEmptyAsync();

        var empty = Sequence();
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => empty.Should().NotBeEmptyAsync());
        Assert.AreEqual("Expected empty not to be empty, but it was empty.", ex.Message);
    }

    [TestMethod]
    public async Task HaveCount_PassAndFail() {
        await Sequence(1, 2, 3).Should().HaveCountAsync(3);

        var words = Sequence(1, 2);
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => words.Should().HaveCountAsync(3));
        Assert.AreEqual("Expected words to have 3 item(s), but found 2.", ex.Message);
    }

    [TestMethod]
    public async Task Contain_PassFailAndNull() {
        await Sequence(1, 2).Should().ContainAsync(2);

        var words = Sequence(1, 2);
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => words.Should().ContainAsync(9));
        Assert.AreEqual("Expected words to contain '9', but it was not found.", ex.Message);

        IAsyncEnumerable<int>? missing = null;
        var nullEx = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => missing.Should().ContainAsync(1));
        Assert.AreEqual("Expected missing to be a non-null async sequence, but it was null.", nullEx.Message);
    }

    [TestMethod]
    public async Task NotContain_PassAndIndexedFailure() {
        await Sequence(1, 2).Should().NotContainAsync(9);

        var words = Sequence(1, 2, 3);
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => words.Should().NotContainAsync(2));
        Assert.AreEqual("Expected words not to contain '2', but found it at index 1.", ex.Message);
    }

    [TestMethod]
    public async Task ContainInOrder_AllowsGaps_AndNamesTheSearchWindow() {
        await Sequence(1, 2, 3, 4).Should().ContainInOrderAsync([1, 3]);
        await Sequence(1, 2).Should().ContainInOrderAsync([]);

        var firstMissing = Sequence(1, 2);
        var firstEx = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => firstMissing.Should().ContainInOrderAsync([3]));
        Assert.AreEqual("Expected firstMissing to contain '3' in order, but it was not found at or after index 0.", firstEx.Message);

        var afterMatch = Sequence(1, 2, 3);
        var afterEx = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => afterMatch.Should().ContainInOrderAsync([3, 2]));
        Assert.AreEqual("Expected afterMatch to contain '2' in order, but it was not found at or after index 3.", afterEx.Message);

        var nullExpected = await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => Sequence(1).Should().ContainInOrderAsync(null!));
        Assert.AreEqual("expected", nullExpected.ParamName);
    }

    [TestMethod]
    public async Task BeEquivalentTo_IsAMultiset() {
        await Sequence(2, 1, 1).Should().BeEquivalentToAsync([1, 2, 1]);

        var countMismatch = Sequence(1, 2, 3);
        var countEx = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => countMismatch.Should().BeEquivalentToAsync([1, 2]));
        Assert.AreEqual("Expected countMismatch to have 2 item(s), but found 3.", countEx.Message);

        var missing = Sequence(1, 2);
        var missingEx = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => missing.Should().BeEquivalentToAsync([1, 3]));
        Assert.AreEqual("Expected missing to be equivalent to the expected items, but no match was found for '3'.", missingEx.Message);
    }

    [TestMethod]
    public async Task Complete_DrainsAndExposesItems() {
        var result = await Sequence(1, 2, 3).Should().CompleteAsync();

        Assert.HasCount(3, result.Which);
        Assert.AreEqual(1, result.Which[0]);
        Assert.AreEqual(3, result.Which[2]);

        IAsyncEnumerable<int>? missing = null;
        var nullEx = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => missing.Should().CompleteAsync());
        Assert.AreEqual("Expected missing to be a non-null async sequence, but it was null.", nullEx.Message);
    }

    [TestMethod]
    public async Task CanceledToken_ThrowsBeforeEnumerating() {
        using var source = new CancellationTokenSource();
        await source.CancelAsync();

        var words = Sequence(1);
        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => words.Should().HaveCountAsync(1, source.Token));
        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => words.Should().CompleteAsync(source.Token));
        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => words.Should().BeEmptyAsync(source.Token));
    }

    [TestMethod]
    public async Task OperationCanceledException_IsNotSwallowedAsAFailure() {
        using var source = new CancellationTokenSource();
        await source.CancelAsync();

        var failure = await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => Sequence(1).Should().NotContainAsync(2, source.Token));
        Assert.AreEqual(source.Token, failure.CancellationToken);
    }

    private static async IAsyncEnumerable<int> Sequence(params int[] items) {
        foreach(var item in items) {
            await Task.Yield();
            yield return item;
        }
    }
}

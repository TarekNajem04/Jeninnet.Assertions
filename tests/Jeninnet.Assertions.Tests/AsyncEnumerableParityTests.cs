//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class AsyncEnumerableParityTests {
    private static readonly int[] _visited = [1, 20];
    [TestMethod]
    public async Task All_PassFailAndGuard() {
        await Sequence(2, 4).Should().AllAsync(static item => item % 2 == 0);

        var items = Sequence(2, 3, 4);
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => items.Should().AllAsync(static item => item % 2 == 0));
        Assert.AreEqual("Expected every item of items to satisfy the predicate, but item at index 1 did not.", ex.Message);

        await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => Sequence(1).Should().AllAsync(null!));
    }

    [TestMethod]
    public async Task Any_PassFailAndGuard() {
        await Sequence(1, 2, 3).Should().AnyAsync(static item => item > 2);

        var items = Sequence(1, 2);
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => items.Should().AnyAsync(static item => item > 5));
        Assert.AreEqual("Expected items to contain at least one item satisfying the predicate, but none did.", ex.Message);

        await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => Sequence(1).Should().AnyAsync(null!));
    }

    [TestMethod]
    public async Task ContainSingle_ReturnsWhichAndFailsAtZeroOrMany() {
        var single = await Sequence(7).Should().ContainSingleAsync();
        Assert.AreEqual(7, single.Which);

        var empty = Sequence();
        var emptyEx = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => empty.Should().ContainSingleAsync());
        Assert.AreEqual("Expected empty to contain a single item, but it was empty.", emptyEx.Message);

        var many = Sequence(1, 2);
        var manyEx = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => many.Should().ContainSingleAsync());
        Assert.AreEqual("Expected many to contain a single item, but found 2.", manyEx.Message);
    }

    [TestMethod]
    public async Task ContainSingle_WithPredicate_CountsOnlyMatches() {
        var match = await Sequence(1, 2, 3).Should().ContainSingleAsync(static item => item == 2);
        Assert.AreEqual(2, match.Which);

        var none = Sequence(1, 3);
        var noneEx = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => none.Should().ContainSingleAsync(static item => item == 2));
        Assert.AreEqual("Expected none to contain a single item satisfying the predicate, but none did.", noneEx.Message);

        var many = Sequence(2, 4);
        var manyEx = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => many.Should().ContainSingleAsync(static item => item % 2 == 0));
        Assert.AreEqual("Expected many to contain a single item satisfying the predicate, but found 2.", manyEx.Message);
    }

    [TestMethod]
    public async Task OnlyHaveUniqueItems_PassAndReportDuplicate() {
        await Sequence(1, 2, 3).Should().OnlyHaveUniqueItemsAsync();

        var items = Sequence(1, 2, 2, 3);
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => items.Should().OnlyHaveUniqueItemsAsync());
        Assert.AreEqual("Expected items to contain only unique items, but '2' was repeated at index 2.", ex.Message);
    }

    [TestMethod]
    public async Task ContainInConsecutiveOrder_FindsStartMiddleAndEndWindows() {
        await Sequence(1, 2, 3, 4).Should().ContainInConsecutiveOrderAsync([1, 2]);
        await Sequence(1, 2, 3, 4).Should().ContainInConsecutiveOrderAsync([2, 3]);
        await Sequence(1, 2, 3, 4).Should().ContainInConsecutiveOrderAsync([3, 4]);
        await Sequence(1, 2).Should().ContainInConsecutiveOrderAsync([]);

        var items = Sequence(1, 2, 3, 4);
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => items.Should().ContainInConsecutiveOrderAsync([2, 4]));
        Assert.AreEqual("Expected items to contain the items [2, 4] in consecutive order, but they were not found.", ex.Message);
    }

    [TestMethod]
    public async Task NotContainInOrder_PassAndFail() {
        await Sequence(1, 3).Should().NotContainInOrderAsync([1, 2]);

        var items = Sequence(1, 2, 3);
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => items.Should().NotContainInOrderAsync([1, 3]));
        Assert.AreEqual("Expected items not to contain the items [1, 3] in order, but they were found.", ex.Message);

        var empty = Sequence(1);
        var emptyEx = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => empty.Should().NotContainInOrderAsync([]));
        Assert.AreEqual("Expected empty not to contain the items [] in order, but they were found.", emptyEx.Message);
    }

    [TestMethod]
    public async Task CountComparisons_PassAndFail() {
        await Sequence(1, 2, 3).Should().HaveCountGreaterThanAsync(2);
        await Sequence(1, 2, 3).Should().HaveCountGreaterThanOrEqualToAsync(3);
        await Sequence(1, 2, 3).Should().HaveCountLessThanAsync(4);
        await Sequence(1, 2, 3).Should().HaveCountLessThanOrEqualToAsync(3);

        var items = Sequence(1, 2, 3);
        var greater = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => items.Should().HaveCountGreaterThanAsync(3));
        Assert.AreEqual("Expected items to have more than 3 item(s), but found 3.", greater.Message);

        var atLeast = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => items.Should().HaveCountGreaterThanOrEqualToAsync(4));
        Assert.AreEqual("Expected items to have at least 4 item(s), but found 3.", atLeast.Message);

        var fewer = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => items.Should().HaveCountLessThanAsync(3));
        Assert.AreEqual("Expected items to have fewer than 3 item(s), but found 3.", fewer.Message);

        var atMost = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => items.Should().HaveCountLessThanOrEqualToAsync(2));
        Assert.AreEqual("Expected items to have at most 2 item(s), but found 3.", atMost.Message);
    }

    [TestMethod]
    public async Task SatisfyRespectively_AwaitsInspectorsInOrderAndChecksCount() {
        var visited = new List<int>();
        await Sequence(1, 2).Should().SatisfyRespectivelyAsync([
            async item => { await Task.Yield(); visited.Add(item); },
            async item => { await Task.Yield(); visited.Add(item * 10); },
        ]);

        CollectionAssert.AreEqual(_visited, visited);

        var mismatch = Sequence(1, 2, 3);
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => mismatch.Should().SatisfyRespectivelyAsync([
            static _ => Task.CompletedTask,
            static _ => Task.CompletedTask,
        ]));
        Assert.AreEqual("Expected mismatch to have 2 item(s), but found 3.", ex.Message);
    }

    [TestMethod]
    public async Task Satisfy_ChecksEveryPredicateAndReportsTheIndex() {
        await Sequence(1, 2).Should().SatisfyAsync([
            static item => Task.FromResult(item == 1),
            static item => Task.FromResult(item == 2),
        ]);

        var items = Sequence(1, 2);
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => items.Should().SatisfyAsync([
            static _ => Task.FromResult(true),
            static _ => Task.FromResult(false),
        ]));
        Assert.AreEqual("Expected the item at index 1 of items to satisfy its predicate, but it did not.", ex.Message);
    }

    [TestMethod]
    public async Task NewVerbs_AreCancellationAware() {
        using var source = new CancellationTokenSource();
        await source.CancelAsync();
        var token = source.Token;

        var items = Sequence(1, 2);
        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => items.Should().AllAsync(static _ => true, token));
        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => items.Should().AnyAsync(static _ => true, token));
        await Assert.ThrowsExactlyAsync<OperationCanceledException>(async () => { _ = await items.Should().ContainSingleAsync(token); });
        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => items.Should().OnlyHaveUniqueItemsAsync(token));
        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => items.Should().ContainInConsecutiveOrderAsync([1, 2], token));
        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => items.Should().NotContainInOrderAsync([1, 2], token));
        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => items.Should().HaveCountGreaterThanAsync(1, token));
        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => items.Should().SatisfyRespectivelyAsync([static _ => Task.CompletedTask], token));
        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => items.Should().SatisfyAsync([static _ => Task.FromResult(true)], token));
    }

    [TestMethod]
    public async Task NewVerbs_EnumerateTheSourceExactlyOnce() {
        await AssertSingleEnumeration(static source => source.Should().AllAsync(static _ => true));
        await AssertSingleEnumeration(static source => source.Should().AnyAsync(static _ => true));
        await AssertSingleEnumeration(async source => { _ = await source.Should().ContainSingleAsync(); });
        await AssertSingleEnumeration(static source => source.Should().OnlyHaveUniqueItemsAsync());
        await AssertSingleEnumeration(static source => source.Should().ContainInConsecutiveOrderAsync([1]));
        await AssertSingleEnumeration(static source => source.Should().NotContainInOrderAsync([9]));
        await AssertSingleEnumeration(static source => source.Should().HaveCountGreaterThanAsync(0));
        await AssertSingleEnumeration(static source => source.Should().HaveCountGreaterThanOrEqualToAsync(1));
        await AssertSingleEnumeration(static source => source.Should().HaveCountLessThanAsync(2));
        await AssertSingleEnumeration(static source => source.Should().HaveCountLessThanOrEqualToAsync(1));
        await AssertSingleEnumeration(async source => { _ = await source.Should().ContainSingleAsync(static item => item == 1); });
        await AssertSingleEnumeration(static source => source.Should().SatisfyRespectivelyAsync([static _ => Task.CompletedTask]));
        await AssertSingleEnumeration(static source => source.Should().SatisfyAsync([static _ => Task.FromResult(true)]));
    }

    private static async Task AssertSingleEnumeration(Func<CountingSequence<int>, Task> assertion) {
        var source = new CountingSequence<int>(1);

        await assertion(source);

        Assert.AreEqual(1, source.Enumerations);
    }

    private static async IAsyncEnumerable<int> Sequence(params int[] items) {
        foreach(var item in items) {
            await Task.Yield();
            yield return item;
        }
    }

    private sealed class CountingSequence<T>(params T[] items) : IAsyncEnumerable<T> {
        private readonly T[] _items = items;

        public int Enumerations { get; private set; }

        public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) {
            Enumerations++;
            foreach(var item in _items) {
                await Task.Yield();
                cancellationToken.ThrowIfCancellationRequested();
                yield return item;
            }
        }
    }
}

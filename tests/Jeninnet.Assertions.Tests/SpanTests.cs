//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class SpanTests {
    [TestMethod]
    public void Arrays_StillBindToCollections() {
        int[] values = [1, 2, 3];
        values.Should().HaveCount(3).And.Contain(1);
    }

    [TestMethod]
    public void Span_AsAssertions_HaveCount() {
        int[] values = [1, 2, 3];
        var span = values.AsSpan();
        var assertions = span.AsAssertions();
        assertions.HaveCount(3).And.Contain(2);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => assertions.HaveCount(2));
        Assert.AreEqual("Expected span to contain 2 item(s), but found 3.", ex.Message);
    }

    [TestMethod]
    public void ReadOnlySpan_AsAssertions() {
        ReadOnlySpan<int> span = (int[])[];
        var assertions = span.AsAssertions();
        assertions.BeEmpty().And.HaveCount(0);

        ReadOnlySpan<int> fullSpan = (int[])[1];
        var fullAssertions = fullSpan.AsAssertions();
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => fullAssertions.HaveCount(0));
        Assert.AreEqual("Expected fullSpan to contain 0 item(s), but found 1.", ex.Message);
    }

    [TestMethod]
    public void ReadOnlySpan_BeEmpty_FailsForContent() {
        ReadOnlySpan<int> span = (int[])[1];
        var assertions = span.AsAssertions();
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => assertions.BeEmpty());
        Assert.AreEqual("Expected span to be empty, but found 1 item(s).", ex.Message);
    }

    [TestMethod]
    public void Span_ContainSingle_ExposesWhich() {
        int[] values = [7];
        var span = values.AsSpan();
        span.AsAssertions().ContainSingle().Which.Should().Be(7);

        int[] pair = [7, 8];
        var pairAssertions = pair.AsSpan().AsAssertions();
        Assert.ThrowsExactly<AssertionFailedException>(() => pairAssertions.ContainSingle());
    }

    [TestMethod]
    public void Span_BeEquivalentTo_IgnoresOrder() {
        int[] values = [1, 2, 3];
        var span = values.AsSpan();
        var assertions = span.AsAssertions();
        assertions.BeEquivalentTo(3, 1, 2);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => assertions.BeEquivalentTo(1, 2));
        Assert.AreEqual("Expected span to contain 2 item(s), but found 3.", ex.Message);
    }

    [TestMethod]
    public void Span_Equal_RequiresOrder() {
        int[] values = [1, 2, 3];
        var span = values.AsSpan();
        var assertions = span.AsAssertions();
        assertions.Equal(1, 2, 3);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => assertions.Equal(3, 2, 1));
        Assert.AreEqual("Expected span to equal ['3', '2', '1'], but item at index 0 was '1' instead of '3'.", ex.Message);
    }

    [TestMethod]
    public void Span_StartWith_EndWith() {
        int[] values = [1, 2, 3];
        var span = values.AsSpan();
        var assertions = span.AsAssertions();
        assertions.StartWith(1, 2).And.EndWith(2, 3);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => assertions.StartWith(2, 3));
        Assert.AreEqual("Expected span to start with ['2', '3'], but item at index 0 was '1'.", ex.Message);
    }

    [TestMethod]
    public void Span_Should_MatchAndMismatch() {
        int[] values = [1, 2, 3];
        values.AsSpan().Should().HaveCount(3).And.Contain(2);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.AsSpan().Should().HaveCount(2));
        Assert.AreEqual("Expected values.AsSpan() to contain 2 item(s), but found 3.", ex.Message);
    }

    [TestMethod]
    public void Span_Should_EmptySpan() {
        ReadOnlySpan<int> span = (int[])[];
        span.Should().BeEmpty();
    }

    [TestMethod]
    public void Memory_Should_MatchAndMismatch() {
        var memory = new Memory<int>([1, 2, 3]);
        memory.Should().HaveCount(3);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => memory.Should().HaveCount(2));
        Assert.AreEqual("Expected memory to contain 2 item(s), but found 3.", ex.Message);

        var readonlyMemory = new ReadOnlyMemory<int>([1, 2, 3]);
        readonlyMemory.Should().Contain(2);
    }

    [TestMethod]
    public void SpanChar_Should_BeStringWithoutAmbiguity() {
        Span<char> span = ['a', 'b', 'c'];
        span.Should().Be("abc");

        char[] other = ['a', 'b', 'd'];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => other.AsSpan().Should().Be("abc"));
        Assert.AreEqual("Expected other.AsSpan() to be 'abc', but found 'abd'.", ex.Message);

        "abc".AsSpan().Should().Be("abc");
    }

    [TestMethod]
    public void Span_ExtendedVerbs_DelegateToCollections() {
        int[] values = [1, 2, 3];
        values.AsSpan().Should().BeEqualTo([1, 2, 3]);
        values.AsSpan().Should().NotBeEqualTo([1, 3, 2]);
        values.AsSpan().Should().NotEqual([3, 2, 1]);
        values.AsSpan().Should().BeSubsetOf([1, 2, 3, 4]);
        values.AsSpan().Should().BeSupersetOf([1, 2]);
        values.AsSpan().Should().BeProperSupersetOf([1, 2]);
        values.AsSpan().Should().HaveSameCount([7, 8, 9]);
        values.AsSpan().Should().NotHaveSameCount([7, 8]);
        values.AsSpan().Should().IntersectWith([3, 9]);
        values.AsSpan().Should().NotIntersectWith([7, 8]);
        values.AsSpan().Should().ContainInOrder([1, 3]);
        values.AsSpan().Should().NotContainInOrder([3, 1]);
        values.AsSpan().Should().ContainInConsecutiveOrder([2, 3]);
        values.AsSpan().Should().NotContainInConsecutiveOrder([1, 3]);
        values.AsSpan().Should().HaveElementAt(0, 1);
        values.AsSpan().Should().HaveElementSucceeding(1, 2);
        values.AsSpan().Should().HaveElementPreceding(2, 1);
        values.AsSpan().Should().SatisfyRespectively(x => x.Should().Be(1), x => x.Should().Be(2), x => x.Should().Be(3));
        values.AsSpan().Should().AllSatisfy(static x => x.Should().BePositive());
        values.AsSpan().Should().StartWith(1);
        values.AsSpan().Should().EndWith(3);
        values.AsSpan().Should().OnlyHaveUniqueItems();
        values.AsSpan().Should().NotContainNulls();
        values.AsSpan().Should().HaveCountGreaterThan(2);
        values.AsSpan().Should().NotHaveCount(5);
        values.AsSpan().Should().AllBeOfType<int>().Which.Should().Be(1);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.AsSpan().Should().BeSupersetOf([1, 9]));
        Assert.AreEqual("Expected values.AsSpan() to be a superset of ['1', '9'], but '9' was missing from it.", ex.Message);
    }
}

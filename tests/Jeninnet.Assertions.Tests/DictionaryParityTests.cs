//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class DictionaryParityTests {
    private static Dictionary<string, int> Scores() => new() { ["a"] = 1, ["b"] = 2, };

    private sealed class CountingPairs(IEnumerable<KeyValuePair<string, int>> inner) : IEnumerable<KeyValuePair<string, int>> {
        public int Enumerations { get; private set; }

        public IEnumerator<KeyValuePair<string, int>> GetEnumerator() {
            Enumerations++;
            return inner.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [TestMethod]
    public void OverloadResolution_KeepsDictionaryAndReadOnlyDictionaryOnTheMutableEntryPoint() {
        var scores = Scores();
        var mutable = scores.Should();
        var readOnly = new ReadOnlyDictionary<string, int>(scores).Should();
        var readOnlyInterface = ((IReadOnlyDictionary<string, int>)scores).Should();
        var pairs = ((IEnumerable<KeyValuePair<string, int>>)scores).Should();
        var list = new List<KeyValuePair<string, int>>(scores);
        var listAssertions = list.Should();

        mutable.HaveCount(2).And.ContainKey("a");
        readOnly.ContainKey("a").And.NotContainKey("z");
        readOnlyInterface.ContainValue(2).Which.Should().Be(2);
        pairs.Contain("a", 1);
        listAssertions.ContainKey("b").And.Contain("a", 1);
    }

    [TestMethod]
    public void ReadOnlyDictionary_SupportsExistingVocabulary() {
        IReadOnlyDictionary<string, int> scores = Scores();

        scores.Should().HaveCount(2)
            .And.ContainKey("a")
            .And.Contain("b", 2)
            .And.NotContainKey("z")
            .And.HaveSameCount(Scores());
        scores.Should().ContainValue(2).Which.Should().Be(2);
        scores.Should().NotContainValue(9);
        scores.Should().Equal(Scores());
    }

    [TestMethod]
    public void ImmutableAndFrozenDictionaries_WorkThroughReadOnlyViews() {
        IReadOnlyDictionary<string, int> immutableView = ImmutableDictionary.CreateRange(Scores());
        immutableView.Should().HaveCount(2).And.ContainKey("a").And.Contain("b", 2);

        IReadOnlyDictionary<string, int> frozenView = Scores().ToFrozenDictionary();
        frozenView.Should().HaveCount(2).And.ContainKey("b").And.NotContainKey("z");
        frozenView.Should().ContainValue(1).Which.Should().Be(1);
    }

    [TestMethod]
    public void EnumerablePairs_AreMaterializedOnce() {
        var counting = new CountingPairs(Scores());
        var assertions = counting.Should();

        Assert.AreEqual(1, counting.Enumerations);

        assertions.HaveCount(2)
            .And.ContainKey("a")
            .And.Contain("b", 2)
            .And.HaveCountGreaterThan(1)
            .And.HaveCountLessThan(3);
        assertions.ContainValue(2).Which.Should().Be(2);

        Assert.AreEqual(1, counting.Enumerations);
    }

    [TestMethod]
    public void EnumerablePairs_EmptyAndSingle() {
        IEnumerable<KeyValuePair<string, int>> empty = [];
        empty.Should().BeEmpty();
        var emptySingle = Assert.ThrowsExactly<AssertionFailedException>(() => empty.Should().ContainSingle());
        Assert.AreEqual("Expected empty to contain exactly one item, but found 0.", emptySingle.Message);

        IEnumerable<KeyValuePair<string, int>> single = [new("a", 1)];
        var which = single.Should().ContainSingle().Which;
        Assert.AreEqual("a", which.Key);
        Assert.AreEqual(1, which.Value);
    }

    [TestMethod]
    public void EnumerablePairs_NullBehavesLikeNullDictionary() {
        IEnumerable<KeyValuePair<string, int>>? missing = null;

        missing.Should().BeNull();
        var key = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().ContainKey("a"));
        Assert.AreEqual("Expected missing to contain key 'a', but dictionary is null.", key.Message);

        var single = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().ContainSingle());
        Assert.AreEqual("Expected missing to contain exactly one item, but dictionary is null.", single.Message);
    }

    [TestMethod]
    public void CountBounds_PassAndFail() {
        var scores = Scores();
        scores.Should().HaveCountGreaterThan(1)
            .And.HaveCountGreaterThanOrEqualTo(2)
            .And.HaveCountLessThan(3)
            .And.HaveCountLessThanOrEqualTo(2);

        var greater = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().HaveCountGreaterThan(2));
        Assert.AreEqual("Expected scores to contain more than 2 item(s), but found 2.", greater.Message);

        var atLeast = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().HaveCountGreaterThanOrEqualTo(3));
        Assert.AreEqual("Expected scores to contain at least 3 item(s), but found 2.", atLeast.Message);

        var fewer = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().HaveCountLessThan(2));
        Assert.AreEqual("Expected scores to contain fewer than 2 item(s), but found 2.", fewer.Message);

        var atMost = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().HaveCountLessThanOrEqualTo(1));
        Assert.AreEqual("Expected scores to contain at most 1 item(s), but found 2.", atMost.Message);
    }

    [TestMethod]
    public void CountBounds_NullDictionary() {
        Dictionary<string, int>? missing = null;

        Assert.AreEqual("Expected missing to contain more than 2 item(s), but dictionary is null.", Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveCountGreaterThan(2)).Message);
        Assert.AreEqual("Expected missing to contain at least 2 item(s), but dictionary is null.", Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveCountGreaterThanOrEqualTo(2)).Message);
        Assert.AreEqual("Expected missing to contain fewer than 2 item(s), but dictionary is null.", Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveCountLessThan(2)).Message);
        Assert.AreEqual("Expected missing to contain at most 2 item(s), but dictionary is null.", Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveCountLessThanOrEqualTo(2)).Message);
    }

    [TestMethod]
    public void ContainSingle_PassAndFail() {
        var single = new Dictionary<string, int> { ["a"] = 1 };
        var which = single.Should().ContainSingle().Which;
        Assert.AreEqual("a", which.Key);
        Assert.AreEqual(1, which.Value);

        var scores = Scores();
        var multiple = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().ContainSingle());
        Assert.AreEqual("Expected scores to contain exactly one item, but found 2.", multiple.Message);

        var empty = new Dictionary<string, int>();
        var none = Assert.ThrowsExactly<AssertionFailedException>(() => empty.Should().ContainSingle());
        Assert.AreEqual("Expected empty to contain exactly one item, but found 0.", none.Message);

        Dictionary<string, int>? missing = null;
        var nullDictionary = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().ContainSingle());
        Assert.AreEqual("Expected missing to contain exactly one item, but dictionary is null.", nullDictionary.Message);
    }
}

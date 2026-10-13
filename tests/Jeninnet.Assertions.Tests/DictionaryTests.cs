//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class DictionaryTests {
    private static Dictionary<string, int> Scores() => new() { ["a"] = 1, ["b"] = 2, };

    [TestMethod]
    public void HaveCount_PassAndFail() {
        var scores = Scores();
        scores.Should().HaveCount(2).And.ContainKey("a");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().HaveCount(3));
        Assert.AreEqual("Expected scores to contain 3 item(s), but found 2.", ex.Message);
    }

    [TestMethod]
    public void BeEmpty_NotBeEmpty() {
        var empty = new Dictionary<string, int>();
        empty.Should().BeEmpty();

        var scores = Scores();
        scores.Should().NotBeEmpty();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().BeEmpty());
        Assert.AreEqual("Expected scores to be empty, but found 2 item(s).", ex.Message);
    }

    [TestMethod]
    public void ContainKey_PassAndFail() {
        var scores = Scores();
        scores.Should().ContainKey("a");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().ContainKey("z"));
        Assert.AreEqual("Expected scores to contain key 'z', but it was not found.", ex.Message);
    }

    [TestMethod]
    public void NotContainKey_PassAndFail() {
        var scores = Scores();
        scores.Should().NotContainKey("z");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().NotContainKey("a"));
        Assert.AreEqual("Expected scores not to contain key 'a', but it was found.", ex.Message);
    }

    [TestMethod]
    public void ContainKeys_PassAndFail() {
        var scores = Scores();
        scores.Should().ContainKeys("a", "b");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().ContainKeys("a", "z"));
        Assert.AreEqual("Expected scores to contain key 'z', but it was not found.", ex.Message);
    }

    [TestMethod]
    public void NotContainKeys_PassAndFail() {
        var scores = Scores();
        scores.Should().NotContainKeys("x", "y");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().NotContainKeys("x", "a"));
        Assert.AreEqual("Expected scores not to contain key 'a', but it was found.", ex.Message);
    }

    [TestMethod]
    public void ContainValue_ExposesWhich() {
        var scores = Scores();
        scores.Should().ContainValue(2).Which.Should().Be(2);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().ContainValue(9));
        Assert.AreEqual("Expected scores to contain value '9', but it was not found.", ex.Message);
    }

    [TestMethod]
    public void Contain_KeyValue_PassAndFail() {
        var scores = Scores();
        scores.Should().Contain("a", 1);

        var wrongValue = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().Contain("a", 2));
        Assert.AreEqual("Expected scores to have value '2' at key 'a', but found '1'.", wrongValue.Message);

        var missingKey = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().Contain("z", 1));
        Assert.AreEqual("Expected scores to contain key 'z', but it was not found.", missingKey.Message);
    }

    [TestMethod]
    public void NullDictionary_FailsWithNullMessage() {
        IDictionary<string, int>? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().ContainKey("a"));
        Assert.AreEqual("Expected missing to contain key 'a', but dictionary is null.", ex.Message);
    }

    [TestMethod]
    public void NotEqual_PassForDifferentDictionaries() {
        var scores = Scores();
        scores.Should().NotEqual(new Dictionary<string, int> { ["a"] = 1, ["b"] = 3 });
    }

    [TestMethod]
    public void NotEqual_FailsForEqualDictionaries() {
        var scores = Scores();
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().NotEqual(new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 }));
        Assert.AreEqual("Expected scores not to equal 'System.Collections.Generic.Dictionary`2[System.String,System.Int32]', but found differences.", ex.Message);
    }

    [TestMethod]
    public void NotContain_PassForMissingKey() {
        var scores = Scores();
        scores.Should().NotContain("z");
    }

    [TestMethod]
    public void NotContain_FailsForPresentKey() {
        var scores = Scores();
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().NotContain("a"));
        Assert.AreEqual("Expected scores not to contain key 'a', but found it.", ex.Message);
    }

    [TestMethod]
    public void NotContainValue_PassForMissingValue() {
        var scores = Scores();
        scores.Should().NotContainValue(99);
    }

    [TestMethod]
    public void NotContainValue_FailsForPresentValue() {
        var scores = Scores();
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().NotContainValue(1));
        Assert.AreEqual("Expected scores not to contain value '1', but found it.", ex.Message);
    }

    [TestMethod]
    public void NotContainValues_PassForMissingValues() {
        var scores = Scores();
        scores.Should().NotContainValues([99, 98]);
    }

    [TestMethod]
    public void NotContainValues_FailsForPresentValues() {
        var scores = Scores();
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().NotContainValues([1, 99]));
        Assert.AreEqual("Expected scores not to contain values ['1', '99'], but found them.", ex.Message);
    }

    [TestMethod]
    public void NotHaveCount_PassForDifferentCount() {
        var scores = Scores();
        scores.Should().NotHaveCount(3);
    }

    [TestMethod]
    public void NotHaveCount_FailsForMatchingCount() {
        var scores = Scores();
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().NotHaveCount(2));
        Assert.AreEqual("Expected scores not to contain 2 item(s), but found 2.", ex.Message);
    }

    [TestMethod]
    public void NotHaveSameCount_PassForDifferentCount() {
        var scores = Scores();
        scores.Should().NotHaveSameCount<string>(new Dictionary<string, string> { ["x"] = "1" });
    }

    [TestMethod]
    public void NotHaveSameCount_FailsForMatchingCount() {
        var scores = Scores();
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().NotHaveSameCount<string>(new Dictionary<string, string> { ["x"] = "1", ["y"] = "2" }));
        Assert.AreEqual("Expected scores not to have the same count as 2 item(s), but found 2.", ex.Message);
    }

    [TestMethod]
    public void NotContainPair_PassAndFail() {
        var scores = Scores();
        scores.Should().NotContain(new KeyValuePair<string, int>("z", 9));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().NotContain(new KeyValuePair<string, int>("a", 1)));
        Assert.AreEqual("Expected scores not to contain pair '[a, 1]', but found it.", ex.Message);
    }

    [TestMethod]
    public void NotContainPairs_PassAndFail() {
        var scores = Scores();
        scores.Should().NotContain([new KeyValuePair<string, int>("z", 9)]);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().NotContain([new KeyValuePair<string, int>("z", 9), new KeyValuePair<string, int>("a", 1)]));
        Assert.AreEqual("Expected scores not to contain pairs ['[z, 9]', '[a, 1]'], but found '[a, 1]'.", ex.Message);
    }

    [TestMethod]
    public void NotContainKeyValue_PassAndFail() {
        var scores = Scores();
        scores.Should().NotContain("a", 2);
        scores.Should().NotContain("z", 1);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().NotContain("a", 1));
        Assert.AreEqual("Expected scores not to contain key 'a' with value '1', but found it.", ex.Message);
    }

    [TestMethod]
    public void NotContainPair_NullDictionaryFails() {
        IDictionary<string, int>? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotContain(new KeyValuePair<string, int>("a", 1)));
        Assert.AreEqual("Expected missing not to contain key 'a', but dictionary is null.", ex.Message);
    }

    [TestMethod]
    public void Equal_PassFailAndNullGuard() {
        var scores = Scores();
        scores.Should().Equal(new Dictionary<string, int> { ["b"] = 2, ["a"] = 1 });
        scores.Should().BeEqualTo(new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 });

        var countEx = Assert.ThrowsExactly<AssertionFailedException>(
            () => scores.Should().Equal(new Dictionary<string, int> { ["a"] = 1 }));
        Assert.AreEqual("Expected scores to equal ['[a, 1]'], but it has 2 item(s) instead of 1.", countEx.Message);

        var missingEx = Assert.ThrowsExactly<AssertionFailedException>(
            () => scores.Should().Equal(new Dictionary<string, int> { ["a"] = 1, ["z"] = 9 }));
        Assert.AreEqual("Expected scores to equal ['[a, 1]', '[z, 9]'], but key ''z'' was not found.", missingEx.Message);

        var wrongEx = Assert.ThrowsExactly<AssertionFailedException>(
            () => scores.Should().Equal(new Dictionary<string, int> { ["a"] = 1, ["b"] = 9 }));
        Assert.AreEqual("Expected scores to equal ['[a, 1]', '[b, 9]'], but key ''b'' was ''2'' instead of ''9''.", wrongEx.Message);

        scores.Should().NotBeEqualTo(new Dictionary<string, int> { ["a"] = 1 });

        IDictionary<string, int>? missingMap = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missingMap.Should().Equal(Scores()));
        Assert.AreEqual("Expected missingMap to equal ['[a, 1]', '[b, 2]'], but dictionary is null.", nullEx.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => scores.Should().Equal((IDictionary<string, int>)null!));
    }

    [TestMethod]
    public void EqualWithPredicate_ComparesPairWise() {
        var scores = Scores();
        var texts = new[] { "a-1", "b-2" };
        var renamed = new[] { "a-1", "b-9" };
        scores.Should().Equal(texts, static (pair, text) => $"{pair.Key}-{pair.Value}" == text);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => scores.Should().Equal(renamed, static (pair, text) => $"{pair.Key}-{pair.Value}" == text));
        Assert.Contains("was ''[b, 2]'' instead of ''b-9''.", ex.Message);
    }

    [TestMethod]
    public void SetRelations_PassAndFail() {
        var scores = Scores();
        scores.Should().BeSubsetOf([new KeyValuePair<string, int>("a", 1), new KeyValuePair<string, int>("b", 2), new KeyValuePair<string, int>("c", 3)]);
        scores.Should().BeSupersetOf([new KeyValuePair<string, int>("a", 1)]);
        scores.Should().BeSupersetOf([]);
        scores.Should().BeProperSubsetOf([new KeyValuePair<string, int>("a", 1), new KeyValuePair<string, int>("b", 2), new KeyValuePair<string, int>("c", 3)]);
        scores.Should().BeProperSupersetOf([new KeyValuePair<string, int>("a", 1)]);

        var outsider = Assert.ThrowsExactly<AssertionFailedException>(
            () => scores.Should().NotBeSubsetOf([new KeyValuePair<string, int>("a", 1), new KeyValuePair<string, int>("b", 2), new KeyValuePair<string, int>("c", 3)]));
        Assert.Contains("Expected scores to be a subset of", outsider.Message);

        var missing = Assert.ThrowsExactly<AssertionFailedException>(
            () => scores.Should().BeSupersetOf([new KeyValuePair<string, int>("z", 9)]));
        Assert.AreEqual("Expected scores to be a superset of ['[z, 9]'], but ''[z, 9]'' was missing from it.", missing.Message);

        var sameSub = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().BeProperSubsetOf(scores));
        Assert.AreEqual("Expected scores to be a proper subset of ['[a, 1]', '[b, 2]'], but they contain the same items.", sameSub.Message);

        var sameSuper = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().BeProperSupersetOf(scores));
        Assert.AreEqual("Expected scores to be a proper superset of ['[a, 1]', '[b, 2]'], but they contain the same items.", sameSuper.Message);
    }

    [TestMethod]
    public void ContainValues_PassFailAndWhich() {
        var scores = Scores();
        scores.Should().ContainValues((IEnumerable<int>)[1, 2]).Which.Should().HaveCount(2);
        scores.Should().ContainValues(1, 2);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().ContainValues(1, 9));
        Assert.AreEqual("Expected scores to contain value ['9'], but it was not found.", ex.Message);

        IDictionary<string, int>? missingMap = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missingMap.Should().ContainValues(1));
        Assert.AreEqual("Expected missingMap to contain value ['1'], but dictionary is null.", nullEx.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => scores.Should().ContainValues((IEnumerable<int>)null!));
    }

    [TestMethod]
    public void HaveSameCount_PassAndFail() {
        var scores = Scores();
        var pair = new[] { "x", "y" };
        var single = new[] { "x" };
        scores.Should().HaveSameCount(pair);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => scores.Should().HaveSameCount(single));
        Assert.AreEqual("Expected scores to have the same count (1) as the expected collection, but found 2.", ex.Message);

        IDictionary<string, int>? missingMap = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missingMap.Should().HaveSameCount(pair));
        Assert.AreEqual("Expected missingMap to have the same count (2) as the expected collection, but found <null>.", nullEx.Message);
    }
}

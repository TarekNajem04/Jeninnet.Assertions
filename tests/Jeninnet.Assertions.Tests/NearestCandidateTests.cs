//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class NearestCandidateTests {
    [TestMethod]
    public void Dictionary_TransposedKey_SuggestsTheNearestKey() {
        var prices = new Dictionary<string, int> { ["name"] = 1 };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => prices.Should().ContainKey("naem"));
        Assert.AreEqual("Expected prices to contain key 'naem', but it was not found. Did you mean 'name'?", ex.Message);
    }

    [TestMethod]
    public void Dictionary_ShortKey_SuggestsTheLongerKey() {
        var prices = new Dictionary<string, int> { ["name"] = 1 };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => prices.Should().ContainKey("nam"));
        Assert.AreEqual("Expected prices to contain key 'nam', but it was not found. Did you mean 'name'?", ex.Message);
    }

    [TestMethod]
    public void Dictionary_FarKey_AppendsNothing() {
        var prices = new Dictionary<string, int> { ["name"] = 1 };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => prices.Should().ContainKey("zzz"));
        Assert.AreEqual("Expected prices to contain key 'zzz', but it was not found.", ex.Message);
    }

    [TestMethod]
    public void Dictionary_SingleCharacterKey_IsTooShortToSuggest() {
        var prices = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => prices.Should().ContainKey("z"));
        Assert.AreEqual("Expected prices to contain key 'z', but it was not found.", ex.Message);
    }

    [TestMethod]
    public void Dictionary_NonStringKeys_AppendNothing() {
        var numbers = new Dictionary<int, int> { [1] = 10 };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => numbers.Should().ContainKey(2));
        Assert.AreEqual("Expected numbers to contain key '2', but it was not found.", ex.Message);
    }

    [TestMethod]
    public void Dictionary_MultipleSuggestions_AreCappedAtThree_AndOrderedOrdinally() {
        var codes = new Dictionary<string, int> { ["abh"] = 1, ["abe"] = 2, ["abg"] = 3, ["abf"] = 4 };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => codes.Should().ContainKey("abd"));
        Assert.AreEqual("Expected codes to contain key 'abd', but it was not found. Did you mean one of 'abe', 'abf', 'abg'?", ex.Message);
    }

    [TestMethod]
    public void Dictionary_CloserDistance_WinsOverAlphabeticalOrder() {
        var codes = new Dictionary<string, int> { ["zbcdef"] = 1, ["abcdxx"] = 2 };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => codes.Should().ContainKey("abcdef"));
        Assert.AreEqual("Expected codes to contain key 'abcdef', but it was not found. Did you mean one of 'zbcdef', 'abcdxx'?", ex.Message);
    }

    [TestMethod]
    public void StringCollection_TransposedItem_SuggestsTheNearestItem() {
        IEnumerable<string> words = ["alpha", "beta"];

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => words.Should().Contain("alhpa"));
        Assert.AreEqual("Expected words to contain 'alhpa', but it was not found. Did you mean 'alpha'?", ex.Message);
    }

    [TestMethod]
    public void JsonProperty_TransposedName_SuggestsASibling() {
        var node = JsonNode.Parse("""{"name":"a"}""");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => node.Should().HaveProperty("naem"));
        Assert.AreEqual("Expected node to have property 'naem', but found {\"name\":\"a\"}. Did you mean 'name'?", ex.Message);
    }

    [TestMethod]
    public void JsonPath_TypoedSegment_SuggestsASibling() {
        var node = JsonNode.Parse("""{"user":{"name":"ada","id":1}}""");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => node.Should().HaveJsonPath("$.user.naem"));
        Assert.AreEqual("Expected node to have path '$.user.naem', but property 'naem' was not found under '$.user' (available: name, id). Did you mean 'name'?", ex.Message);
    }

    [TestMethod]
    public void LargeDictionary_FailureDiagnostics_StayFast() {
        var big = new Dictionary<string, int>();
        for(var index = 0; index < 10000; index++) {
            big[$"key{index:D5}"] = index;
        }

        var started = DateTime.UtcNow;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => big.Should().ContainKey("key9999z"));
        var elapsed = DateTime.UtcNow - started;

        Assert.Contains("Did you mean", ex.Message);
        Assert.IsLessThan(TimeSpan.FromSeconds(5), elapsed, $"Diagnostics took {elapsed}.");
    }
}

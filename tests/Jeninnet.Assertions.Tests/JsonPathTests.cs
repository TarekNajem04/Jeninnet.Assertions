//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class JsonPathTests {
    [TestMethod]
    public void RootPath_ReturnsTheRootNode() {
        var node = JsonNode.Parse("""{"name":"a","tags":["x"]}""");

        node.Should().HaveJsonPath("$").Which.Should().HaveProperty("name").Which.Should().HaveValue("a");
    }

    [TestMethod]
    public void NestedObject_ResolvesToTheNode_AndChains() {
        var node = JsonNode.Parse("""{"user":{"name":"ada","id":1}}""");

        node.Should().HaveJsonPath("$.user.name").Which.Should().BeString().Which.Should().Be("ada");
        node.Should().HaveJsonPath("$.user").Which.Should().HaveProperty("id");
    }

    [TestMethod]
    public void ArrayIndex_ResolvesToTheNode() {
        var node = JsonNode.Parse("""{"users":[{"name":"ada"},{"name":"grace"}]}""");

        node.Should().HaveJsonPath("$.users[1].name").Which.Should().HaveValue("grace");

        var array = JsonNode.Parse("""[{"name":"ada"}]""");
        array.Should().HaveJsonPath("$[0].name").Which.Should().HaveValue("ada");
    }

    [TestMethod]
    public void NullLeaf_CountsAsPresent() {
        var node = JsonNode.Parse("""{"user":{"middle":null}}""");

        node.Should().HaveJsonPath("$.user.middle").Which.Should().BeNull();
    }

    [TestMethod]
    public void MissingProperty_FailsListingAvailableSiblings() {
        var node = JsonNode.Parse("""{"user":{"name":"ada","id":1}}""");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => node.Should().HaveJsonPath("$.user.email"));
        Assert.AreEqual("Expected node to have path '$.user.email', but property 'email' was not found under '$.user' (available: name, id).", ex.Message);
    }

    [TestMethod]
    public void MissingProperty_OnEmptyObject_OmitsTheAvailableList() {
        var node = JsonNode.Parse("""{"user":{}}""");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => node.Should().HaveJsonPath("$.user.email"));
        Assert.AreEqual("Expected node to have path '$.user.email', but property 'email' was not found under '$.user'.", ex.Message);
    }

    [TestMethod]
    public void AvailableSiblings_PreserveInsertionOrder_AndCapAtTen() {
        var ordered = JsonNode.Parse("""{"b":1,"a":2}""");
        var orderedEx = Assert.ThrowsExactly<AssertionFailedException>(() => ordered.Should().HaveJsonPath("$.c"));
        Assert.AreEqual("Expected ordered to have path '$.c', but property 'c' was not found under '$' (available: b, a).", orderedEx.Message);

        var many = JsonNode.Parse("""{"k1":1,"k2":2,"k3":3,"k4":4,"k5":5,"k6":6,"k7":7,"k8":8,"k9":9,"k10":10,"k11":11}""");
        var manyEx = Assert.ThrowsExactly<AssertionFailedException>(() => many.Should().HaveJsonPath("$.missing"));
        Assert.AreEqual("Expected many to have path '$.missing', but property 'missing' was not found under '$' (available: k1, k2, k3, k4, k5, k6, k7, k8, k9, k10, …).", manyEx.Message);
    }

    [TestMethod]
    public void PropertyNames_AreCaseSensitive() {
        var node = JsonNode.Parse("""{"Name":"ada"}""");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => node.Should().HaveJsonPath("$.name"));
        Assert.AreEqual("Expected node to have path '$.name', but property 'name' was not found under '$' (available: Name). Did you mean 'Name'?", ex.Message);
    }

    [TestMethod]
    public void IndexOutOfRange_FailsWithTheElementCount() {
        var array = JsonNode.Parse("[1,2,3]");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => array.Should().HaveJsonPath("$[5]"));
        Assert.AreEqual("Expected array to have path '$[5]', but index 5 is out of range under '$' (3 element(s) present).", ex.Message);
    }

    [TestMethod]
    public void NullMidPath_Fails() {
        var node = JsonNode.Parse("""{"a":null}""");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => node.Should().HaveJsonPath("$.a.b"));
        Assert.AreEqual("Expected node to have path '$.a.b', but '$.a' is null.", ex.Message);
    }

    [TestMethod]
    public void ScalarMidPath_Fails_NotAnObject() {
        var node = JsonNode.Parse("""{"a":42}""");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => node.Should().HaveJsonPath("$.a.b"));
        Assert.AreEqual("Expected node to have path '$.a.b', but '$.a' is not a JSON object.", ex.Message);
    }

    [TestMethod]
    public void IndexSegment_OnANonArray_Fails() {
        var node = JsonNode.Parse("""{"a":42}""");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => node.Should().HaveJsonPath("$.a[0]"));
        Assert.AreEqual("Expected node to have path '$.a[0]', but '$.a' is not a JSON array.", ex.Message);
    }

    [TestMethod]
    public void PropertySegment_OnANonObject_Fails() {
        var array = JsonNode.Parse("[1,2]");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => array.Should().HaveJsonPath("$.x"));
        Assert.AreEqual("Expected array to have path '$.x', but '$' is not a JSON object.", ex.Message);
    }

    [TestMethod]
    public void NullRoot_Fails_IsNull() {
        JsonNode? node = null;

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => node.Should().HaveJsonPath("$.a"));
        Assert.AreEqual("Expected node to have path '$.a', but '$' is null.", ex.Message);
    }

    [TestMethod]
    public void NotHaveJsonPath_PassAndFail() {
        var node = JsonNode.Parse("""{"a":1}""");

        node.Should().NotHaveJsonPath("$.missing");
        node.Should().NotHaveJsonPath("$.a.missing");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => node.Should().NotHaveJsonPath("$.a"));
        Assert.AreEqual("Expected node not to have path '$.a', but it exists.", ex.Message);
    }

    [TestMethod]
    public void MalformedPaths_ThrowArgumentException() {
        var node = JsonNode.Parse("""{"a":[1]}""");

        foreach(var path in (string[])["user.name", "$.", "$..a", "$.a[", "$[]", "$[-1]", "$[a]", "$.a]", "$.a[0]b", "$x", "$[99999999999999]"]) {
            var ex = Assert.ThrowsExactly<ArgumentException>(() => node.Should().HaveJsonPath(path));
            Assert.AreEqual("path", ex.ParamName);
            Assert.Contains("malformed", ex.Message);
        }
    }
}

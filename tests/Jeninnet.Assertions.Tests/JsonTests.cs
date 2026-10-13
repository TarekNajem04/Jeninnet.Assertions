//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class JsonTests {
    [TestMethod]
    public void BeEquivalentTo_PassForEqualDocuments() {
        var actual = JsonNode.Parse("""{"name":"a","tags":["x","y"]}""");
        var expected = JsonNode.Parse("""{"name":"a","tags":["x","y"]}""");
        actual.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void BeEquivalentTo_FailsReportingPreviews() {
        var actual = JsonNode.Parse("""{"name":"a"}""");
        var expected = JsonNode.Parse("""{"name":"b"}""");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.AreEqual("Expected actual to be equivalent, but found {\"name\":\"a\"} instead of {\"name\":\"b\"}.", ex.Message);
    }

    [TestMethod]
    public void HaveCount_PassAndFail() {
        var array = JsonNode.Parse("[1,2,3]");
        array.Should().HaveCount(3);

        var obj = JsonNode.Parse("""{"a":1}""");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => obj.Should().HaveCount(1));
        Assert.AreEqual("Expected obj to contain 1 item(s), but found {\"a\":1}.", ex.Message);
    }

    [TestMethod]
    public void HaveProperty_ExposesWhich() {
        var node = JsonNode.Parse("""{"name":"a"}""");
        node.Should().HaveProperty("name").Which.Should().HaveValue("a");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => node.Should().HaveProperty("missing"));
        Assert.AreEqual("Expected node to have property 'missing', but found {\"name\":\"a\"}.", ex.Message);
    }

    [TestMethod]
    public void HaveValue_PassAndFail() {
        JsonNode.Parse("42").Should().HaveValue(42);

        var node = JsonNode.Parse("1");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => node.Should().HaveValue(2));
        Assert.AreEqual("Expected node to have value 2, but found 1.", ex.Message);
    }

    [TestMethod]
    public void BeNull_NotBeNull() {
        JsonNode.Parse("null").Should().BeNull();
        ((JsonNode?)null).Should().BeNull();
        JsonNode.Parse("1").Should().NotBeNull();
        JsonNode.Parse("""{"a":null}""").Should().HaveProperty("a").Which.Should().BeNull();

        var jsonNull = JsonNode.Parse("null");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => jsonNull.Should().NotBeNull());
        Assert.AreEqual("Expected jsonNull not to be null, but found <null>.", ex.Message);
    }

    [TestMethod]
    public void NotHaveProperty_PassForMissingProperty() {
        var node = JsonNode.Parse("""{"name":"a"}""");
        node.Should().NotHaveProperty("missing");
    }

    [TestMethod]
    public void NotHaveProperty_FailsForPresentProperty() {
        var node = JsonNode.Parse("""{"name":"a"}""");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => node.Should().NotHaveProperty("name"));
        Assert.AreEqual("Expected node not to have property 'name', but found it.", ex.Message);
    }

    [TestMethod]
    public void NotBeEquivalentTo_PassAndFail() {
        var actual = JsonNode.Parse("""{"name":"a"}""");
        var different = JsonNode.Parse("""{"name":"b"}""");
        var same = JsonNode.Parse("""{"name":"a"}""");
        actual.Should().NotBeEquivalentTo(different);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().NotBeEquivalentTo(same));
        Assert.AreEqual("Expected actual not to be equivalent, but found {\"name\":\"a\"} instead of {\"name\":\"a\"}.", ex.Message);
    }

    [TestMethod]
    public void BeString_PassFailAndWhich() {
        JsonNode.Parse("\"a\"").Should().BeString().Which.Should().Be("a");

        var node = JsonNode.Parse("42");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => node.Should().BeString());
        Assert.AreEqual("Expected node to be a JSON string, but found 42.", ex.Message);

        JsonNode.Parse("42").Should().NotBeString();
        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => JsonNode.Parse("\"a\"").Should().NotBeString());
        Assert.Contains("not to be a JSON string", notEx.Message);
    }

    [TestMethod]
    public void BeNumeric_PassFailAndWhich() {
        JsonNode.Parse("42").Should().BeNumeric().Which.Should().Be("42");

        var node = JsonNode.Parse("\"a\"");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => node.Should().BeNumeric());
        Assert.AreEqual("Expected node to be a JSON number, but found \"a\".", ex.Message);

        JsonNode.Parse("\"a\"").Should().NotBeNumeric();
        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => JsonNode.Parse("42").Should().NotBeNumeric());
        Assert.Contains("not to be a JSON number", notEx.Message);
    }

    [TestMethod]
    public void BeBool_PassFailAndWhich() {
        JsonNode.Parse("true").Should().BeBool().Which.Should().BeTrue();

        var node = JsonNode.Parse("\"true\"");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => node.Should().BeBool());
        Assert.AreEqual("Expected node to be a JSON boolean, but found \"true\".", ex.Message);

        JsonNode.Parse("\"true\"").Should().NotBeBool();
        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => JsonNode.Parse("false").Should().NotBeBool());
        Assert.Contains("not to be a JSON boolean", notEx.Message);
    }

    [TestMethod]
    public void BeAnArray_PassFailAndWhich() {
        JsonNode.Parse("[1,2]").Should().BeAnArray().Which.Should().HaveCount(2);

        var node = JsonNode.Parse("""{"a":1}""");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => node.Should().BeAnArray());
        Assert.AreEqual("Expected node to be a JSON array, but found {\"a\":1}.", ex.Message);

        JsonNode.Parse("""{"a":1}""").Should().NotBeAnArray();
        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => JsonNode.Parse("[1]").Should().NotBeAnArray());
        Assert.Contains("not to be a JSON array", notEx.Message);
    }

    [TestMethod]
    public void BeUtcDate_PassFailAndWhich() {
        var utc = JsonNode.Parse("\"2026-10-05T00:00:00Z\"");
        var which = utc.Should().BeUtcDate().Which;
        Assert.AreEqual(DateTimeKind.Utc, which.Kind);

        var local = JsonNode.Parse("\"2026-10-05T00:00:00\"");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => local.Should().BeUtcDate());
        Assert.AreEqual("Expected local to be a UTC date, but found \"2026-10-05T00:00:00\".", ex.Message);

        local.Should().NotBeUtcDate();
        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => utc.Should().NotBeUtcDate());
        Assert.Contains("not to be a UTC date", notEx.Message);
    }

    [TestMethod]
    public void BeLocalDate_PassFailAndWhich() {
        var local = JsonNode.Parse("\"2026-10-05T00:00:00\"");
        var which = local.Should().BeLocalDate().Which;
        Assert.AreEqual(new DateTime(2026, 10, 5, 0, 0, 0, DateTimeKind.Unspecified), which);
        Assert.AreNotEqual(DateTimeKind.Utc, which.Kind);

        var utc = JsonNode.Parse("\"2026-10-05T00:00:00Z\"");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => utc.Should().BeLocalDate());
        Assert.AreEqual("Expected utc to be a local date, but found \"2026-10-05T00:00:00Z\".", ex.Message);

        utc.Should().NotBeLocalDate();
        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => local.Should().NotBeLocalDate());
        Assert.Contains("not to be a local date", notEx.Message);
    }

    [TestMethod]
    public void GenericEntry_PreservesClosedNodeType() {
        var node = JsonNode.Parse("""{"name":"a"}""");
        node.Should<JsonNode>().HaveProperty("name").Which.Should().HaveValue("a");

        var array = JsonNode.Parse("[1,2]")!.AsArray();
        array.Should<JsonArray>().BeAnArray().Which.Should().HaveCount(2);
    }

    [TestMethod]
    public void JsonArrayEntry_TreatsArrayAsCollection() {
        var array = JsonNode.Parse("[1,2,3]")!.AsArray();
        array.Should().HaveCount(3);
        array.Should().Contain(node => JsonNode.DeepEquals(node, JsonNode.Parse("2")));
    }
}

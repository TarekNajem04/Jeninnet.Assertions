//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class JsonElementTests {
    private static JsonElement Element(string json) {
        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }

    [TestMethod]
    public void KindsAndValues_Pass() {
        var root = Element("""{"name":"a","count":2,"active":true,"missing":null}""");
        root.Should().BeObject().And.NotBeAnArray();
        root.Should().HaveProperty("name").Which.Should().BeString().Which.Should().Be("a");
        root.Should().HaveProperty("count").Which.Should().BeNumeric().Which.Should().Be("2");
        root.Should().HaveProperty("active").Which.Should().BeBool().Which.Should().BeTrue();
        root.Should().HaveProperty("missing").Which.Should().BeNull();

        Element("[1,2,3]").Should().BeAnArray().And.HaveCount(3);
        Element("\"text\"").Should().BeString().Which.Should().Be("text");
        Element("42").Should().BeNumeric().Which.Should().Be("42");
        Element("true").Should().BeBool().Which.Should().BeTrue();
        Element("null").Should().BeNull().And.NotBeObject().And.NotBeAnArray();
    }

    [TestMethod]
    public void BeObject_FailureShowsPreview() {
        var element = Element("[1,2]");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => element.Should().BeObject());
        Assert.AreEqual("Expected element to be a JSON object, but found [1,2].", ex.Message);
    }

    [TestMethod]
    public void BeEquivalentTo_IgnoresObjectPropertyOrder() {
        var actual = Element("""{"a":1,"b":[2,3]}""");
        actual.Should().BeEquivalentTo(Element("""{"b":[2,3],"a":1}"""));
        actual.Should().NotBeEquivalentTo(Element("""{"a":1}"""));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(Element("""{"a":2}""")));
        Assert.Contains("to be equivalent", ex.Message);
    }

    [TestMethod]
    public void HaveValue_ComparesScalars() {
        Element("42").Should().HaveValue(42);
        Element("\"x\"").Should().HaveValue("x");
        Element("true").Should().HaveValue(true);

        var element = Element("1");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => element.Should().HaveValue(2));
        Assert.AreEqual("Expected element to have value 2, but found 1.", ex.Message);
    }

    [TestMethod]
    public void JsonDocument_Overload_UsesRootElement() {
        using var document = JsonDocument.Parse("""{"a":1}""");
        document.Should().BeObject().And.HaveProperty("a").Which.Should().HaveValue(1);
    }

    [TestMethod]
    public void NullJsonDocument_BehavesLikeMissingElement() {
        JsonDocument? missing = null;
        missing.Should().BeNull();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotBeNull());
        Assert.AreEqual("Expected missing not to be null, but found <null>.", ex.Message);
    }

    [TestMethod]
    public void DisposedDocument_FailsWithClearMessage() {
        var document = JsonDocument.Parse("{}");
        var root = document.RootElement;
        document.Dispose();

        var documentEx = Assert.ThrowsExactly<AssertionFailedException>(() => document.Should());
        Assert.AreEqual("Expected document to be a live JSON document, but it was disposed.", documentEx.Message);

        var elementEx = Assert.ThrowsExactly<AssertionFailedException>(() => root.Should());
        Assert.AreEqual("Expected root to be a live JSON document, but it was disposed.", elementEx.Message);
    }

    [TestMethod]
    public void HaveProperty_MissingKeySuggestsNearestName() {
        var element = Element("""{"Name":"a"}""");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => element.Should().HaveProperty("name"));
        Assert.AreEqual("Expected element to have property 'name', but found {\"Name\":\"a\"}. Did you mean 'Name'?", ex.Message);
    }

    [TestMethod]
    public void JsonPathV2_WorksOnElements() {
        var element = Element("""{"users":[{"name":"ada"},{"name":"grace"}]}""");

        element.Should().HaveJsonPath("$.users[-1].name").Which.Should().BeString().Which.Should().Be("grace");
        var values = element.Should().HaveJsonPathValues("$.users[*].name").Which;
        Assert.HasCount(2, values);
        values[0].Should().BeString().Which.Should().Be("ada");
        values[1].Should().BeString().Which.Should().Be("grace");

        var multiple = Assert.ThrowsExactly<AssertionFailedException>(() => element.Should().HaveJsonPath("$.users[*].name"));
        Assert.Contains("Use HaveJsonPathValues", multiple.Message);

        element.Should().NotHaveJsonPath("$.users[*].missing");
    }

    [TestMethod]
    public void MatchSnapshot_IsDeterministicAndLFOnly() {
        using var environment = new TestEnvironment();
        var directory = environment.CreateDirectory("json-snaps");
        var received = Path.Combine(directory, "payload.received.json");
        var verified = Path.Combine(directory, "payload.verified.json");
        var element = Element("""{"b":[2,3],"a":1}""");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => element.Should().MatchSnapshot("payload", directory));
        Assert.Contains("no verified snapshot exists", ex.Message);

        const string expected = "{\n  \"a\": 1,\n  \"b\": [\n    2,\n    3\n  ]\n}";
        Assert.AreEqual(expected, File.ReadAllText(received));
        Assert.DoesNotContain("\r", File.ReadAllText(received));

        File.Move(received, verified);
        element.Should().MatchSnapshot("payload", directory);
        Assert.IsFalse(File.Exists(received));
    }
}

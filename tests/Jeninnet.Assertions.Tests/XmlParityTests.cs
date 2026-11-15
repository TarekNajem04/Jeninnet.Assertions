//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class XmlParityTests {
    private const string PREFIXED = """<ns:root xmlns:ns="urn:ns"><ns:child id="1">text</ns:child></ns:root>""";
    private const string DEFAULT_NAMESPACE = """<root xmlns="urn:default"><child id="1">text</child></root>""";

    [TestMethod]
    public void XmlElement_Should_IsMoreSpecificThanXmlNode() {
        var document = new XmlDocument();
        document.LoadXml(PREFIXED);

        XmlElement element = document.DocumentElement!;
        XmlNode node = element;

        Assert.IsInstanceOfType<XmlElementAssertions>(element.Should());
        Assert.IsInstanceOfType<XmlNodeAssertions>(node.Should());
    }

    [TestMethod]
    public void XmlElement_NameNamespaceAndAttributes() {
        var document = new XmlDocument();
        document.LoadXml(PREFIXED);
        var root = document.DocumentElement!;

        root.Should().HaveName("ns:root").And.HaveNamespace("urn:ns");
        root.Should().HaveAttributeWithValue("xmlns:ns", "urn:ns");

        var child = root.Should().HaveElement("ns:child").Which;
        child.Should().HaveInnerText("text").And.HaveAttributeWithValue("id", "1");
        child.Should().HaveAttribute("id").Which.Should().HaveValue("1");
        child.Should().NotHaveAttribute("missing");
    }

    [TestMethod]
    public void XmlElement_NamespacesWorkForDefaultAndPrefixes() {
        var document = new XmlDocument();
        document.LoadXml(DEFAULT_NAMESPACE);
        var root = document.DocumentElement!;

        root.Should().HaveName("root").And.HaveNamespace("urn:default");
        root.Should().HaveElement("child").Which.Should().HaveNamespace("urn:default");
    }

    [TestMethod]
    public void XmlElement_HaveAttribute_DistinguishesMissingFromWrongValue() {
        var document = new XmlDocument();
        document.LoadXml(PREFIXED);
        var root = document.DocumentElement!;

        var missing = Assert.ThrowsExactly<AssertionFailedException>(() => root.Should().HaveAttribute("missing"));
        Assert.AreEqual("Expected root to have attribute 'missing', but it was not found.", missing.Message);

        var wrong = Assert.ThrowsExactly<AssertionFailedException>(() => root.Should().HaveAttributeWithValue("xmlns:ns", "urn:other"));
        Assert.AreEqual("Expected root to have attribute 'xmlns:ns' with value 'urn:other', but found 'urn:ns'.", wrong.Message);

        var name = Assert.ThrowsExactly<AssertionFailedException>(() => root.Should().HaveName("other"));
        Assert.AreEqual("Expected root to have name 'other', but found 'ns:root'.", name.Message);

        var ns = Assert.ThrowsExactly<AssertionFailedException>(() => root.Should().HaveNamespace("urn:other"));
        Assert.AreEqual("Expected root to have namespace 'urn:other', but found 'urn:ns'.", ns.Message);
    }

    [TestMethod]
    public void XmlElement_HaveElement_OccurrenceAndValue() {
        var document = new XmlDocument();
        document.LoadXml("""<root><child id="1">one</child><child id="2">two</child></root>""");
        var root = document.DocumentElement!;

        root.Should().HaveElement("child", Exactly.Twice()).Which.Should().HaveCount(2);
        root.Should().HaveElementWithValue("child", "two").Which.Should().HaveAttributeWithValue("id", "2");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => root.Should().HaveElementWithValue("child", "three"));
        Assert.AreEqual("Expected root to have element 'child' with value 'three', but found 'one'.", ex.Message);
    }

    [TestMethod]
    public void XmlElement_BeEquivalentTo_ReportsPathAndValue() {
        var actualDocument = new XmlDocument();
        actualDocument.LoadXml("""<root><child id="2">text</child></root>""");
        var expectedDocument = new XmlDocument();
        expectedDocument.LoadXml("""<root><child id="1">text</child></root>""");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actualDocument.DocumentElement!.Should().BeEquivalentTo(expectedDocument.DocumentElement));

        Assert.Contains("/root/child/@id", ex.Message);
        Assert.Contains("expected '1', found '2'", ex.Message);
    }

    [TestMethod]
    public void XmlElement_BeEquivalentTo_ReportsElementAndNamespaceDifferences() {
        var actual = new XmlDocument();
        actual.LoadXml("""<root><other /></root>""");
        var expected = new XmlDocument();
        expected.LoadXml("""<root><child /></root>""");

        var elementEx = Assert.ThrowsExactly<AssertionFailedException>(() => actual.DocumentElement!.Should().BeEquivalentTo(expected.DocumentElement));
        Assert.Contains("/root/other", elementEx.Message);

        var nsActual = new XmlDocument();
        nsActual.LoadXml("""<root xmlns="urn:actual" />""");
        var nsExpected = new XmlDocument();
        nsExpected.LoadXml("""<root xmlns="urn:expected" />""");
        var nsEx = Assert.ThrowsExactly<AssertionFailedException>(() => nsActual.DocumentElement!.Should().BeEquivalentTo(nsExpected.DocumentElement));
        Assert.Contains("/root", nsEx.Message);
        Assert.Contains("urn:expected", nsEx.Message);
        Assert.Contains("urn:actual", nsEx.Message);
    }

    [TestMethod]
    public void XElement_EquivalencyOptions_RelaxWhitespaceOrderAndComments() {
        var compact = XElement.Parse("""<root><a>1</a><b>2</b></root>""");
        var formatted = XElement.Parse("""
            <root>
              <!-- note -->
              <a>1</a>
              <b>2</b>
            </root>
            """);

        var strict = Assert.ThrowsExactly<AssertionFailedException>(() => compact.Should().BeEquivalentTo(formatted));
        Assert.Contains("/root/comment()", strict.Message);

        compact.Should().BeEquivalentTo(formatted, options => options
            .IgnoringWhitespace()
            .IgnoringComments());

        var reversed = XElement.Parse("""<root><b>2</b><a>1</a></root>""");
        var order = Assert.ThrowsExactly<AssertionFailedException>(() => compact.Should().BeEquivalentTo(reversed));
        Assert.Contains("/root/a", order.Message);

        compact.Should().BeEquivalentTo(reversed, options => options.IgnoringElementOrder());
    }

    [TestMethod]
    public void XDocument_NewVerbs_DelegateToTheRoot() {
        var defaultDocument = XDocument.Parse(DEFAULT_NAMESPACE);

        defaultDocument.Should().HaveAttributeWithValue("xmlns", "urn:default");
        defaultDocument.Should().NotHaveAttribute("missing");

        var prefixed = XDocument.Parse(PREFIXED);
        prefixed.Should().HaveAttributeWithValue(XNamespace.Xmlns + "ns", "urn:ns");

        var value = XDocument.Parse("""<root id="7">text</root>""");
        value.Should().HaveAttributeWithValue("id", "7");
        value.Should().HaveValue("text");
        value.Should().HaveNamespace("");
    }

    [TestMethod]
    public void XDocument_BeEquivalentTo_ReportsPath() {
        var actual = XDocument.Parse("""<root><child id="2">text</child></root>""");
        var expected = XDocument.Parse("""<root><child id="1">text</child></root>""");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));

        Assert.Contains("/root/child/@id", ex.Message);
        Assert.Contains("expected '1', found '2'", ex.Message);
    }
}

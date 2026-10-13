//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class XmlTests {
    private const string CATALOG = """<root><child id="1">text</child></root>""";

    [TestMethod]
    public void XDocument_RootAndElement() {
        var doc = XDocument.Parse(CATALOG);
        doc.Should().HaveRoot("root").Which.Should().HaveElement("child");
    }

    [TestMethod]
    public void XDocument_HaveElement_FailsForMissing() {
        var doc = XDocument.Parse(CATALOG);
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => doc.Should().HaveElement("missing"));
        Assert.AreEqual("Expected doc to have element 'missing', but it was not found.", ex.Message);
    }

    [TestMethod]
    public void XElement_ValueAndAttribute() {
        var doc = XDocument.Parse(CATALOG);
        doc.Should().HaveElement("child").Which.Should().HaveValue("text").And.HaveAttributeWithValue("id", "1");
        doc.Should().HaveElement("child").Which.Should().HaveAttribute("id").Which.Should().HaveValue("1");
    }

    [TestMethod]
    public void XDocument_BeEquivalentTo() {
        var first = XDocument.Parse(CATALOG);
        var second = XDocument.Parse(CATALOG);
        first.Should().BeEquivalentTo(second);

        var changed = XDocument.Parse("""<root><child id="2">text</child></root>""");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => first.Should().BeEquivalentTo(changed));
        Assert.Contains("to be equivalent", ex.Message);
    }

    [TestMethod]
    public void ClassicXml_ElementAttributeValue() {
        var doc = new XmlDocument();
        doc.LoadXml("""<root><child id="1">text</child></root>""");
        doc.Should().HaveElement("child").Which.Should().HaveValue("text");
        doc.Should().HaveElement("child").Which.Should().HaveAttribute("id").Which.Value.Should().Be("1");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => doc.Should().HaveElement("missing"));
        Assert.AreEqual("Expected doc to have element 'missing', but it was not found.", ex.Message);
    }

    [TestMethod]
    public void ClassicXml_BeEquivalentTo() {
        var first = new XmlDocument();
        first.LoadXml(CATALOG);
        var second = new XmlDocument();
        second.LoadXml(CATALOG);
        first.Should().BeEquivalentTo(second);

        var changed = new XmlDocument();
        changed.LoadXml("""<root><child id="2">text</child></root>""");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => first.Should().BeEquivalentTo(changed));
        Assert.Contains("to be equivalent", ex.Message);
    }

    [TestMethod]
    public void XElement_NotHaveElementAndAttribute() {
        var element = XElement.Parse(CATALOG);
        element.Should().NotHaveElement("missing");
        element.Should().NotHaveAttribute("missing");

        var elementEx = Assert.ThrowsExactly<AssertionFailedException>(() => element.Should().NotHaveElement("child"));
        Assert.AreEqual("Expected element not to have element 'child', but it was found.", elementEx.Message);

        var child = element.Should().HaveElement("child").Which;
        var attributeEx = Assert.ThrowsExactly<AssertionFailedException>(() => child.Should().NotHaveAttribute("id"));
        Assert.AreEqual("Expected child not to have attribute 'id', but it was found.", attributeEx.Message);
    }

    [TestMethod]
    public void XElement_XNameOverloads_MatchQualifiedNames() {
        var element = XElement.Parse("""<root xmlns:part="urn:example"><part:child part:id="1">text</part:child></root>""");
        var name = XNamespace.Get("urn:example") + "child";
        element.Should().HaveElement(name).Which.Should().HaveValue("text");
        element.Should().NotHaveElement(XNamespace.Get("urn:other") + "child");

        var attributeName = XNamespace.Get("urn:example") + "id";
        element.Should().HaveElement(name).Which.Should().HaveAttribute(attributeName).Which.Should().HaveValue("1");
        element.Should().HaveElement(name).Which.Should().NotHaveAttribute(XNamespace.Get("urn:other") + "id");
    }

    [TestMethod]
    public void XElement_ElementWithValue_PassAndFail() {
        var element = XElement.Parse(CATALOG);
        element.Should().HaveElementWithValue("child", "text").Which.Should().HaveAttributeWithValue("id", "1");
        element.Should().NotHaveElementWithValue("child", "other");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => element.Should().HaveElementWithValue("child", "other"));
        Assert.AreEqual("Expected element to have element 'child' with value 'other', but found 'text'.", ex.Message);

        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => element.Should().NotHaveElementWithValue("child", "text"));
        Assert.AreEqual("Expected element not to have element 'child' with value 'text', but found it.", notEx.Message);
    }

    [TestMethod]
    public void XElement_AttributeWithValue_PassAndFail() {
        var element = XElement.Parse(CATALOG);
        var child = element.Should().HaveElement("child").Which;
        child.Should().HaveAttributeWithValue("id", "1");
        child.Should().NotHaveAttributeWithValue("id", "2");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => child.Should().HaveAttributeWithValue("id", "2"));
        Assert.AreEqual("Expected child to have attribute 'id' with value '2', but found '1'.", ex.Message);
    }

    [TestMethod]
    public void XAttribute_BeAndNotBe_PassAndFail() {
        var attribute = XElement.Parse(CATALOG).Should().HaveElement("child").Which.Should().HaveAttribute("id").Which;
        attribute.Should().Be(new XAttribute("id", "1"));
        attribute.Should().NotBe(new XAttribute("id", "2"));

        var be = Assert.ThrowsExactly<AssertionFailedException>(() => attribute.Should().Be(new XAttribute("id", "2")));
        Assert.AreEqual("Expected attribute to be id=\"2\", but found id=\"1\".", be.Message);

        var notBe = Assert.ThrowsExactly<AssertionFailedException>(() => attribute.Should().NotBe(new XAttribute("id", "1")));
        Assert.AreEqual("Expected attribute not to be id=\"1\", but found id=\"1\".", notBe.Message);
    }

    [TestMethod]
    public void XElement_ElementOccurrences_PassAndFail() {
        var element = XElement.Parse("<root><child>a</child><child>b</child></root>");
        element.Should().HaveElement("child", Exactly.Twice()).Which.Should().HaveCount(2);
        element.Should().HaveElement("child", AtLeast.Once());
        element.Should().HaveElement("child", Exactly.Times(2));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => element.Should().HaveElement("child", Exactly.Once()));
        Assert.AreEqual("Expected element to have element 'child' exactly once, but found 2.", ex.Message);
    }

    [TestMethod]
    public void XElement_Namespaces_PassAndFail() {
        var element = XElement.Parse("""<root xmlns:part="urn:example"><part:child part:id="1">text</part:child></root>""");
        element.Should().HaveElementWithNamespace("child", "urn:example").Which.Should().HaveValue("text");
        element.Should().HaveElementWithNamespace("child", "urn:example").Which.Should().HaveAttributeWithNamespace("id", "urn:example");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => element.Should().HaveElementWithNamespace("child", "urn:other"));
        Assert.AreEqual("Expected element to have element 'child' in namespace 'urn:other', but it was not found.", ex.Message);
    }

    [TestMethod]
    public void XElement_InnerText_PassAndFail() {
        var element = XElement.Parse("<root>one<child>two</child>three</root>");
        element.Should().HaveInnerText("onetwothree");
        element.Should().NotHaveInnerText("other");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => element.Should().HaveInnerText("other"));
        Assert.AreEqual("Expected element to have inner text 'other', but found 'onetwothree'.", ex.Message);

        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => element.Should().NotHaveInnerText("onetwothree"));
        Assert.AreEqual("Expected element not to have inner text 'onetwothree', but found it.", notEx.Message);
    }

    [TestMethod]
    public void XElement_BeNotBeAndEquivalence() {
        var element = XElement.Parse(CATALOG);
        var same = XElement.Parse(CATALOG);
        var changed = XElement.Parse("""<root><child id="2">text</child></root>""");
        element.Should().Be(same);
        element.Should().NotBe(changed);
        element.Should().BeEquivalentTo(same);
        element.Should().NotBeEquivalentTo(changed);

        var beEx = Assert.ThrowsExactly<AssertionFailedException>(() => element.Should().Be(changed));
        Assert.Contains("Expected element to be", beEx.Message);

        var notBeEx = Assert.ThrowsExactly<AssertionFailedException>(() => element.Should().NotBe(same));
        Assert.Contains("Expected element not to be", notBeEx.Message);

        var notEquivalentEx = Assert.ThrowsExactly<AssertionFailedException>(() => element.Should().NotBeEquivalentTo(same));
        Assert.Contains("not to be equivalent", notEquivalentEx.Message);
    }

    [TestMethod]
    public void XDocument_RootElementOccurrencesAndNamespaces() {
        var doc = XDocument.Parse("""<root xmlns:part="urn:example"><part:child>one</part:child><part:child>two</part:child></root>""");
        doc.Should().HaveRoot(XNamespace.None + "root");
        doc.Should().HaveElementWithNamespace("child", "urn:example");
        doc.Should().HaveElement(XNamespace.Get("urn:example") + "child", Exactly.Twice()).Which.Should().HaveCount(2);
        doc.Should().HaveElementWithValue(XNamespace.Get("urn:example") + "child", "two");
        doc.Should().NotHaveElementWithValue(XNamespace.Get("urn:example") + "child", "three");
        doc.Should().NotHaveElement(XNamespace.Get("urn:other") + "child");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => doc.Should().HaveRoot(XNamespace.Get("urn:example") + "root"));
        Assert.AreEqual("Expected doc to have root '{urn:example}root', but it was not found.", ex.Message);
    }

    [TestMethod]
    public void XDocument_BeNotBeInnerTextAndEquivalence() {
        var doc = XDocument.Parse(CATALOG);
        var same = XDocument.Parse(CATALOG);
        var changed = XDocument.Parse("""<root><child id="2">text</child></root>""");
        doc.Should().Be(same);
        doc.Should().NotBe(changed);
        doc.Should().HaveInnerText("text");
        doc.Should().NotHaveInnerText("other");
        doc.Should().NotBeEquivalentTo(changed);

        var notBeEx = Assert.ThrowsExactly<AssertionFailedException>(() => doc.Should().NotBe(same));
        Assert.Contains("Expected doc not to be", notBeEx.Message);
    }

    [TestMethod]
    public void ClassicXml_NegativesValuesNamespacesAndEquality() {
        var doc = new XmlDocument();
        doc.LoadXml("""<root xmlns:part="urn:example"><part:child part:id="1">text</part:child></root>""");
        doc.Should().NotHaveElement("missing");
        doc.Should().HaveElementWithValue("part:child", "text");
        doc.Should().NotHaveElementWithValue("part:child", "other");
        doc.Should().HaveElementWithNamespace("child", "urn:example");
        doc.Should().HaveElement("part:child").Which.Should().HaveAttributeWithValue("part:id", "1");
        doc.Should().HaveElement("part:child").Which.Should().NotHaveAttributeWithValue("part:id", "2");
        doc.Should().HaveElement("part:child").Which.Should().HaveAttributeWithNamespace("id", "urn:example");
        doc.Should().HaveInnerText("text");
        doc.Should().NotHaveInnerText("other");

        var same = new XmlDocument();
        same.LoadXml("""<root xmlns:part="urn:example"><part:child part:id="1">text</part:child></root>""");
        var changed = new XmlDocument();
        changed.LoadXml("""<root xmlns:part="urn:example"><part:child part:id="2">text</part:child></root>""");
        doc.Should().Be(same);
        doc.Should().NotBe(changed);
        doc.Should().NotBeEquivalentTo(changed);

        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => doc.Should().NotHaveElement("part:child"));
        Assert.AreEqual("Expected doc not to have element 'part:child', but it was found.", notEx.Message);
    }
}

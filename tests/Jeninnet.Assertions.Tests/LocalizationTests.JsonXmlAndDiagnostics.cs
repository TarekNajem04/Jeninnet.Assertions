//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

public partial class LocalizationTests {
    [TestMethod]
    public void Arabic_NearestCandidate() {
        using var _ = new ArabicScope();

        var prices = new Dictionary<string, int> { ["name"] = 1 };
        var typo = Assert.ThrowsExactly<AssertionFailedException>(() => prices.Should().ContainKey("naem"));
        Assert.AreEqual("المتوقع من prices أن يحتوي المفتاح 'naem'، لكنه لم يُعثر عليه. هل تقصد 'name'؟", typo.Message);
    }

    [TestMethod]
    public void Arabic_Json() {
        using var _ = new ArabicScope();

        var node = JsonNode.Parse("{\"name\":\"a\"}")!;
        var beNull = Assert.ThrowsExactly<AssertionFailedException>(() => node.Should().BeNull());
        Assert.AreEqual("المتوقع من node أن يكون null، لكن وُجد {\"name\":\"a\"}.", beNull.Message);

        JsonNode? missing = null;
        var notBeNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotBeNull());
        Assert.AreEqual("المتوقع من missing ألا يكون null، لكن وُجد <null>.", notBeNull.Message);

        var other = JsonNode.Parse("{\"name\":\"b\"}")!;
        var equivalent = Assert.ThrowsExactly<AssertionFailedException>(() => node.Should().BeEquivalentTo(other));
        Assert.AreEqual("المتوقع من node أن يكون مكافئاً، لكن وُجد {\"name\":\"a\"} بدل {\"name\":\"b\"}.", equivalent.Message);

        var numbers = JsonNode.Parse("[1, 2]")!;
        var count = Assert.ThrowsExactly<AssertionFailedException>(() => numbers.Should().HaveCount(1));
        Assert.AreEqual("المتوقع من numbers أن يحتوي 1 من العناصر، لكن وُجد [1,2].", count.Message);

        var name = JsonNode.Parse("{\"a\":1}")!["a"]!;
        var value = Assert.ThrowsExactly<AssertionFailedException>(() => name.Should().HaveValue(2));
        Assert.AreEqual("المتوقع من name أن تكون قيمته 2، لكن وُجد 1.", value.Message);

        var property = Assert.ThrowsExactly<AssertionFailedException>(() => node.Should().HaveProperty("missing"));
        Assert.AreEqual("المتوقع من node أن يحتوي الخاصية 'missing'، لكن وُجد {\"name\":\"a\"}.", property.Message);
    }

    [TestMethod]
    public void Arabic_JsonPaths() {
        using var _ = new ArabicScope();

        var node = JsonNode.Parse("{\"user\":{\"name\":\"ada\",\"id\":1}}")!;
        var missing = Assert.ThrowsExactly<AssertionFailedException>(() => node.Should().HaveJsonPath("$.user.email"));
        Assert.AreEqual("المتوقع من node أن يحتوي المسار '$.user.email'، لكن الخاصية 'email' غير موجودة تحت '$.user' (المتاح: name, id).", missing.Message);

        var array = JsonNode.Parse("[1,2,3]")!;
        var range = Assert.ThrowsExactly<AssertionFailedException>(() => array.Should().HaveJsonPath("$[5]"));
        Assert.AreEqual("المتوقع من array أن يحتوي المسار '$[5]'، لكن الفهرس 5 خارج النطاق تحت '$' (عدد العناصر الحالي 3).", range.Message);

        var notHave = Assert.ThrowsExactly<AssertionFailedException>(() => node.Should().NotHaveJsonPath("$.user"));
        Assert.AreEqual("المتوقع من node ألا يحتوي المسار '$.user'، لكنه موجود.", notHave.Message);
    }

    [TestMethod]
    public void Arabic_Xml() {
        using var _ = new ArabicScope();

        var doc = XDocument.Parse("<root/>");
        var root = Assert.ThrowsExactly<AssertionFailedException>(() => doc.Should().HaveRoot("missing"));
        Assert.AreEqual("المتوقع من doc أن يحتوي الجذر 'missing'، لكنه لم يُعثر عليه.", root.Message);

        var item = XElement.Parse("<item id=\"1\">text</item>");
        var beNull = Assert.ThrowsExactly<AssertionFailedException>(() => item.Should().BeNull());
        Assert.AreEqual("المتوقع من item أن يكون null، لكنه لم يكن null.", beNull.Message);

        var element = Assert.ThrowsExactly<AssertionFailedException>(() => item.Should().HaveElement("missing"));
        Assert.AreEqual("المتوقع من item أن يحتوي العنصر 'missing'، لكنه لم يُعثر عليه.", element.Message);

        var attribute = Assert.ThrowsExactly<AssertionFailedException>(() => item.Should().HaveAttribute("missing"));
        Assert.AreEqual("المتوقع من item أن يحتوي السمة 'missing'، لكنه لم يُعثر عليها.", attribute.Message);

        var attributeValue = Assert.ThrowsExactly<AssertionFailedException>(() => item.Should().HaveAttributeWithValue("id", "2"));
        Assert.AreEqual("المتوقع من item أن تحتوي السمة 'id' القيمة '2'، لكن وُجد '1'.", attributeValue.Message);

        var value = Assert.ThrowsExactly<AssertionFailedException>(() => item.Should().HaveValue("other"));
        Assert.AreEqual("المتوقع من item أن تكون قيمته 'other'، لكن وُجد 'text'.", value.Message);

        var equivalent = Assert.ThrowsExactly<AssertionFailedException>(() => item.Should().BeEquivalentTo(XElement.Parse("<item>other</item>")));
        Assert.AreEqual("المتوقع من item أن يكون مكافئاً، لكن وُجد <item id=\"1\">text</item> بدل <item>other</item>.", equivalent.Message);

        XElement? missingElement = null;
        var notBeNull = Assert.ThrowsExactly<AssertionFailedException>(() => missingElement.Should().NotBeNull());
        Assert.AreEqual("المتوقع من missingElement ألا يكون null، لكن وُجد null.", notBeNull.Message);

        var xml = new XmlDocument();
        xml.LoadXml("<root/>");
        var xmlElement = Assert.ThrowsExactly<AssertionFailedException>(() => xml.Should().HaveElement("missing"));
        Assert.AreEqual("المتوقع من xml أن يحتوي العنصر 'missing'، لكنه لم يُعثر عليه.", xmlElement.Message);

        var attr = new XAttribute("id", "1");
        var attrValue = Assert.ThrowsExactly<AssertionFailedException>(() => attr.Should().HaveValue("2"));
        Assert.AreEqual("المتوقع من attr أن تكون قيمته '2'، لكن وُجد '1'.", attrValue.Message);
    }

    [TestMethod]
    public void Arabic_CustomComparersAndSerialization() {
        using var _ = new ArabicScope();

        object fruit = "pear";
        var oneOf = Assert.ThrowsExactly<AssertionFailedException>(() => fruit.Should().BeOneOf(["apple", "plum"], StringComparer.OrdinalIgnoreCase));
        Assert.AreEqual("المتوقع من fruit أن يكون واحداً من apple, plum، لكن وُجد pear.", oneOf.Message);

        var lossy = new { X = 1, Y = (object)2 };
        var json = Assert.ThrowsExactly<AssertionFailedException>(() => lossy.Should().BeJsonSerializable());
        Assert.Contains("المتوقع من lossy أن يكون قابلاً للتسلسل عبر JSON", json.Message);

        var spanMessage = Assert.ThrowsExactly<AssertionFailedException>(() => "hello".AsSpan().Should().Be("world"));
        Assert.AreEqual("المتوقع من \"hello\".AsSpan() أن يكون 'world'، لكن وُجد 'hello'.", spanMessage.Message);

        Action instant = () => { };
        var close = Assert.ThrowsExactly<AssertionFailedException>(
            () => instant.ExecutionTime().Should().BeCloseTo(TimeSpan.FromHours(1), TimeSpan.FromSeconds(1)));
        Assert.Contains("المتوقع من زمن التنفيذ أن يكون قريباً من", close.Message);
    }

    [TestMethod]
    public void Arabic_XmlAttributeEquality() {
        using var _ = new ArabicScope();

        var attribute = new XAttribute("id", "1");
        var be = Assert.ThrowsExactly<AssertionFailedException>(() => attribute.Should().Be(new XAttribute("id", "2")));
        Assert.AreEqual("المتوقع من attribute أن يكون id=\"2\"، لكن وُجد id=\"1\".", be.Message);

        var notBe = Assert.ThrowsExactly<AssertionFailedException>(() => attribute.Should().NotBe(new XAttribute("id", "1")));
        Assert.AreEqual("المتوقع من attribute ألا يكون id=\"1\"، لكن وُجد id=\"1\".", notBe.Message);
    }
}

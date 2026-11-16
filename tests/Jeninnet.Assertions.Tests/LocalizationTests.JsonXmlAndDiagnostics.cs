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
        var typo = Throws<AssertionFailedException>(() => prices.Should().ContainKey("naem"));
        typo.Message.Should().Be("المتوقع من prices أن يحتوي المفتاح 'naem'، لكنه لم يُعثر عليه. هل تقصد 'name'؟");
    }

    [TestMethod]
    public void Arabic_Json() {
        using var _ = new ArabicScope();

        var node = JsonNode.Parse("{\"name\":\"a\"}")!;
        var beNull = Throws<AssertionFailedException>(() => node.Should().BeNull());
        beNull.Message.Should().Be("المتوقع من node أن يكون null، لكن وُجد {\"name\":\"a\"}.");

        JsonNode? missing = null;
        var notBeNull = Throws<AssertionFailedException>(() => missing.Should().NotBeNull());
        notBeNull.Message.Should().Be("المتوقع من missing ألا يكون null، لكن وُجد <null>.");

        var other = JsonNode.Parse("{\"name\":\"b\"}")!;
        var equivalent = Throws<AssertionFailedException>(() => node.Should().BeEquivalentTo(other));
        equivalent.Message.Should().Be("المتوقع من node أن يكون مكافئاً، لكن وُجد {\"name\":\"a\"} بدل {\"name\":\"b\"}.");

        var numbers = JsonNode.Parse("[1, 2]")!;
        var count = Throws<AssertionFailedException>(() => numbers.Should().HaveCount(1));
        count.Message.Should().Be("المتوقع من numbers أن يحتوي 1 من العناصر، لكن وُجد [1,2].");

        var name = JsonNode.Parse("{\"a\":1}")!["a"]!;
        var value = Throws<AssertionFailedException>(() => name.Should().HaveValue(2));
        value.Message.Should().Be("المتوقع من name أن تكون قيمته 2، لكن وُجد 1.");

        var property = Throws<AssertionFailedException>(() => node.Should().HaveProperty("missing"));
        property.Message.Should().Be("المتوقع من node أن يحتوي الخاصية 'missing'، لكن وُجد {\"name\":\"a\"}.");
    }

    [TestMethod]
    public void Arabic_JsonPaths() {
        using var _ = new ArabicScope();

        var node = JsonNode.Parse("{\"user\":{\"name\":\"ada\",\"id\":1}}")!;
        var missing = Throws<AssertionFailedException>(() => node.Should().HaveJsonPath("$.user.email"));
        missing.Message.Should().Be("المتوقع من node أن يحتوي المسار '$.user.email'، لكن الخاصية 'email' غير موجودة تحت '$.user' (المتاح: name, id).");

        var array = JsonNode.Parse("[1,2,3]")!;
        var range = Throws<AssertionFailedException>(() => array.Should().HaveJsonPath("$[5]"));
        range.Message.Should().Be("المتوقع من array أن يحتوي المسار '$[5]'، لكن الفهرس 5 خارج النطاق تحت '$' (عدد العناصر الحالي 3).");

        var notHave = Throws<AssertionFailedException>(() => node.Should().NotHaveJsonPath("$.user"));
        notHave.Message.Should().Be("المتوقع من node ألا يحتوي المسار '$.user'، لكنه موجود.");
    }

    [TestMethod]
    public void Arabic_JsonElementAndPathV2() {
        using var _ = new ArabicScope();

        using var arrayDocument = JsonDocument.Parse("[1,2]");
        var element = arrayDocument.RootElement.Clone();
        var beObject = Throws<AssertionFailedException>(() => element.Should().BeObject());
        beObject.Message.Should().Be("المتوقع من element أن يكون كائن JSON، لكن وُجد [1,2].");

        var document = JsonDocument.Parse("{}");
        document.Dispose();
        var disposed = Throws<AssertionFailedException>(() => document.Should());
        disposed.Message.Should().Be("المتوقع من document أن يكون مستند JSON حياً، لكنه كان مفكوكاً.");

        var node = JsonNode.Parse("""{"users":[{"name":"ada"},{"name":"grace"}]}""")!;
        var multiple = Throws<AssertionFailedException>(() => node.Should().HaveJsonPath("$.users[*].name"));
        multiple.Message.Should().Be("المتوقع من node أن يحل المسار '$.users[*].name' إلى قيمة واحدة، لكنه أنتج 2 قيمة. استخدم HaveJsonPathValues للنتائج المتعددة.");

        var noMatch = Throws<AssertionFailedException>(() => node.Should().HaveJsonPathValues("$.users[*].missing"));
        noMatch.Message.Should().Be("المتوقع من node أن يحل المسار '$.users[*].missing'، لكن لم تُطابق أي قيمة.");
    }

    [TestMethod]
    public void Arabic_Xml() {
        using var _ = new ArabicScope();

        var doc = XDocument.Parse("<root/>");
        var root = Throws<AssertionFailedException>(() => doc.Should().HaveRoot("missing"));
        root.Message.Should().Be("المتوقع من doc أن يحتوي الجذر 'missing'، لكنه لم يُعثر عليه.");

        var item = XElement.Parse("<item id=\"1\">text</item>");
        var beNull = Throws<AssertionFailedException>(() => item.Should().BeNull());
        beNull.Message.Should().Be("المتوقع من item أن يكون null، لكنه لم يكن null.");

        var element = Throws<AssertionFailedException>(() => item.Should().HaveElement("missing"));
        element.Message.Should().Be("المتوقع من item أن يحتوي العنصر 'missing'، لكنه لم يُعثر عليه.");

        var attribute = Throws<AssertionFailedException>(() => item.Should().HaveAttribute("missing"));
        attribute.Message.Should().Be("المتوقع من item أن يحتوي السمة 'missing'، لكنه لم يُعثر عليها.");

        var attributeValue = Throws<AssertionFailedException>(() => item.Should().HaveAttributeWithValue("id", "2"));
        attributeValue.Message.Should().Be("المتوقع من item أن تحتوي السمة 'id' القيمة '2'، لكن وُجد '1'.");

        var value = Throws<AssertionFailedException>(() => item.Should().HaveValue("other"));
        value.Message.Should().Be("المتوقع من item أن تكون قيمته 'other'، لكن وُجد 'text'.");

        var equivalent = Throws<AssertionFailedException>(() => item.Should().BeEquivalentTo(XElement.Parse("<item>other</item>")));
        equivalent.Message.Should().Be("المتوقع أن يكون item مكافئًا للـ XML المتوقع، لكن وُجد 2 اختلاف/اختلافات: 1. /item/@id: المتوقع <مفقود>, وُجد '1'; 2. /item/text(): المتوقع 'other', وُجد 'text'");

        XElement? missingElement = null;
        var notBeNull = Throws<AssertionFailedException>(() => missingElement.Should().NotBeNull());
        notBeNull.Message.Should().Be("المتوقع من missingElement ألا يكون null، لكن وُجد null.");

        var xml = new XmlDocument();
        xml.LoadXml("<root/>");
        var xmlElement = Throws<AssertionFailedException>(() => xml.Should().HaveElement("missing"));
        xmlElement.Message.Should().Be("المتوقع من xml أن يحتوي العنصر 'missing'، لكنه لم يُعثر عليه.");

        var attr = new XAttribute("id", "1");
        var attrValue = Throws<AssertionFailedException>(() => attr.Should().HaveValue("2"));
        attrValue.Message.Should().Be("المتوقع من attr أن تكون قيمته '2'، لكن وُجد '1'.");
    }

    [TestMethod]
    public void Arabic_CustomComparersAndSerialization() {
        using var _ = new ArabicScope();

        object fruit = "pear";
        var oneOf = Throws<AssertionFailedException>(() => fruit.Should().BeOneOf(["apple", "plum"], StringComparer.OrdinalIgnoreCase));
        oneOf.Message.Should().Be("المتوقع من fruit أن يكون واحداً من apple, plum، لكن وُجد pear.");

        var lossy = new { X = 1, Y = (object)2 };
        var json = Throws<AssertionFailedException>(() => lossy.Should().BeJsonSerializable());
        json.Message.Should().Contain("المتوقع من lossy أن يكون قابلاً للتسلسل عبر JSON");

        var spanMessage = Throws<AssertionFailedException>(() => "hello".AsSpan().Should().Be("world"));
        spanMessage.Message.Should().Be("المتوقع من \"hello\".AsSpan() أن يكون 'world'، لكن وُجد 'hello'.");

        Action instant = () => { };
        var close = Throws<AssertionFailedException>(
            () => instant.ExecutionTime().Should().BeCloseTo(TimeSpan.FromHours(1), TimeSpan.FromSeconds(1)));
        close.Message.Should().Contain("المتوقع من زمن التنفيذ أن يكون قريباً من");
    }

    [TestMethod]
    public void Arabic_XmlAttributeEquality() {
        using var _ = new ArabicScope();

        var attribute = new XAttribute("id", "1");
        var be = Throws<AssertionFailedException>(() => attribute.Should().Be(new XAttribute("id", "2")));
        be.Message.Should().Be("المتوقع من attribute أن يكون id=\"2\"، لكن وُجد id=\"1\".");

        var notBe = Throws<AssertionFailedException>(() => attribute.Should().NotBe(new XAttribute("id", "1")));
        notBe.Message.Should().Be("المتوقع من attribute ألا يكون id=\"1\"، لكن وُجد id=\"1\".");
    }

    /// <summary>Asserts the exact exception type through the library and returns the caught exception.</summary>
    /// <typeparam name="TException">The expected exception type.</typeparam>
    /// <param name="action">The action expected to throw.</param>
    /// <returns>The caught exception.</returns>
    private static TException Throws<TException>(Action action) where TException : Exception =>
        action.Should().ThrowExactly<TException>().Which;
}

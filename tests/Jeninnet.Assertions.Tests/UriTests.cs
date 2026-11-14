//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class UriTests {
    [TestMethod]
    public void Be_PassForCanonicalEquivalentUris() {
        var uri = new Uri("HTTPS://Example.COM:443/path");
        uri.Should().Be(new Uri("https://example.com/path"));
    }

    [TestMethod]
    public void Be_FailsForDifferentUri() {
        var uri = new Uri("https://example.com/path");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => uri.Should().Be(new Uri("https://example.com/other")));
        Assert.AreEqual("Expected uri to be https://example.com/other, but found https://example.com/path.", ex.Message);
    }

    [TestMethod]
    public void NotBe_FailsForCanonicalEquivalentUris() {
        var uri = new Uri("https://example.com/path");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => uri.Should().NotBe(new Uri("https://example.com/path")));
        Assert.AreEqual("Expected uri not to be https://example.com/path, but found https://example.com/path.", ex.Message);
    }

    [TestMethod]
    public void BeNull_And_NotBeNull() {
        Uri? missing = null;
        missing.Should().BeNull();
        missing.Should().Be(null);

        var uri = new Uri("https://example.com");
        uri.Should().NotBeNull();
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => uri.Should().BeNull());
        Assert.AreEqual("Expected uri to be null, but found https://example.com.", ex.Message);
    }

    [TestMethod]
    public void BeAbsolute_And_BeRelative() {
        new Uri("https://example.com").Should().BeAbsolute();
        new Uri("/orders", UriKind.Relative).Should().BeRelative();

        var absolute = new Uri("https://example.com");
        var relative = new Uri("/orders", UriKind.Relative);
        var absoluteEx = Assert.ThrowsExactly<AssertionFailedException>(() => relative.Should().BeAbsolute());
        Assert.AreEqual("Expected relative to be absolute, but it is relative.", absoluteEx.Message);

        var relativeEx = Assert.ThrowsExactly<AssertionFailedException>(() => absolute.Should().BeRelative());
        Assert.AreEqual("Expected absolute to be relative, but it is absolute.", relativeEx.Message);
    }

    [TestMethod]
    public void HaveScheme_IsCaseInsensitive_AndReportsActual() {
        var uri = new Uri("https://example.com");
        uri.Should().HaveScheme("HTTPS");
        uri.Should().NotHaveScheme("http");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => uri.Should().HaveScheme("http"));
        Assert.AreEqual("Expected uri to have scheme 'http', but found 'https'.", ex.Message);
    }

    [TestMethod]
    public void HaveHost_IsCaseInsensitive_AndAcceptsIpv6Brackets() {
        var uri = new Uri("https://EXAMPLE.com/path");
        uri.Should().HaveHost("example.COM");

        var ipv6 = new Uri("https://[::1]:8443/");
        ipv6.Should().HaveHost("::1").And.HaveHost("[::1]");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => uri.Should().HaveHost("other.example"));
        Assert.AreEqual("Expected uri to have host 'other.example', but found 'example.com'.", ex.Message);
    }

    [TestMethod]
    public void HavePort_UsesSchemeDefaultsAndExplicitPorts() {
        new Uri("https://example.com").Should().HavePort(443);
        new Uri("http://example.com").Should().HavePort(80);
        new Uri("https://example.com:8443").Should().HavePort(8443);

        var uri = new Uri("https://example.com");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => uri.Should().HavePort(80));
        Assert.AreEqual("Expected uri to have port 80, but found 443.", ex.Message);
    }

    [TestMethod]
    public void HavePath_NormalizesLeadingSlashAndReportsActual() {
        var uri = new Uri("https://example.com/orders/42");
        uri.Should().HavePath("/orders/42");
        uri.Should().HavePath("orders/42");

        new Uri("https://example.com").Should().HavePath("/");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => uri.Should().HavePath("/orders/43"));
        Assert.AreEqual("Expected uri to have path '/orders/43', but found '/orders/42'.", ex.Message);
    }

    [TestMethod]
    public void HaveFragment_AcceptsOptionalHashAndReportsActual() {
        var uri = new Uri("https://example.com/page#top");
        uri.Should().HaveFragment("top");
        uri.Should().HaveFragment("#top");
        uri.Should().NotHaveFragment("bottom");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => uri.Should().HaveFragment("bottom"));
        Assert.AreEqual("Expected uri to have fragment 'bottom', but found 'top'.", ex.Message);
    }

    [TestMethod]
    public void HaveQueryParameter_ExposesFirstValueAndSupportsDuplicates() {
        var uri = new Uri("https://example.com/orders?page=2&sort=desc&tag=a&tag=b");
        uri.Should().HaveQueryParameter("page").Which.Should().Be("2");
        uri.Should().HaveQueryParameterValue("page", "2");
        uri.Should().HaveQueryParameterValue("tag", "b");

        var first = uri.Should().HaveQueryParameter("tag");
        Assert.AreEqual("a", first.Which);
    }

    [TestMethod]
    public void HaveQueryParameter_UnescapesNamesAndValues() {
        var uri = new Uri("https://example.com/search?q=hello%20world&plus=a+b");
        uri.Should().HaveQueryParameterValue("q", "hello world");
        uri.Should().HaveQueryParameterValue("plus", "a b");
    }

    [TestMethod]
    public void HaveQueryParameter_MissingParameterSuggestsNearestName() {
        var uri = new Uri("https://example.com/orders?name=value");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => uri.Should().HaveQueryParameter("nmae"));
        Assert.Contains("available: name", ex.Message);
        Assert.Contains("Did you mean 'name'?", ex.Message);
    }

    [TestMethod]
    public void HaveQueryParameter_WrongValueReportsActualValues() {
        var uri = new Uri("https://example.com/orders?page=2&page=3");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => uri.Should().HaveQueryParameterValue("page", "4"));
        Assert.AreEqual("Expected uri to have query parameter 'page' with value '4', but found '2, 3'.", ex.Message);
    }

    [TestMethod]
    public void NotHaveQueryParameter_FailsWhenPresent() {
        var uri = new Uri("https://example.com/orders?page=2");
        uri.Should().NotHaveQueryParameter("missing");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => uri.Should().NotHaveQueryParameter("page"));
        Assert.AreEqual("Expected uri not to have query parameter 'page', but found '2'.", ex.Message);
    }

    [TestMethod]
    public void BeBaseOf_RequiresPathSegmentBoundary() {
        var baseUri = new Uri("https://example.com/orders");
        baseUri.Should().BeBaseOf(new Uri("https://example.com/orders/42"));
        baseUri.Should().BeBaseOf(new Uri("https://example.com:443/orders/42"));
        baseUri.Should().BeBaseOf(new Uri("https://example.com/orders"));
        baseUri.Should().NotBeBaseOf(new Uri("https://example.com/orders-archive"));
        baseUri.Should().NotBeBaseOf(new Uri("http://example.com/orders/42"));
        baseUri.Should().NotBeBaseOf(new Uri("https://example.com:8443/orders/42"));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => baseUri.Should().BeBaseOf(new Uri("https://example.com/orders-archive")));
        Assert.AreEqual("Expected baseUri to be a base of https://example.com/orders-archive, but it was not.", ex.Message);
    }

    [TestMethod]
    public void BeBaseOf_RejectsRelativeUris() {
        var absolute = new Uri("https://example.com/orders");
        var relative = new Uri("/orders", UriKind.Relative);

        var subjectEx = Assert.ThrowsExactly<AssertionFailedException>(() => relative.Should().BeBaseOf(absolute));
        Assert.AreEqual("Expected relative to be an absolute URI, but it is relative.", subjectEx.Message);

        var expectedEx = Assert.ThrowsExactly<AssertionFailedException>(() => absolute.Should().BeBaseOf(relative));
        Assert.AreEqual("Expected /orders to be an absolute base URI, but it is relative.", expectedEx.Message);
    }

    [TestMethod]
    public void RelativeUri_ComponentAssertionExplainsAbsoluteRequirement() {
        var relative = new Uri("/orders", UriKind.Relative);
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => relative.Should().HaveScheme("https"));
        Assert.AreEqual("Expected relative to be an absolute URI, but it is relative.", ex.Message);
    }

    [TestMethod]
    public void ComponentAssertion_FailsForNullUri() {
        Uri? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveScheme("https"));
        Assert.AreEqual("Expected missing to be a URI, but found <null>.", ex.Message);
    }

    [TestMethod]
    public void BecauseClause_IsAppended() {
        var uri = new Uri("https://example.com");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => uri.Should().HaveHost("other", "because the host changed"));
        Assert.Contains("because the host changed", ex.Message);
    }

    [TestMethod]
    public void ArabicMessages_AreLocalized() {
        using(AssertionIsolation.Isolate()) {
            AssertionCulture.Current = new CultureInfo("ar");
            var uri = new Uri("https://example.com");
            var ex = Assert.ThrowsExactly<AssertionFailedException>(() => uri.Should().HaveScheme("http"));
            Assert.Contains("المتوقع", ex.Message);
        }
    }
}

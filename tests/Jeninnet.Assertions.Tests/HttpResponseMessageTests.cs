//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class HttpResponseMessageTests {
    [TestMethod]
    public void HaveStatusCode_PassAndFail() {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Should().HaveStatusCode(HttpStatusCode.OK);

        using var missing = new HttpResponseMessage(HttpStatusCode.NotFound);
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveStatusCode(HttpStatusCode.OK));
        Assert.AreEqual("Expected missing to have status code OK, but found NotFound.", ex.Message);
    }

    [TestMethod]
    public void HaveStatusCode_FailsForNull() {
        HttpResponseMessage? response = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => response.Should().HaveStatusCode(HttpStatusCode.OK));
        Assert.AreEqual("Expected response to have status code OK, but response is null.", ex.Message);
    }

    [TestMethod]
    public void BeSuccessful_PassAndFail() {
        using var ok = new HttpResponseMessage(HttpStatusCode.OK);
        ok.Should().BeSuccessful();

        using var created = new HttpResponseMessage(HttpStatusCode.Created);
        created.Should().BeSuccessful();

        using var missing = new HttpResponseMessage(HttpStatusCode.NotFound);
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeSuccessful());
        Assert.AreEqual("Expected missing to be successful, but found NotFound.", ex.Message);
    }

    [TestMethod]
    public void BeSuccessful_FailsForNull() {
        HttpResponseMessage? response = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => response.Should().BeSuccessful());
        Assert.AreEqual("Expected response to be successful, but response is null.", ex.Message);
    }

    [TestMethod]
    public void HaveHeader_PassAndFail() {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Headers.Add("X-Test", "yes");
        response.Should().HaveHeader("X-Test").And.HaveHeader("X-Test", "yes");

        var value = Assert.ThrowsExactly<AssertionFailedException>(() => response.Should().HaveHeader("X-Test", "no"));
        Assert.AreEqual("Expected response to have header 'X-Test' with value 'no', but found 'yes'.", value.Message);

        var absent = Assert.ThrowsExactly<AssertionFailedException>(() => response.Should().HaveHeader("X-Missing"));
        Assert.AreEqual("Expected response to have header 'X-Missing', but it was not found.", absent.Message);
    }

    [TestMethod]
    public void HaveHeader_FindsContentHeaders() {
        using var response = new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent("hi", Encoding.UTF8, "application/json"),
        };
        response.Should().HaveHeader("Content-Type", "application/json; charset=utf-8");
    }

    [TestMethod]
    public void HaveHeader_FailsForNull() {
        HttpResponseMessage? response = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => response.Should().HaveHeader("X-Test"));
        Assert.AreEqual("Expected response to have header 'X-Test', but response is null.", ex.Message);
    }

    [TestMethod]
    public void HaveHeader_ThrowsForNullName() {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        Assert.ThrowsExactly<ArgumentNullException>(() => response.Should().HaveHeader(null!));
    }

    [TestMethod]
    public void HaveHeader_NullValue_ChecksPresenceOnly() {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Headers.Add("X-Test", "yes");
        response.Should().HaveHeader("X-Test", null);

        var absent = Assert.ThrowsExactly<AssertionFailedException>(() => response.Should().HaveHeader("X-Missing", null));
        Assert.AreEqual("Expected response to have header 'X-Missing', but it was not found.", absent.Message);
    }
}

//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class HttpResponseMessageTests {
    private sealed class UnknownLengthContent : HttpContent {
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) => Task.CompletedTask;

        protected override bool TryComputeLength(out long length) {
            length = 0;
            return false;
        }
    }

    [TestMethod]
    public void BeNull_NotBeNull() {
        HttpResponseMessage? missing = null;
        missing.Should().BeNull();
        Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotBeNull());

        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Should().NotBeNull();
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => response.Should().BeNull());
        Assert.AreEqual("Expected response to be null, but found a response.", ex.Message);
    }

    [TestMethod]
    public void StatusSugar_PassAndFail() {
        using var redirect = new HttpResponseMessage(HttpStatusCode.Redirect);
        redirect.Should().BeRedirect();
        Assert.ThrowsExactly<AssertionFailedException>(() => redirect.Should().NotBeRedirect());

        using var badRequest = new HttpResponseMessage(HttpStatusCode.BadRequest);
        badRequest.Should().BeClientError();

        using var serverError = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        serverError.Should().BeServerError();

        using var noContent = new HttpResponseMessage(HttpStatusCode.NoContent);
        noContent.Should().BeNoContent();

        using var ok = new HttpResponseMessage(HttpStatusCode.OK);
        var client = Assert.ThrowsExactly<AssertionFailedException>(() => ok.Should().BeClientError());
        Assert.AreEqual("Expected ok to be a client error (4xx), but found OK.", client.Message);

        var noContentEx = Assert.ThrowsExactly<AssertionFailedException>(() => ok.Should().BeNoContent());
        Assert.AreEqual("Expected ok to have no content (204), but found OK.", noContentEx.Message);
    }

    [TestMethod]
    public void ContentTypeAndLength_PassAndFail() {
        using var response = new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent("abc", Encoding.UTF8, "application/json"),
        };

        response.Should().HaveContentType("application/json")
            .And.HaveContentType("application/json", "utf-8")
            .And.HaveContentLength(3);

        var contentType = Assert.ThrowsExactly<AssertionFailedException>(() => response.Should().HaveContentType("text/plain"));
        Assert.Contains("to have content type 'text/plain'", contentType.Message);

        var charset = Assert.ThrowsExactly<AssertionFailedException>(() => response.Should().HaveContentType("application/json", "ascii"));
        Assert.Contains("charset=ascii", charset.Message);

        var length = Assert.ThrowsExactly<AssertionFailedException>(() => response.Should().HaveContentLength(4));
        Assert.AreEqual("Expected response to have content length 4, but found 3.", length.Message);

        using var unknown = new HttpResponseMessage(HttpStatusCode.OK) { Content = new UnknownLengthContent() };
        var missing = Assert.ThrowsExactly<AssertionFailedException>(() => unknown.Should().HaveContentLength(1));
        Assert.AreEqual("Expected unknown to have content length 1, but no content length was present.", missing.Message);

        var missingType = Assert.ThrowsExactly<AssertionFailedException>(() => unknown.Should().HaveContentType("application/json"));
        Assert.AreEqual("Expected unknown to have content type 'application/json', but no content type was present.", missingType.Message);

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => response.Should().HaveContentLength(-1));
    }

    [TestMethod]
    public void VersionReasonETagAndLocation_PassAndFail() {
        using var response = new HttpResponseMessage(HttpStatusCode.OK) {
            Version = HttpVersion.Version20,
            ReasonPhrase = "Fine",
        };
        response.Headers.ETag = new EntityTagHeaderValue("\"abc\"");
        response.Headers.Location = new Uri("/next", UriKind.Relative);

        response.Should().HaveVersion(HttpVersion.Version20)
            .And.HaveReasonPhrase("Fine")
            .And.HaveETag("abc")
            .And.HaveETag("\"abc\"")
            .And.HaveLocation("/next");

        var version = Assert.ThrowsExactly<AssertionFailedException>(() => response.Should().HaveVersion(HttpVersion.Version11));
        Assert.Contains("to use HTTP version 1.1", version.Message);

        var reason = Assert.ThrowsExactly<AssertionFailedException>(() => response.Should().HaveReasonPhrase("OK"));
        Assert.Contains("reason phrase 'OK'", reason.Message);

        var etag = Assert.ThrowsExactly<AssertionFailedException>(() => response.Should().HaveETag("other"));
        Assert.Contains("ETag 'other'", etag.Message);

        var location = Assert.ThrowsExactly<AssertionFailedException>(() => response.Should().HaveLocation("/other"));
        Assert.Contains("Location '/other'", location.Message);

        Assert.ThrowsExactly<ArgumentException>(() => response.Should().HaveETag("not a tag\""));
        Assert.ThrowsExactly<ArgumentNullException>(() => response.Should().HaveVersion(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => response.Should().HaveReasonPhrase(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => response.Should().HaveETag(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => response.Should().HaveLocation(null!));
    }

    [TestMethod]
    public void RetryAfter_SecondsAndDate() {
        using var seconds = new HttpResponseMessage(HttpStatusCode.TooManyRequests);
        seconds.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(30));
        seconds.Should().HaveRetryAfter(TimeSpan.FromSeconds(30));

        var secondsEx = Assert.ThrowsExactly<AssertionFailedException>(() => seconds.Should().HaveRetryAfter(TimeSpan.FromSeconds(10)));
        Assert.Contains("Retry-After '00:00:10'", secondsEx.Message);

        using var date = new HttpResponseMessage(HttpStatusCode.TooManyRequests);
        date.Headers.RetryAfter = new RetryConditionHeaderValue(DateTimeOffset.UtcNow.AddSeconds(30));
        date.Should().HaveRetryAfter(TimeSpan.FromSeconds(30));

        using var missing = new HttpResponseMessage(HttpStatusCode.TooManyRequests);
        var missingEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveRetryAfter(TimeSpan.FromSeconds(1)));
        Assert.AreEqual("Expected missing to have Retry-After '00:00:01', but no Retry-After was present.", missingEx.Message);

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => seconds.Should().HaveRetryAfter(TimeSpan.FromSeconds(-1)));
    }

    [TestMethod]
    public void HaveCookie_PassAndFail() {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Headers.TryAddWithoutValidation("Set-Cookie", "session=abc; Path=/; HttpOnly");
        response.Headers.TryAddWithoutValidation("Set-Cookie", "theme=\"dark\"; Path=/");

        response.Should().HaveCookie("session")
            .And.HaveCookie("session", "abc")
            .And.HaveCookie("theme", "dark");

        var missing = Assert.ThrowsExactly<AssertionFailedException>(() => response.Should().HaveCookie("missing"));
        Assert.AreEqual("Expected response to set cookie 'missing', but it was not found.", missing.Message);

        var wrongValue = Assert.ThrowsExactly<AssertionFailedException>(() => response.Should().HaveCookie("session", "other"));
        Assert.AreEqual("Expected response to set cookie 'session' with value 'other', but found 'abc'.", wrongValue.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => response.Should().HaveCookie(null!));
    }

    [TestMethod]
    public void NewActions_FailForNullResponse() {
        HttpResponseMessage? missing = null;

        var contentType = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveContentType("application/json"));
        Assert.AreEqual("Expected missing to have content type 'application/json', but response is null.", contentType.Message);

        var length = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveContentLength(1));
        Assert.AreEqual("Expected missing to have content length 1, but response is null.", length.Message);

        var redirect = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeRedirect());
        Assert.AreEqual("Expected missing to be a redirect (3xx), but response is null.", redirect.Message);

        var cookie = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveCookie("session"));
        Assert.AreEqual("Expected missing to set cookie 'session', but response is null.", cookie.Message);
    }

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

//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class HttpResponseMessageContentTests {
    private sealed class CancellableContent : HttpContent {
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) => Task.CompletedTask;

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken) =>
            Task.Delay(Timeout.Infinite, cancellationToken);

        protected override bool TryComputeLength(out long length) {
            length = 0;
            return false;
        }
    }

    [TestMethod]
    public async Task HaveStringContentAsync_PassAndFail() {
        using var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("hello") };

        var which = await response.Should().HaveStringContentAsync("hello");
        Assert.AreEqual("hello", which.Which);

        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => response.Should().HaveStringContentAsync("bye"));
        Assert.AreEqual("Expected response to have string content 'bye', but found 'hello'.", ex.Message);

        await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => response.Should().HaveStringContentAsync((string)null!));
    }

    [TestMethod]
    public async Task HaveStringContentAsync_PredicatePassAndFail() {
        using var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("hello") };

        var which = await response.Should().HaveStringContentAsync(static text => text.StartsWith("he", StringComparison.Ordinal));
        Assert.AreEqual("hello", which.Which);

        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => response.Should().HaveStringContentAsync(static text => text.Length > 10));
        Assert.AreEqual("Expected response to have string content matching the predicate, but found 'hello'.", ex.Message);

        await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => response.Should().HaveStringContentAsync((Func<string, bool>)null!));
    }

    [TestMethod]
    public async Task HaveStringContentAsync_HonorsCancellation() {
        using var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new CancellableContent() };

        await Assert.ThrowsExactlyAsync<TaskCanceledException>(
            () => response.Should().HaveStringContentAsync("never", cancellationToken: new CancellationToken(canceled: true)));
    }

    [TestMethod]
    public async Task HaveJsonContentAsync_PassAndFail() {
        using var response = new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent("""{"a":1}""", Encoding.UTF8, "application/json"),
        };

        var which = await response.Should().HaveJsonContentAsync();
        which.Which.Should().HaveProperty("a").Which.Should().HaveValue(1);

        using var invalid = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("not json") };
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => invalid.Should().HaveJsonContentAsync());
        Assert.Contains("to have valid JSON content, but parsing failed", ex.Message);
        Assert.Contains("not json", ex.Message);
    }

    [TestMethod]
    public async Task BeProblemDetailsAsync_PassForProblemJson() {
        using var response = new HttpResponseMessage(HttpStatusCode.BadRequest) {
            Content = new StringContent(
                """{"type":"https://example.test/errors/bad","title":"Bad request","status":400,"detail":"Missing id"}""",
                Encoding.UTF8,
                "application/problem+json"),
        };

        await response.Should().BeProblemDetailsAsync(title: "Bad request", status: 400, detail: "Missing id");
        await response.Should().BeProblemDetailsAsync();
    }

    [TestMethod]
    public async Task BeProblemDetailsAsync_AcceptsJsonContentType() {
        using var response = new HttpResponseMessage(HttpStatusCode.BadRequest) {
            Content = new StringContent("""{"title":"Bad request","status":400}""", Encoding.UTF8, "application/json"),
        };

        await response.Should().BeProblemDetailsAsync(title: "Bad request", status: 400);
    }

    [TestMethod]
    public async Task BeProblemDetailsAsync_FailsForWrongContentType() {
        using var response = new HttpResponseMessage(HttpStatusCode.BadRequest) {
            Content = new StringContent("""{"title":"Bad request"}""", Encoding.UTF8, "text/plain"),
        };

        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => response.Should().BeProblemDetailsAsync());
        Assert.Contains("to be ProblemDetails with content type", ex.Message);
        Assert.Contains("text/plain", ex.Message);
    }

    [TestMethod]
    public async Task BeProblemDetailsAsync_FailsForNormalJsonShape() {
        using var response = new HttpResponseMessage(HttpStatusCode.BadRequest) {
            Content = new StringContent("""{"foo":"bar"}""", Encoding.UTF8, "application/json"),
        };

        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => response.Should().BeProblemDetailsAsync(title: "Bad request"));
        Assert.AreEqual("Expected response to have ProblemDetails field 'title' with value 'Bad request', but found '<missing>'.", ex.Message);
    }

    [TestMethod]
    public async Task BeProblemDetailsAsync_FailsForNonObject() {
        using var response = new HttpResponseMessage(HttpStatusCode.BadRequest) {
            Content = new StringContent("[]", Encoding.UTF8, "application/problem+json"),
        };

        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => response.Should().BeProblemDetailsAsync());
        Assert.AreEqual("Expected response to be a ProblemDetails JSON object, but found [].", ex.Message);
    }

    [TestMethod]
    public async Task AsyncContent_FailsForNullResponseOrContent() {
        HttpResponseMessage? missing = null;
        var nullResponse = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => missing.Should().HaveStringContentAsync("x"));
        Assert.AreEqual("Expected missing to have string content, but response is null.", nullResponse.Message);

        using var emptyContent = new HttpResponseMessage(HttpStatusCode.OK);
        var invalidEmpty = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => emptyContent.Should().HaveJsonContentAsync());
        Assert.Contains("to have valid JSON content, but parsing failed", invalidEmpty.Message);
    }
}

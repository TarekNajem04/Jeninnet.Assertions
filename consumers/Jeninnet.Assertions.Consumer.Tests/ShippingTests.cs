using System.Net;
using System.Text.Json.Nodes;
using Jeninnet.Assertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jeninnet.Assertions.Consumer.Tests;

[TestClass]
public class ShippingTests {
    public TestContext TestContext { get; set; } = null!;

    [TestMethod]
    public async Task The_tracking_number_is_read_from_the_provider_response() {
        using var httpClient = new HttpClient(new StubShippingHandler());
        var client = new ShippingClient(httpClient);

        var trackingNumber = await client.GetTrackingNumberAsync("C-42", TestContext.CancellationToken);

        trackingNumber.Should().Be("TRACK-42");
    }

    [TestMethod]
    public async Task The_provider_response_is_a_successful_json_response() {
        using var httpClient = new HttpClient(new StubShippingHandler());

        using var response = await httpClient.GetAsync(new Uri("https://shipping.example/api/orders/C-42/tracking"), TestContext.CancellationToken);

        response.Should().BeSuccessful().And.HaveHeader("Content-Type");
    }

    [TestMethod]
    public async Task A_failed_provider_response_surfaces_as_an_http_error() {
        using var httpClient = new HttpClient(new StubShippingHandler(HttpStatusCode.NotFound, """{"error":"missing"}"""));
        var client = new ShippingClient(httpClient);

        Func<Task> call = () => client.GetTrackingNumberAsync("C-404", TestContext.CancellationToken);

        await call.Should().ThrowAsync<HttpRequestException>();
    }

    [TestMethod]
    public async Task The_response_body_exposes_the_tracking_number_via_json_path() {
        using var httpClient = new HttpClient(new StubShippingHandler());

        using var response = await httpClient.GetAsync(new Uri("https://shipping.example/api/orders/C-42/tracking"), TestContext.CancellationToken);
        var json = await response.Content.ReadAsStringAsync(TestContext.CancellationToken);
        var node = JsonNode.Parse(json)!;

        node.Should().HaveJsonPath("$.trackingNumber").Which.Should().HaveValue("TRACK-42");
    }
}

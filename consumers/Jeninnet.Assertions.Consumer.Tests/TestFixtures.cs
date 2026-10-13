using System.Net;
using System.Text;

namespace Jeninnet.Assertions.Consumer.Tests;

internal static class TestOrders {
    public static Order SampleOrder() =>
        new("C-42", [new OrderLine("WIDGET", 2, 60m), new OrderLine("GADGET", 1, 10m)]);
}

internal sealed class StubShippingHandler(
    HttpStatusCode statusCode = HttpStatusCode.OK,
    string body = """{"trackingNumber":"TRACK-42"}""") : HttpMessageHandler {
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
        Task.FromResult(new HttpResponseMessage(statusCode) {
            Content = new StringContent(body, Encoding.UTF8, "application/json"),
        });
}

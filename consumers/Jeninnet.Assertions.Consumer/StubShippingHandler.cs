using System.Net;
using System.Text;

namespace Jeninnet.Assertions.Consumer;

/// <summary>A stub shipping provider used by the console demo.</summary>
internal sealed class StubShippingHandler : HttpMessageHandler {
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
        Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent("""{"trackingNumber":"TRACK-42"}""", Encoding.UTF8, "application/json"),
        });
}

using System.Text.Json.Nodes;

namespace Jeninnet.Assertions.Consumer;

/// <summary>Talks to the shipping provider's HTTP API.</summary>
public sealed class ShippingClient {
    private readonly HttpClient _httpClient;

    /// <summary>Initializes the client with the HTTP client used for shipping calls.</summary>
    /// <param name="httpClient">The HTTP client; must not be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="httpClient"/> is <see langword="null"/>.</exception>
    public ShippingClient(HttpClient httpClient) {
        ArgumentNullException.ThrowIfNull(httpClient);

        _httpClient = httpClient;
    }

    /// <summary>Gets the tracking number assigned to an order.</summary>
    /// <param name="orderId">The order identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The provider tracking number.</returns>
    /// <exception cref="HttpRequestException">The provider responded with a non-success status code.</exception>
    /// <exception cref="InvalidOperationException">The provider returned an empty body or no tracking number.</exception>
    public async Task<string> GetTrackingNumberAsync(string orderId, CancellationToken cancellationToken = default) {
        using var response = await _httpClient
            .GetAsync(new Uri($"https://shipping.example/api/orders/{orderId}/tracking"), cancellationToken)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var node = JsonNode.Parse(json) ?? throw new InvalidOperationException("The shipping provider returned an empty body.");

        return node["trackingNumber"]?.GetValue<string>()
            ?? throw new InvalidOperationException("The shipping provider returned no tracking number.");
    }
}

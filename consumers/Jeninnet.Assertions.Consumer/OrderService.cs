namespace Jeninnet.Assertions.Consumer;

/// <summary>Coordinates validation, pricing, payment, and shipping for an order.</summary>
public sealed class OrderService {
    private readonly IPaymentGateway _paymentGateway;
    private readonly ShippingClient _shippingClient;

    /// <summary>Initializes the service with its collaborators.</summary>
    /// <param name="paymentGateway">The gateway used to charge customers; must not be <see langword="null"/>.</param>
    /// <param name="shippingClient">The client used to obtain tracking numbers; must not be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public OrderService(IPaymentGateway paymentGateway, ShippingClient shippingClient) {
        ArgumentNullException.ThrowIfNull(paymentGateway);
        ArgumentNullException.ThrowIfNull(shippingClient);

        _paymentGateway = paymentGateway;
        _shippingClient = shippingClient;
    }

    /// <summary>Raised after an order has been paid for and shipped.</summary>
    public event EventHandler<OrderPlacedEventArgs>? OrderPlaced;

    /// <summary>Places an order end to end: validate, price, charge, ship, and invoice.</summary>
    /// <param name="order">The order to place; must not be <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The confirmation with the invoice, transaction, and tracking number.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="order"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The order failed validation.</exception>
    /// <exception cref="PaymentDeclinedException">The gateway declined the charge.</exception>
    /// <exception cref="HttpRequestException">The shipping provider rejected the request.</exception>
    public async Task<OrderConfirmation> PlaceOrderAsync(Order order, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(order);

        var errors = OrderValidator.Validate(order);
        if(errors.Count > 0) {
            throw new InvalidOperationException(string.Join(" ", errors));
        }

        var price = PriceCalculator.Calculate(order);
        var receipt = await _paymentGateway.ChargeAsync(order.CustomerId, price.Total, cancellationToken).ConfigureAwait(false);
        var trackingNumber = await _shippingClient.GetTrackingNumberAsync(order.CustomerId, cancellationToken).ConfigureAwait(false);
        var invoice = InvoiceRenderer.Render(order, price, trackingNumber);

        OrderPlaced?.Invoke(this, new OrderPlacedEventArgs(order, price.Total, trackingNumber));

        return new OrderConfirmation(invoice, receipt.TransactionId, trackingNumber);
    }
}

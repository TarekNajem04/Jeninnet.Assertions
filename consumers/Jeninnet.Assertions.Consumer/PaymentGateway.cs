namespace Jeninnet.Assertions.Consumer;

/// <summary>Charges customers for orders.</summary>
public interface IPaymentGateway {
    /// <summary>Charges the specified amount to the customer.</summary>
    /// <param name="customerId">The customer to charge.</param>
    /// <param name="amount">The amount to charge.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A receipt describing the successful charge.</returns>
    Task<PaymentReceipt> ChargeAsync(string customerId, decimal amount, CancellationToken cancellationToken = default);
}

/// <summary>A simulated payment gateway used by the consumer template.</summary>
public sealed class PaymentGateway : IPaymentGateway {
    /// <summary>Gets the amount above which charges are declined.</summary>
    public const decimal DECLINED_THRESHOLD = 10_000m;

    /// <summary>Gets the simulated processing latency of the gateway.</summary>
    public static TimeSpan Latency { get; } = TimeSpan.FromMilliseconds(25);

    /// <inheritdoc />
    public async Task<PaymentReceipt> ChargeAsync(string customerId, decimal amount, CancellationToken cancellationToken = default) {
        await Task.Delay(Latency, cancellationToken).ConfigureAwait(false);

        if(amount > DECLINED_THRESHOLD) {
            throw new PaymentDeclinedException(
                $"The payment of {amount.ToString(System.Globalization.CultureInfo.InvariantCulture)} was declined by the gateway.");
        }

        return new PaymentReceipt($"R-{customerId}-{amount.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
    }
}

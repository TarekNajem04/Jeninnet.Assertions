namespace Jeninnet.Assertions.Consumer;

/// <summary>The confirmation returned after an order is placed.</summary>
/// <param name="Invoice">The rendered invoice text.</param>
/// <param name="TransactionId">The payment transaction identifier.</param>
/// <param name="TrackingNumber">The shipping tracking number.</param>
public sealed record OrderConfirmation(string Invoice, string TransactionId, string TrackingNumber);

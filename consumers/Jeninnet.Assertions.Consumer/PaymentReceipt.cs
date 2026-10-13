namespace Jeninnet.Assertions.Consumer;

/// <summary>The receipt of a successful charge.</summary>
/// <param name="TransactionId">The gateway transaction identifier.</param>
public sealed record PaymentReceipt(string TransactionId);

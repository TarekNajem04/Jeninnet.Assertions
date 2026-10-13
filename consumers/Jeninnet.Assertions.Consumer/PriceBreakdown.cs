namespace Jeninnet.Assertions.Consumer;

/// <summary>The monetary breakdown of an order.</summary>
/// <param name="Subtotal">The sum of all line totals.</param>
/// <param name="Discount">The volume discount applied to the subtotal.</param>
/// <param name="Tax">The tax computed on the discounted amount.</param>
/// <param name="Total">The final amount to charge.</param>
public sealed record PriceBreakdown(decimal Subtotal, decimal Discount, decimal Tax, decimal Total);

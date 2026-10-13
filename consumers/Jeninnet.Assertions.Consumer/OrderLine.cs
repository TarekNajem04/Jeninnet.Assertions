namespace Jeninnet.Assertions.Consumer;

/// <summary>One line of an order: a product, a quantity, and the unit price.</summary>
/// <param name="Sku">The product identifier.</param>
/// <param name="Quantity">The number of units ordered.</param>
/// <param name="UnitPrice">The price of a single unit.</param>
public sealed record OrderLine(string Sku, int Quantity, decimal UnitPrice);

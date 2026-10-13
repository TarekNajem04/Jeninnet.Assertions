namespace Jeninnet.Assertions.Consumer;

/// <summary>An order placed by a customer.</summary>
/// <param name="CustomerId">The identifier of the customer placing the order.</param>
/// <param name="Lines">The ordered lines; must contain at least one line.</param>
public sealed record Order(string CustomerId, IReadOnlyList<OrderLine> Lines);

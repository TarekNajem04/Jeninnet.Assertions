namespace Jeninnet.Assertions.Consumer;

/// <summary>Carries the details of an order that has been placed.</summary>
/// <param name="order">The placed order.</param>
/// <param name="total">The charged total.</param>
/// <param name="trackingNumber">The shipping tracking number.</param>
public sealed class OrderPlacedEventArgs(Order order, decimal total, string trackingNumber) : EventArgs {
    /// <summary>Gets the placed order.</summary>
    public Order Order { get; } = order;

    /// <summary>Gets the charged total.</summary>
    public decimal Total { get; } = total;

    /// <summary>Gets the shipping tracking number.</summary>
    public string TrackingNumber { get; } = trackingNumber;
}

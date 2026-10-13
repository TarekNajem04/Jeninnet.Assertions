namespace Jeninnet.Assertions.Consumer;

/// <summary>Computes the monetary breakdown of an order.</summary>
public static class PriceCalculator {
    /// <summary>Gets the minimum subtotal that qualifies for the volume discount.</summary>
    public const decimal VOLUME_DISCOUNT_THRESHOLD = 100m;

    /// <summary>Gets the volume discount rate applied at or above the threshold.</summary>
    public const decimal VOLUME_DISCOUNT_RATE = 0.10m;

    /// <summary>Gets the tax rate applied to the discounted amount.</summary>
    public const decimal TAX_RATE = 0.14m;

    /// <summary>Calculates the breakdown for the specified order.</summary>
    /// <param name="order">The order to price; must not be <see langword="null"/>.</param>
    /// <returns>The computed subtotal, discount, tax, and total.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="order"/> is <see langword="null"/>.</exception>
    public static PriceBreakdown Calculate(Order order) {
        ArgumentNullException.ThrowIfNull(order);

        var subtotal = Round(order.Lines.Sum(line => line.Quantity * line.UnitPrice));
        var discount = subtotal >= VOLUME_DISCOUNT_THRESHOLD ? Round(subtotal * VOLUME_DISCOUNT_RATE) : 0m;
        var tax = Round((subtotal - discount) * TAX_RATE);

        return new PriceBreakdown(subtotal, discount, tax, Round(subtotal - discount + tax));
    }

    private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}

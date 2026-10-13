using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Jeninnet.Assertions.Consumer;

/// <summary>Renders invoices as human-readable text and as deterministic JSON.</summary>
public static class InvoiceRenderer {
    private static readonly JsonSerializerOptions _indentedOptions = new() { WriteIndented = true };

    /// <summary>Renders a human-readable invoice for a priced order.</summary>
    /// <param name="order">The order being invoiced; must not be <see langword="null"/>.</param>
    /// <param name="price">The computed price breakdown; must not be <see langword="null"/>.</param>
    /// <param name="trackingNumber">The shipping tracking number.</param>
    /// <returns>The invoice text, one line per entry, using <c>\n</c> line separators so snapshots are byte-identical on every machine.</returns>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public static string Render(Order order, PriceBreakdown price, string trackingNumber) {
        ArgumentNullException.ThrowIfNull(order);
        ArgumentNullException.ThrowIfNull(price);

        var builder = new StringBuilder();
        builder.Append("INVOICE\n");
        builder.Append(CultureInfo.InvariantCulture, $"Customer: {order.CustomerId}\n");

        foreach(var line in order.Lines) {
            builder.Append(
                CultureInfo.InvariantCulture,
                $"{line.Quantity} x {line.Sku} @ {line.UnitPrice:0.00} = {line.Quantity * line.UnitPrice:0.00}\n");
        }

        builder.Append(CultureInfo.InvariantCulture, $"Subtotal: {price.Subtotal:0.00}\n");
        builder.Append(CultureInfo.InvariantCulture, $"Discount: {price.Discount:0.00}\n");
        builder.Append(CultureInfo.InvariantCulture, $"Tax: {price.Tax:0.00}\n");
        builder.Append(CultureInfo.InvariantCulture, $"Total: {price.Total:0.00}\n");
        builder.Append(CultureInfo.InvariantCulture, $"Tracking: {trackingNumber}");

        return builder.ToString();
    }

    /// <summary>Exports the invoice as indented, deterministic JSON.</summary>
    /// <param name="order">The order being invoiced; must not be <see langword="null"/>.</param>
    /// <param name="price">The computed price breakdown; must not be <see langword="null"/>.</param>
    /// <param name="trackingNumber">The shipping tracking number.</param>
    /// <returns>The invoice JSON.</returns>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public static string ExportJson(Order order, PriceBreakdown price, string trackingNumber) {
        ArgumentNullException.ThrowIfNull(order);
        ArgumentNullException.ThrowIfNull(price);

        var invoice = new {
            customerId = order.CustomerId,
            lines = order.Lines
                .Select(line => new {
                    sku = line.Sku,
                    quantity = line.Quantity,
                    unitPrice = line.UnitPrice,
                    lineTotal = line.Quantity * line.UnitPrice,
                })
                .ToArray(),
            subtotal = price.Subtotal,
            discount = price.Discount,
            tax = price.Tax,
            total = price.Total,
            trackingNumber,
        };

        return JsonSerializer.Serialize(invoice, _indentedOptions);
    }
}

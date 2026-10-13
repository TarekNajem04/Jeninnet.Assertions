namespace Jeninnet.Assertions.Consumer;

/// <summary>Validates orders before they are priced and charged.</summary>
public static class OrderValidator {
    /// <summary>Validates the specified order and returns every violation found.</summary>
    /// <param name="order">The order to validate; must not be <see langword="null"/>.</param>
    /// <returns>One message per violation; empty when the order is valid.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="order"/> is <see langword="null"/>.</exception>
    public static IReadOnlyList<string> Validate(Order order) {
        ArgumentNullException.ThrowIfNull(order);

        var errors = new List<string>();
        if(string.IsNullOrWhiteSpace(order.CustomerId)) {
            errors.Add("The customer id must not be empty.");
        }

        if(order.Lines.Count == 0) {
            errors.Add("The order must contain at least one line.");
        }

        foreach(var line in order.Lines) {
            if(line.Quantity <= 0) {
                errors.Add($"Line {line.Sku} must have a positive quantity.");
            }

            if(line.UnitPrice <= 0m) {
                errors.Add($"Line {line.Sku} must have a positive unit price.");
            }
        }

        return errors;
    }
}

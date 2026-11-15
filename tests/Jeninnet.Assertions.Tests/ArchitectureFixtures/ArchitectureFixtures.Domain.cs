//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests.ArchitectureFixtures.Domain;

public sealed record Money(decimal Amount, string Currency);

public sealed class OrderLine {
    public string Sku { get; init; } = string.Empty;

    public int Quantity { get; init; }
}

public sealed class Order {
    public Guid Id { get; init; }

    public IReadOnlyList<OrderLine> Lines { get; } = [];

    public Money Total { get; init; } = new(0m, "USD");
}

public interface IOrderRepository {
    Order? Find(Guid id);

    void Save(Order order);
}

public sealed class LeakyOrder {
    public Infrastructure.SqlConnection? Connection { get; set; }
}

public sealed class OrderWithQuote {
    public Sales.Quote? Quote { get; set; }
}

[Infrastructure.InfrastructureMarker(typeof(Infrastructure.SqlConnection))]
public sealed class AttributedLeak;

public static class MethodOnlyLeak {
    public static void Connect() => _ = new Infrastructure.SqlConnection("server=.;database=orders");
}

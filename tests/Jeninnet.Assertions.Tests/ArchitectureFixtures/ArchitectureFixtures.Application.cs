//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests.ArchitectureFixtures.Application;

using Domain = Jeninnet.Assertions.Tests.ArchitectureFixtures.Domain;

public sealed class OrderService(Domain.IOrderRepository repository) {
    public Domain.Order? Find(Guid id) => repository.Find(id);

    public void Checkout(Domain.Order order) => repository.Save(order);
}

public static class PricingService {
    public static Domain.Money Reprice(Domain.Money price) {
        ArgumentNullException.ThrowIfNull(price);
        return price with { Amount = price.Amount * 1.1m };
    }
}

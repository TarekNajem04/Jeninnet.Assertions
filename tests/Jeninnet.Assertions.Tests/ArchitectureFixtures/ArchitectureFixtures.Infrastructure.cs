//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
using Domain = Jeninnet.Assertions.Tests.ArchitectureFixtures.Domain;

namespace Jeninnet.Assertions.Tests.ArchitectureFixtures.Infrastructure;

[AttributeUsage(AttributeTargets.Class)]
public sealed class InfrastructureMarkerAttribute(Type type) : Attribute {
    public Type Type { get; } = type;
}

public sealed record SqlConnection(string ConnectionString);

public sealed class SqlOrderRepository(SqlConnection connection) : Domain.IOrderRepository {
    public SqlConnection Connection { get; } = connection;

    public Domain.Order? Find(Guid id) => null;

    public void Save(Domain.Order order) {
    }
}

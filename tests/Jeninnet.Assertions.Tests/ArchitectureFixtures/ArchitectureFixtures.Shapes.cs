//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests.ArchitectureFixtures.Shapes;

#pragma warning disable S2094, CA1852, CA1051 // Shape fixtures: exist only for reflection selection and public-surface checks.

public interface IShape {
    double Area();
}

public abstract class ShapeBase {
    public abstract double Area();
}

public sealed class Circle : ShapeBase, IShape {
    public override double Area() => 3.14;
}

public sealed class PaymentHandler;

public sealed class OrderHandler;

public sealed class AuditService;

public class MixedVisibility {
    public int PublicValue;

    internal int InternalValue = 1;

    protected int ProtectedValue = 2;

    private readonly int _privateValue = 3;

    public int UsePrivate() => _privateValue + ProtectedValue + InternalValue;
}

public sealed class PublicSurface {
    public int Value { get; init; }

    private readonly int _hidden = 1;

    public int ReadHidden() => _hidden;
}

public sealed class MutableBox {
    public int Value { get; set; }
}

public sealed class ImmutableBox {
    public int Value { get; init; }
}

public readonly record struct Point(int X, int Y);

public record Person(string Name);

public sealed class GenericThing<T> {
    public T? Value { get; init; }
}

public static class Outer {
    public sealed class NestedPublic;

    internal sealed class NestedInternal;

    private sealed class NestedPrivate;

    internal static Type InternalType => typeof(NestedInternal);

    internal static Type PrivateType => typeof(NestedPrivate);
}

internal sealed class InternalThing;

#pragma warning restore S2094, CA1852, CA1051

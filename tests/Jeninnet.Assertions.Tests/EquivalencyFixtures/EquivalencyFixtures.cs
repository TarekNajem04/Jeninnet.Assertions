//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests.EquivalencyFixtures;

// Fixtures live in their own namespace because type-scanning tests sweep the whole test assembly.
internal class Shape {
    public int Sides { get; set; }
}

internal sealed class Circle : Shape {
    public double Radius { get; set; }
}

internal sealed class Inner {
    public string City { get; set; } = "";

    public string Name { get; set; } = "";

    public int Rank { get; set; }
}

internal sealed class Sample {
    public string Name { get; set; } = "";

    public int Age { get; set; }

    public int Code;

    public Shape? Shape { get; set; }

    public Inner? Inner { get; set; }

    public List<Inner> Items { get; set; } = [];

    public List<Inner> Others { get; set; } = [];
}

internal sealed class WithInternal {
    public int Value { get; set; }

    internal int InternalValue { get; set; }

    internal int InternalField;
}

internal class Base {
    public string Inherited { get; set; } = "";
}

internal sealed class Derived : Base {
    public int Own { get; set; }
}

internal interface ILabeled {
    string Label { get; }
}

internal sealed class Explicitly(string label) : ILabeled {
    string ILabeled.Label => label;
}

internal sealed class Hidden {
    public int Value { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public int Secret { get; set; }
}

internal sealed class WithoutZip {
    public string City { get; set; } = "";
}

internal sealed class WithZip {
    public string City { get; set; } = "";

    public string Zip { get; set; } = "";
}

internal sealed class CityExpect {
    public string City { get; set; } = "";
}

internal sealed class TownSubject {
    public string Town { get; set; } = "";

    public string County { get; set; } = "";
}

internal sealed class CityRoot {
    public string City { get; set; } = "";
}

internal sealed class TownRoot {
    public string Town { get; set; } = "";
}

internal sealed class CityGraph {
    public CityExpect Inner { get; set; } = new();
}

internal sealed class TownGraph {
    public TownSubject Inner { get; set; } = new();
}

internal sealed class Section {
    public List<Inner> Items { get; set; } = [];
}

internal sealed class Grouped {
    public string Name { get; set; } = "";

    public List<Section> Groups { get; set; } = [];
}

internal sealed class Keyed(int id, string name) {
    public int Id { get; } = id;

    public string Name { get; } = name;

    public override bool Equals(object? obj) => obj is Keyed other && Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();
}

internal sealed class Opaque(int value) {
    private readonly int _value = value;

    public override bool Equals(object? obj) => obj is Opaque other && _value == other._value;

    public override int GetHashCode() => _value.GetHashCode();
}

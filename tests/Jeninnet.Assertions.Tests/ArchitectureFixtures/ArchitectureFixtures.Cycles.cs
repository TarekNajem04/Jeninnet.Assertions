//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests.ArchitectureFixtures.Cycles;

#pragma warning disable S2094 // Cycle fixtures: properties create the dependency edges under test.

public sealed class CycleA {
    public CycleB? B { get; set; }
}

public sealed class CycleB {
    public CycleA? A { get; set; }
}

public sealed class SelfCycle {
    public SelfCycle? Next { get; set; }
}

public sealed class CycleLeaf {
    public int Value { get; set; }
}

public sealed class NoCycleRoot {
    public CycleLeaf? Leaf { get; set; }
}

#pragma warning restore S2094

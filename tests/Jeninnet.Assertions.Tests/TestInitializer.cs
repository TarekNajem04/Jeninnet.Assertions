//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

/// <summary>Runs once before the first assertion via the assembly-level engine initializer.</summary>
public static class TestInitializer {
    /// <summary>Gets whether the initializer has run.</summary>
    public static bool Ran { get; private set; }

    /// <summary>Marks the initializer as run.</summary>
    public static void Initialize() => Ran = true;
}

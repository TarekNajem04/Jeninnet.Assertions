//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Equivalency;

/// <summary>Selects how enum values are compared.</summary>
public enum EnumComparison {
    /// <summary>Same runtime type and equal value.</summary>
    Default = 0,

    /// <summary>Equal numeric value across enum types.</summary>
    ByValue = 1,

    /// <summary>Equal identifier name across enum types.</summary>
    ByName = 2,
}

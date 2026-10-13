//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Equivalency;

/// <summary>Selects which member set the root expectation contributes to the comparison.</summary>
public enum MemberTypePreference {
    /// <summary>Runtime-type members (the default).</summary>
    Default = 0,

    /// <summary>Explicitly runtime-type members.</summary>
    Runtime = 1,

    /// <summary>Members declared on the compile-time expectation type.</summary>
    Declared = 2,
}

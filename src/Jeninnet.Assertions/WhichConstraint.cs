//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>
/// Provides access to a matched value after an assertion succeeds.
/// Enables chaining further assertions on the same object without additional lookups.
/// </summary>
/// <typeparam name="T">The type of the matched value.</typeparam>
/// <param name="value">The matched value to expose.</param>
public class WhichConstraint<T>(T value) {
    /// <summary>Gets the matched value from the preceding assertion.</summary>
    public T Which { get; } = value;
}

//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>
/// Enables chaining an additional assertion after a preceding one while also exposing a matched value
/// (for example, the result of an asynchronous operation that completed in time).
/// </summary>
/// <typeparam name="TParent">The type of the parent assertions object to continue asserting on.</typeparam>
/// <typeparam name="T">The type of the matched value.</typeparam>
/// <param name="parent">The parent assertions object exposed through <see cref="AndConstraint{TParent}.And"/>.</param>
/// <param name="value">The matched value exposed through <see cref="Which"/>.</param>
/// <param name="expression">The caller expression naming the subject whose result is exposed.</param>
public class AndWhichConstraint<TParent, T>(TParent parent, T value, string? expression = null) : AndConstraint<TParent>(parent) {
    /// <summary>Gets the matched value from the preceding assertion.</summary>
    public T Which { get; } = value;

    /// <summary>Gets the caller expression naming the subject whose result is exposed.</summary>
    public string? Expression { get; } = expression;
}

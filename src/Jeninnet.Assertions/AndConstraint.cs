//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>
/// Enables chaining an additional assertion after a preceding one
/// (e.g. <c>value.Should().Be("a").And.NotBeNull()</c>).
/// </summary>
/// <typeparam name="TParent">The type of the parent assertions object to continue asserting on.</typeparam>
/// <param name="parent">The parent assertions object exposed through <see cref="And"/>.</param>
public class AndConstraint<TParent>(TParent parent) {
    /// <summary>Gets the parent assertions object to continue asserting on.</summary>
    public TParent And => parent;
}

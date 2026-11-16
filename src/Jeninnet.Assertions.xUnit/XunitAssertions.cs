//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Xunit;

/// <summary>Runs Jeninnet assertions, surfacing failures as xUnit v2 outcomes.</summary>
/// <remarks>Opt-in only: reference this package and wrap assertions explicitly. The core
/// <c>Jeninnet.Assertions</c> package never depends on xUnit. For xUnit v3 use
/// <c>Jeninnet.Assertions.xUnit.v3</c>, which targets its assertion assembly separately.</remarks>
public static class XunitAssertions {
    /// <summary>Runs a synchronous assertion block, translating failures to xUnit.</summary>
    /// <param name="assertion">The assertions to run (must not be <see langword="null"/>).</param>
    /// <exception cref="ArgumentNullException"><paramref name="assertion"/> is <see langword="null"/>.</exception>
    /// <exception cref="XunitException">A Jeninnet assertion failed (message and inner exception preserved).</exception>
    public static void That(Action assertion) => XunitAssertionRunner.That(assertion);

    /// <summary>Runs an asynchronous assertion block, translating failures to xUnit.</summary>
    /// <param name="assertion">The assertions to run (must not be <see langword="null"/>).</param>
    /// <returns>A task that represents the asynchronous assertion operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assertion"/> is <see langword="null"/>.</exception>
    /// <exception cref="XunitException">A Jeninnet assertion failed (message and inner exception preserved).</exception>
    public static Task ThatAsync(Func<Task> assertion) => XunitAssertionRunner.ThatAsync(assertion);
}

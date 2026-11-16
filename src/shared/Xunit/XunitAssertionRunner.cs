//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Xunit.Internal;

/// <summary>Shared xUnit adapter logic compiled into both the v2 and v3 packages.</summary>
internal static class XunitAssertionRunner {
    /// <summary>Runs a synchronous assertion block, translating failures to xUnit.</summary>
    /// <param name="assertion">The assertions to run (must not be <see langword="null"/>).</param>
    /// <exception cref="ArgumentNullException"><paramref name="assertion"/> is <see langword="null"/>.</exception>
    /// <exception cref="XunitException">A Jeninnet assertion failed (message and inner exception preserved).</exception>
    internal static void That(Action assertion) {
        ArgumentNullException.ThrowIfNull(assertion);

        try {
            assertion();
        }
        catch(AssertionFailedException ex) {
            throw Translate(ex);
        }
    }

    /// <summary>Runs an asynchronous assertion block, translating failures to xUnit.</summary>
    /// <param name="assertion">The assertions to run (must not be <see langword="null"/>).</param>
    /// <returns>A task that represents the asynchronous assertion operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assertion"/> is <see langword="null"/>.</exception>
    /// <exception cref="XunitException">A Jeninnet assertion failed (message and inner exception preserved).</exception>
    internal static async Task ThatAsync(Func<Task> assertion) {
        ArgumentNullException.ThrowIfNull(assertion);

        try {
            await assertion();
        }
        catch(AssertionFailedException ex) {
            throw Translate(ex);
        }
    }

    /// <summary>Creates an xUnit exception that preserves the original failure and its inner exception.</summary>
    /// <param name="exception">The Jeninnet assertion failure.</param>
    /// <returns>The translated xUnit exception.</returns>
    private static XunitException Translate(AssertionFailedException exception) =>
        exception.InnerException is null
            ? new XunitException(exception.Message)
            : new XunitException(exception.Message, exception.InnerException);
}

//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.MSTest;

/// <summary>Runs Jeninnet assertions, surfacing failures as MSTest outcomes.</summary>
/// <remarks>Opt-in only: reference this package and wrap assertions explicitly. The core
/// <c>Jeninnet.Assertions</c> package never depends on MSTest.</remarks>
public static class MSTestAssertions {
    /// <summary>Runs a synchronous assertion block, translating failures to MSTest.</summary>
    /// <param name="assertion">The assertions to run (must not be <see langword="null"/>).</param>
    /// <exception cref="ArgumentNullException"><paramref name="assertion"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertFailedException">A Jeninnet assertion failed (message and inner exception preserved).</exception>
    public static void That(Action assertion) {
        ArgumentNullException.ThrowIfNull(assertion);

        try {
            assertion();
        }
        catch(AssertionFailedException ex) {
            throw ex.InnerException is null
                ? new AssertFailedException(ex.Message)
                : new AssertFailedException(ex.Message, ex.InnerException);
        }
    }

    /// <summary>Runs an asynchronous assertion block, translating failures to MSTest.</summary>
    /// <param name="assertion">The assertions to run (must not be <see langword="null"/>).</param>
    /// <returns>A task that represents the asynchronous assertion operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assertion"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertFailedException">A Jeninnet assertion failed (message and inner exception preserved).</exception>
    public static async Task ThatAsync(Func<Task> assertion) {
        ArgumentNullException.ThrowIfNull(assertion);

        try {
            await assertion();
        }
        catch(AssertionFailedException ex) {
            throw ex.InnerException is null
                ? new AssertFailedException(ex.Message)
                : new AssertFailedException(ex.Message, ex.InnerException);
        }
    }
}

//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Asserts directly on a value with a caller-supplied predicate or check.</summary>
/// <remarks>Use inside equivalency callbacks or standalone when a full assertion object is overkill.</remarks>
public static class Value {
    /// <summary>Asserts that <paramref name="value"/> satisfies <paramref name="predicate"/>.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="value">The value under assertion.</param>
    /// <param name="predicate">The condition the value must satisfy (side-effect free; thrown <see cref="AssertionFailedException"/> propagates).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The predicate returns <see langword="false"/>.</exception>
    public static void ThatMatches<T>(T value, Expression<Func<T, bool>> predicate, string? because = null, params object?[] becauseArgs) {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        if(!predicate.Compile()(value)) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.VALUE_THAT_MATCHES, predicate.Body, AssertionScope.FormatInvariant(value)), because, becauseArgs));
        }
    }

    /// <summary>Runs <paramref name="action"/> as the assertion: it must complete without failing.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="value">The value under assertion.</param>
    /// <param name="action">The check to run (typically further assertions on <paramref name="value"/>).</param>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The action throws a non-assertion exception.</exception>
    public static void ThatSatisfies<T>(T value, Action<T> action) {
        if(action is null) {
            throw new ArgumentNullException(nameof(action));
        }

        try {
            action(value);
        }
        catch(AssertionFailedException) {
            throw;
        }
        catch(Exception ex) {
            AssertionScope.Fail(
                Strings.Format(StringKeys.VALUE_THAT_SATISFIES, ex.GetType().FullName ?? ex.GetType().Name, ex.Message),
                ex);
        }
    }
}

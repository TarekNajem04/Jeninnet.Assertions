//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class NullableDateTimeOffsetAssertions {
    private const string NEGATIVE_WINDOW_MESSAGE = "Window must not be negative.";
    /// <summary>Asserts that the value is within <paramref name="window"/> of the current moment and returns a range for anchoring with <c>Before</c>/<c>After</c>.</summary>
    /// <param name="window">The maximum accepted absolute distance from now (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="DateTimeOffsetRangeAssertions"/> for anchoring the window with <c>Before</c>/<c>After</c>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="window"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or further than <paramref name="window"/> from now.</exception>
    public DateTimeOffsetRangeAssertions BeWithin(TimeSpan window, string? because = null, params object?[] becauseArgs) {
        if(window < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(window), NEGATIVE_WINDOW_MESSAGE);
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.DATE_BE_WITHIN_NOW, Expression, AssertionScope.FormatInvariant(window), Format(_value)), because, becauseArgs);
            return new DateTimeOffsetRangeAssertions(null, window, DateTimeOffsetRangeMode.Within, Expression);
        }

        var difference = (_value.Value - AssertionClock.Current.Now).Duration();
        if(difference > window) {
            Fail(Strings.Format(StringKeys.DATE_BE_WITHIN_NOW, Expression, AssertionScope.FormatInvariant(window), Format(_value)), because, becauseArgs);
        }

        return new DateTimeOffsetRangeAssertions(_value, window, DateTimeOffsetRangeMode.Within, Expression);
    }

    /// <summary>Asserts that the value is strictly less than <paramref name="window"/> from the current moment and returns a range for anchoring.</summary>
    /// <param name="window">The exclusive upper bound on the absolute distance from now (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="DateTimeOffsetRangeAssertions"/> for anchoring the window with <c>Before</c>/<c>After</c>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="window"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or at least <paramref name="window"/> from now.</exception>
    public DateTimeOffsetRangeAssertions BeLessThan(TimeSpan window, string? because = null, params object?[] becauseArgs) {
        if(window < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(window), NEGATIVE_WINDOW_MESSAGE);
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.DATE_BE_LESS_THAN_NOW, Expression, AssertionScope.FormatInvariant(window), Format(_value)), because, becauseArgs);
            return new DateTimeOffsetRangeAssertions(null, window, DateTimeOffsetRangeMode.LessThan, Expression);
        }

        var difference = (_value.Value - AssertionClock.Current.Now).Duration();
        if(difference >= window) {
            Fail(Strings.Format(StringKeys.DATE_BE_LESS_THAN_NOW, Expression, AssertionScope.FormatInvariant(window), Format(_value)), because, becauseArgs);
        }

        return new DateTimeOffsetRangeAssertions(_value, window, DateTimeOffsetRangeMode.LessThan, Expression);
    }

    /// <summary>Asserts that the value is strictly more than <paramref name="window"/> from the current moment and returns a range for anchoring.</summary>
    /// <param name="window">The exclusive lower bound on the absolute distance from now (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="DateTimeOffsetRangeAssertions"/> for anchoring the window with <c>Before</c>/<c>After</c>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="window"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or at most <paramref name="window"/> from now.</exception>
    public DateTimeOffsetRangeAssertions BeMoreThan(TimeSpan window, string? because = null, params object?[] becauseArgs) {
        if(window < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(window), NEGATIVE_WINDOW_MESSAGE);
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.DATE_BE_MORE_THAN_NOW, Expression, AssertionScope.FormatInvariant(window), Format(_value)), because, becauseArgs);
            return new DateTimeOffsetRangeAssertions(null, window, DateTimeOffsetRangeMode.MoreThan, Expression);
        }

        var difference = (_value.Value - AssertionClock.Current.Now).Duration();
        if(difference <= window) {
            Fail(Strings.Format(StringKeys.DATE_BE_MORE_THAN_NOW, Expression, AssertionScope.FormatInvariant(window), Format(_value)), because, becauseArgs);
        }

        return new DateTimeOffsetRangeAssertions(_value, window, DateTimeOffsetRangeMode.MoreThan, Expression);
    }

    /// <summary>Asserts that the value is at least <paramref name="window"/> from the current moment and returns a range for anchoring.</summary>
    /// <param name="window">The inclusive lower bound on the absolute distance from now (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="DateTimeOffsetRangeAssertions"/> for anchoring the window with <c>Before</c>/<c>After</c>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="window"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or less than <paramref name="window"/> from now.</exception>
    public DateTimeOffsetRangeAssertions BeAtLeast(TimeSpan window, string? because = null, params object?[] becauseArgs) {
        if(window < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(window), NEGATIVE_WINDOW_MESSAGE);
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.DATE_BE_AT_LEAST_NOW, Expression, AssertionScope.FormatInvariant(window), Format(_value)), because, becauseArgs);
            return new DateTimeOffsetRangeAssertions(null, window, DateTimeOffsetRangeMode.AtLeast, Expression);
        }

        var difference = (_value.Value - AssertionClock.Current.Now).Duration();
        if(difference < window) {
            Fail(Strings.Format(StringKeys.DATE_BE_AT_LEAST_NOW, Expression, AssertionScope.FormatInvariant(window), Format(_value)), because, becauseArgs);
        }

        return new DateTimeOffsetRangeAssertions(_value, window, DateTimeOffsetRangeMode.AtLeast, Expression);
    }

    /// <summary>Defines a window of exactly <paramref name="window"/> for anchoring with <c>Before</c>/<c>After</c>.</summary>
    /// <remarks>Unlike the other window factories, this method performs no immediate assertion on a present
    /// value: tick-exact comparison against a moving clock cannot pass deterministically. The assertion
    /// completes with <c>Before</c>/<c>After</c>. A <see langword="null"/> subject fails immediately.</remarks>
    /// <param name="window">The exact absolute distance (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="DateTimeOffsetRangeAssertions"/> for anchoring the window with <c>Before</c>/<c>After</c>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="window"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/>.</exception>
    public DateTimeOffsetRangeAssertions BeExactly(TimeSpan window, string? because = null, params object?[] becauseArgs) {
        if(window < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(window), NEGATIVE_WINDOW_MESSAGE);
        }

        if(_value is null) {
            Fail(Strings.Format(StringKeys.DATE_BE_EXACTLY_NOW, Expression, AssertionScope.FormatInvariant(window), Format(_value)), because, becauseArgs);
            return new DateTimeOffsetRangeAssertions(null, window, DateTimeOffsetRangeMode.Exactly, Expression);
        }

        return new DateTimeOffsetRangeAssertions(_value, window, DateTimeOffsetRangeMode.Exactly, Expression);
    }
}

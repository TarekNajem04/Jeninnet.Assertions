//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Window comparison modes for <see cref="DateTimeOffsetRangeAssertions"/>.</summary>
internal enum DateTimeOffsetRangeMode {
    Within = 0,
    LessThan = 1,
    MoreThan = 2,
    AtLeast = 3,
    Exactly = 4,
}

/// <summary>Provides relative-window assertions for a <see cref="DateTimeOffset"/> value against an anchor moment.</summary>
/// <remarks>
/// Instances are created by the <c>BeWithin</c>/<c>BeLessThan</c>/<c>BeMoreThan</c>/<c>BeAtLeast</c>/
/// <c>BeExactly</c> factory methods on <see cref="DateTimeOffsetAssertions"/> (which assert the window against
/// the current moment immediately) and refined with <see cref="Before"/> or <see cref="After"/>.
/// <see cref="DateTimeOffset"/> subtraction is instant-based, so offsets never skew the arithmetic.
/// <see cref="Before"/> and <see cref="After"/> are exclusive: a subject exactly equal to the anchor
/// always fails.
/// </remarks>
public class DateTimeOffsetRangeAssertions : AssertionsBase {
    private readonly DateTimeOffset? _value;
    private readonly TimeSpan _window;
    private readonly DateTimeOffsetRangeMode _mode;

    /// <summary>Creates a range assertion over the given value, window, and mode.</summary>
    /// <param name="value">The subject value (may be <see langword="null"/> when built from a nullable subject).</param>
    /// <param name="window">The accepted absolute distance.</param>
    /// <param name="mode">How the distance compares to <paramref name="window"/>.</param>
    /// <param name="expression">The caller expression naming the subject under test.</param>
    internal DateTimeOffsetRangeAssertions(DateTimeOffset? value, TimeSpan window, DateTimeOffsetRangeMode mode, string? expression = null) : base(expression, "date") {
        _value = value;
        _window = window;
        _mode = mode;
    }

    /// <summary>Asserts that the value is strictly before <paramref name="anchor"/> and inside the window.</summary>
    /// <param name="anchor">The exclusive upper bound the value must precede within the window.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is not before <paramref name="anchor"/> inside the window.</exception>
    public AndConstraint<DateTimeOffsetAssertions> Before(DateTimeOffset anchor, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(BeforeKey(), Expression, AssertionScope.FormatInvariant(_window), Format(anchor), "<null>"), because, becauseArgs);
            return new(new DateTimeOffsetAssertions(default, Expression));
        }

        var difference = anchor - _value.Value;
        if(difference <= TimeSpan.Zero || !MatchesMode(difference)) {
            Fail(Strings.Format(BeforeKey(), Expression, AssertionScope.FormatInvariant(_window), Format(anchor), Format(_value.Value)), because, becauseArgs);
        }

        return new(new DateTimeOffsetAssertions(_value.Value, Expression));
    }

    /// <summary>Asserts that the value is strictly after <paramref name="anchor"/> and inside the window.</summary>
    /// <param name="anchor">The exclusive lower bound the value must follow within the window.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is not after <paramref name="anchor"/> inside the window.</exception>
    public AndConstraint<DateTimeOffsetAssertions> After(DateTimeOffset anchor, string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(AfterKey(), Expression, AssertionScope.FormatInvariant(_window), Format(anchor), "<null>"), because, becauseArgs);
            return new(new DateTimeOffsetAssertions(default, Expression));
        }

        var difference = _value.Value - anchor;
        if(difference <= TimeSpan.Zero || !MatchesMode(difference)) {
            Fail(Strings.Format(AfterKey(), Expression, AssertionScope.FormatInvariant(_window), Format(anchor), Format(_value.Value)), because, becauseArgs);
        }

        return new(new DateTimeOffsetAssertions(_value.Value, Expression));
    }

    /// <summary>Evaluates the window comparison for the configured range mode.</summary>
    /// <param name="difference">The positive difference between the value and the anchor.</param>
    /// <returns><see langword="true"/> when the difference satisfies the mode's bound.</returns>
    private bool MatchesMode(TimeSpan difference) => _mode switch {
        DateTimeOffsetRangeMode.Within => difference <= _window,
        DateTimeOffsetRangeMode.LessThan => difference < _window,
        DateTimeOffsetRangeMode.MoreThan => difference > _window,
        DateTimeOffsetRangeMode.AtLeast => difference >= _window,
        _ => difference == _window,
    };

    /// <summary>Gets the localization key for the failure message of a value before the anchor.</summary>
    /// <returns>The template key matching the configured range mode.</returns>
    private string BeforeKey() => _mode switch {
        DateTimeOffsetRangeMode.Within => StringKeys.DATE_BE_WITHIN_BEFORE,
        DateTimeOffsetRangeMode.LessThan => StringKeys.DATE_BE_LESS_THAN_BEFORE,
        DateTimeOffsetRangeMode.MoreThan => StringKeys.DATE_BE_MORE_THAN_BEFORE,
        DateTimeOffsetRangeMode.AtLeast => StringKeys.DATE_BE_AT_LEAST_BEFORE,
        _ => StringKeys.DATE_BE_EXACTLY_BEFORE,
    };

    /// <summary>Gets the localization key for the failure message of a value after the anchor.</summary>
    /// <returns>The template key matching the configured range mode.</returns>
    private string AfterKey() => _mode switch {
        DateTimeOffsetRangeMode.Within => StringKeys.DATE_BE_WITHIN_AFTER,
        DateTimeOffsetRangeMode.LessThan => StringKeys.DATE_BE_LESS_THAN_AFTER,
        DateTimeOffsetRangeMode.MoreThan => StringKeys.DATE_BE_MORE_THAN_AFTER,
        DateTimeOffsetRangeMode.AtLeast => StringKeys.DATE_BE_AT_LEAST_AFTER,
        _ => StringKeys.DATE_BE_EXACTLY_AFTER,
    };

    private static string Format(DateTimeOffset value) => AssertionScope.FormatInvariant(value);
}

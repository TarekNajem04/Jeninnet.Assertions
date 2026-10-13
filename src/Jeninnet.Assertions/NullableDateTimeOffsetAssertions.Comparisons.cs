//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

public partial class NullableDateTimeOffsetAssertions {
    /// <summary>Asserts that the value is before <paramref name="expected"/>.</summary>
    /// <param name="expected">The exclusive upper bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or not before <paramref name="expected"/>.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> BeBefore(DateTimeOffset expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.CompareTo(expected) >= 0) {
            Fail(Strings.Format(StringKeys.DATE_BE_BEFORE, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is not before <paramref name="expected"/>. A <see langword="null"/> subject passes.</summary>
    /// <param name="expected">The value the subject must not precede.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is before <paramref name="expected"/>.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> NotBeBefore(DateTimeOffset expected, string? because = null, params object?[] becauseArgs) {
        if(_value?.CompareTo(expected) < 0) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE_BEFORE, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is on or before <paramref name="expected"/>.</summary>
    /// <param name="expected">The inclusive upper bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or after <paramref name="expected"/>.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> BeOnOrBefore(DateTimeOffset expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.CompareTo(expected) > 0) {
            Fail(Strings.Format(StringKeys.DATE_BE_ON_OR_BEFORE, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is after <paramref name="expected"/>.</summary>
    /// <param name="expected">The exclusive lower bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or not after <paramref name="expected"/>.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> BeAfter(DateTimeOffset expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.CompareTo(expected) <= 0) {
            Fail(Strings.Format(StringKeys.DATE_BE_AFTER, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is not after <paramref name="expected"/>. A <see langword="null"/> subject passes.</summary>
    /// <param name="expected">The value the subject must not follow.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is after <paramref name="expected"/>.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> NotBeAfter(DateTimeOffset expected, string? because = null, params object?[] becauseArgs) {
        if(_value?.CompareTo(expected) > 0) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE_AFTER, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is on or after <paramref name="expected"/>.</summary>
    /// <param name="expected">The inclusive lower bound.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or before <paramref name="expected"/>.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> BeOnOrAfter(DateTimeOffset expected, string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.Value.CompareTo(expected) < 0) {
            Fail(Strings.Format(StringKeys.DATE_BE_ON_OR_AFTER, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is within <paramref name="precision"/> of <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="precision">The maximum accepted absolute difference (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="precision"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The value is <see langword="null"/> or differs by more than <paramref name="precision"/>.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> BeCloseTo(DateTimeOffset expected, TimeSpan precision, string? because = null, params object?[] becauseArgs) {
        if(precision < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(precision), "Precision must not be negative.");
        }

        if(_value is null || (_value.Value - expected).Duration() > precision) {
            Fail(Strings.Format(StringKeys.DATE_BE_CLOSE_TO, Expression, Format(expected), AssertionScope.FormatInvariant(precision), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value differs by more than <paramref name="precision"/> from <paramref name="unexpected"/>. A <see langword="null"/> subject passes.</summary>
    /// <param name="unexpected">The value the subject must not approximate.</param>
    /// <param name="precision">The absolute difference that counts as close (must not be negative).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="precision"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The value is within <paramref name="precision"/> of <paramref name="unexpected"/>.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> NotBeCloseTo(DateTimeOffset unexpected, TimeSpan precision, string? because = null, params object?[] becauseArgs) {
        if(precision < TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(precision), "Precision must not be negative.");
        }

        if(_value is not null && (_value.Value - unexpected).Duration() <= precision) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE_CLOSE_TO, Expression, Format(unexpected), AssertionScope.FormatInvariant(precision), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is not on or after <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The value the subject must strictly precede.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is on or after <paramref name="unexpected"/>.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> NotBeOnOrAfter(DateTimeOffset unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value?.CompareTo(unexpected) >= 0) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE_ON_OR_AFTER, Expression, Format(unexpected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the value is not on or before <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The value the subject must strictly follow.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The value is on or before <paramref name="unexpected"/>.</exception>
    public AndConstraint<NullableDateTimeOffsetAssertions> NotBeOnOrBefore(DateTimeOffset unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value?.CompareTo(unexpected) <= 0) {
            Fail(Strings.Format(StringKeys.DATE_NOT_BE_ON_OR_BEFORE, Expression, Format(unexpected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }
}

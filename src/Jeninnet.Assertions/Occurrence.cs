//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>
/// Describes how many times something is expected to occur, used by assertions such as
/// <c>Contain("x", Exactly.Twice())</c> or <c>MatchRegex("p", AtLeast.Once())</c>.
/// The accepted range is spanned by <see cref="Minimum"/> and <see cref="Maximum"/> (both inclusive),
/// and <see cref="Expectation"/> carries a human-readable description (e.g. <c>exactly twice</c>) for failure messages.
/// </summary>
public readonly record struct OccurrenceConstraint {
    /// <summary>Creates an occurrence constraint.</summary>
    /// <param name="minimum">The minimum accepted number of occurrences (inclusive).</param>
    /// <param name="maximum">The maximum accepted number of occurrences (inclusive).</param>
    /// <param name="expectation">A human-readable description (e.g. <c>exactly twice</c>) used in failure messages.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="minimum"/> is negative.</exception>
    /// <exception cref="ArgumentException"><paramref name="maximum"/> is less than <paramref name="minimum"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="expectation"/> is <see langword="null"/>.</exception>
    public OccurrenceConstraint(int minimum, int maximum, string expectation) {
        if(minimum < 0) {
            throw new ArgumentOutOfRangeException(nameof(minimum));
        }

        if(expectation is null) {
            throw new ArgumentNullException(nameof(expectation));
        }

        if(maximum < minimum) {
            throw new ArgumentException("Maximum must be greater than or equal to minimum.", nameof(maximum));
        }

        Minimum = minimum;
        Maximum = maximum;
        Expectation = expectation;
    }

    /// <summary>Gets the minimum accepted number of occurrences (inclusive).</summary>
    public int Minimum { get; init; }

    /// <summary>Gets the maximum accepted number of occurrences (inclusive).</summary>
    public int Maximum { get; init; }

    /// <summary>Gets a human-readable description (e.g. <c>exactly twice</c>) used in failure messages.</summary>
    public string Expectation { get; init; }

    /// <summary>Determines whether <paramref name="count"/> satisfies this constraint.</summary>
    /// <param name="count">The actual number of occurrences.</param>
    /// <returns><see langword="true"/> when <paramref name="count"/> is within range; otherwise <see langword="false"/>.</returns>
    public bool Matches(int count) => count >= Minimum && count <= Maximum;
}

/// <summary>Factory for exact occurrence constraints (<c>Exactly.Once()</c>, <c>Exactly.Times(3)</c>).</summary>
public static class Exactly {
    /// <summary>Expects exactly one occurrence.</summary>
    public static OccurrenceConstraint Once() => new(1, 1, "exactly once");

    /// <summary>Expects exactly two occurrences.</summary>
    public static OccurrenceConstraint Twice() => new(2, 2, "exactly twice");

    /// <summary>Expects exactly three occurrences.</summary>
    public static OccurrenceConstraint Thrice() => new(3, 3, "exactly thrice");

    /// <summary>Expects exactly <paramref name="count"/> occurrences.</summary>
    /// <param name="count">The expected number of occurrences (must not be negative).</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
    public static OccurrenceConstraint Times(int count) {
        if(count < 0) {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        return new(count, count, $"exactly {TimesWord(count)}");
    }

    internal static string TimesWord(int count) => count switch {
        1 => "once",
        2 => "twice",
        3 => "thrice",
        _ => $"{count} times",
    };
}

/// <summary>Factory for lower-bound occurrence constraints (<c>AtLeast.Once()</c>, <c>AtLeast.Times(2)</c>).</summary>
public static class AtLeast {
    /// <summary>Expects at least one occurrence.</summary>
    public static OccurrenceConstraint Once() => new(1, int.MaxValue, "at least once");

    /// <summary>Expects at least two occurrences.</summary>
    public static OccurrenceConstraint Twice() => new(2, int.MaxValue, "at least twice");

    /// <summary>Expects at least three occurrences.</summary>
    public static OccurrenceConstraint Thrice() => new(3, int.MaxValue, "at least thrice");

    /// <summary>Expects at least <paramref name="count"/> occurrences.</summary>
    /// <param name="count">The minimum number of occurrences (must not be negative).</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
    public static OccurrenceConstraint Times(int count) {
        if(count < 0) {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        return new(count, int.MaxValue, $"at least {Exactly.TimesWord(count)}");
    }
}

/// <summary>Factory for upper-bound occurrence constraints (<c>AtMost.Once()</c>, <c>AtMost.Times(2)</c>).</summary>
public static class AtMost {
    /// <summary>Expects at most one occurrence.</summary>
    public static OccurrenceConstraint Once() => new(0, 1, "at most once");

    /// <summary>Expects at most two occurrences.</summary>
    public static OccurrenceConstraint Twice() => new(0, 2, "at most twice");

    /// <summary>Expects at most three occurrences.</summary>
    public static OccurrenceConstraint Thrice() => new(0, 3, "at most thrice");

    /// <summary>Expects at most <paramref name="count"/> occurrences.</summary>
    /// <param name="count">The maximum number of occurrences (must not be negative).</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
    public static OccurrenceConstraint Times(int count) {
        if(count < 0) {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        return new(0, count, $"at most {Exactly.TimesWord(count)}");
    }
}

/// <summary>Factory for strict lower-bound occurrence constraints (<c>MoreThan.Once()</c>, <c>MoreThan.Times(2)</c>).</summary>
public static class MoreThan {
    /// <summary>Expects more than one occurrence.</summary>
    public static OccurrenceConstraint Once() => new(2, int.MaxValue, "more than once");

    /// <summary>Expects more than two occurrences.</summary>
    public static OccurrenceConstraint Twice() => new(3, int.MaxValue, "more than twice");

    /// <summary>Expects more than three occurrences.</summary>
    public static OccurrenceConstraint Thrice() => new(4, int.MaxValue, "more than thrice");

    /// <summary>Expects more than <paramref name="count"/> occurrences.</summary>
    /// <param name="count">The exclusive lower bound (must not be negative).</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
    public static OccurrenceConstraint Times(int count) {
        if(count < 0) {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        return new(count + 1, int.MaxValue, $"more than {Exactly.TimesWord(count)}");
    }
}

/// <summary>Factory for strict upper-bound occurrence constraints (<c>LessThan.Twice()</c>, <c>LessThan.Times(3)</c>).</summary>
public static class LessThan {
    /// <summary>Expects fewer than one occurrence (i.e. none).</summary>
    public static OccurrenceConstraint Once() => new(0, 0, "less than once");

    /// <summary>Expects fewer than two occurrences.</summary>
    public static OccurrenceConstraint Twice() => new(0, 1, "less than twice");

    /// <summary>Expects fewer than three occurrences.</summary>
    public static OccurrenceConstraint Thrice() => new(0, 2, "less than thrice");

    /// <summary>Expects fewer than <paramref name="count"/> occurrences.</summary>
    /// <param name="count">The exclusive upper bound (must be at least 1).</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is less than 1.</exception>
    public static OccurrenceConstraint Times(int count) {
        if(count < 1) {
            throw new ArgumentOutOfRangeException(nameof(count), "Value must be at least 1.");
        }

        return new(0, count - 1, $"less than {Exactly.TimesWord(count)}");
    }
}

/// <summary>Provides fluent occurrence sugar on integers (e.g. <c>3.TimesExactly()</c>).</summary>
public static class OccurrenceExtensions {
    /// <summary>Creates an exact occurrence constraint for <paramref name="count"/> (equivalent to <c>Exactly.Times(count)</c>).</summary>
    /// <param name="count">The expected number of occurrences (must not be negative).</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
    public static OccurrenceConstraint TimesExactly(this int count) => Exactly.Times(count);

    /// <summary>Creates a lower-bound occurrence constraint for <paramref name="count"/> (equivalent to <c>AtLeast.Times(count)</c>).</summary>
    /// <param name="count">The minimum number of occurrences (must not be negative).</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
    public static OccurrenceConstraint TimesOrMore(this int count) => AtLeast.Times(count);

    /// <summary>Creates an upper-bound occurrence constraint for <paramref name="count"/> (equivalent to <c>AtMost.Times(count)</c>).</summary>
    /// <param name="count">The maximum number of occurrences (must not be negative).</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
    public static OccurrenceConstraint TimesOrLess(this int count) => AtMost.Times(count);
}

//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Equality assertions with a custom comparer for reference-type subjects.</summary>
/// <remarks>
/// The comparer stays typed (<c>IEqualityComparer&lt;TValue&gt;</c>) instead of collapsing to
/// <c>IEqualityComparer&lt;object&gt;</c>: variance does not convert a typed comparer to an
/// object comparer, so the object form would force callers into explicit casts. The
/// <c>where TValue : class</c> constraint restricts these overloads to reference types at compile
/// time — value types keep their native assertions. (A generic <c>Be&lt;T&gt;</c> over the subject
/// type itself could never be invoked: <c>Should(object)</c> always yields
/// <c>ObjectAssertions&lt;object&gt;</c> while the argument would pull <c>T</c> toward a derived
/// type, and generic inference fails on that conflict.)
/// </remarks>
public static class ObjectComparerExtensions {
    /// <summary>Asserts that the value equals <paramref name="expected"/> using <paramref name="comparer"/>.</summary>
    /// <typeparam name="TValue">The reference type under assertion.</typeparam>
    /// <param name="assertions">The object assertions to continue from.</param>
    /// <param name="expected">The expected value.</param>
    /// <param name="comparer">The comparer (a <see langword="null"/> value selects the default equality).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assertions"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The values are not equal under <paramref name="comparer"/>.</exception>
    public static AndConstraint<ObjectAssertions<object>> Be<TValue>(
        this ObjectAssertions<object> assertions,
        TValue? expected,
        IEqualityComparer<TValue>? comparer,
        string? because = null,
        params object?[] becauseArgs) where TValue : class {
        if(assertions is null) {
            throw new ArgumentNullException(nameof(assertions));
        }

        var equality = comparer ?? EqualityComparer<TValue>.Default;
        var subject = assertions.Subject;
        if(!ComparerEquals(equality, subject as TValue, expected)) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.OBJECT_BE, assertions.Expression, Format(expected), Format(subject)), because, becauseArgs));
        }

        return new(assertions);
    }

    /// <summary>Asserts that the value does not equal <paramref name="unexpected"/> using <paramref name="comparer"/>.</summary>
    /// <typeparam name="TValue">The reference type under assertion.</typeparam>
    /// <param name="assertions">The object assertions to continue from.</param>
    /// <param name="unexpected">The value the subject must not equal.</param>
    /// <param name="comparer">The comparer (a <see langword="null"/> value selects the default equality).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assertions"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The values are equal under <paramref name="comparer"/>.</exception>
    public static AndConstraint<ObjectAssertions<object>> NotBe<TValue>(
        this ObjectAssertions<object> assertions,
        TValue? unexpected,
        IEqualityComparer<TValue>? comparer,
        string? because = null,
        params object?[] becauseArgs) where TValue : class {
        if(assertions is null) {
            throw new ArgumentNullException(nameof(assertions));
        }

        var equality = comparer ?? EqualityComparer<TValue>.Default;
        var subject = assertions.Subject;
        if(ComparerEquals(equality, subject as TValue, unexpected)) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.OBJECT_NOT_BE, assertions.Expression, Format(unexpected), Format(subject)), because, becauseArgs));
        }

        return new(assertions);
    }

    /// <summary>Asserts that the value equals one of the <paramref name="expected"/> candidates using <paramref name="comparer"/>.</summary>
    /// <typeparam name="TValue">The reference type under assertion.</typeparam>
    /// <param name="assertions">The object assertions to continue from.</param>
    /// <param name="expected">The candidate values, one of which must equal the subject.</param>
    /// <param name="comparer">The comparer (a <see langword="null"/> value selects the default equality).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assertions"/> or <paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The value equals none of the candidates.</exception>
    public static AndConstraint<ObjectAssertions<object>> BeOneOf<TValue>(
        this ObjectAssertions<object> assertions,
        TValue?[] expected,
        IEqualityComparer<TValue>? comparer,
        string? because = null,
        params object?[] becauseArgs) where TValue : class {
        if(assertions is null) {
            throw new ArgumentNullException(nameof(assertions));
        }

        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var equality = comparer ?? EqualityComparer<TValue>.Default;
        var subject = assertions.Subject;
        if(!expected.Any(candidate => ComparerEquals(equality, subject as TValue, candidate))) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.OBJECT_BE_ONE_OF, assertions.Expression, string.Join(", ", expected.Select(Format)), Format(subject)), because, becauseArgs));
        }

        return new(assertions);
    }

    /// <summary>Compares two maybe-null values with <paramref name="equality"/> (comparers accept null operands by contract).</summary>
    /// <typeparam name="TValue">The reference type under comparison.</typeparam>
    /// <param name="equality">The comparer to use.</param>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    /// <returns>The comparer's verdict.</returns>
    // The operators are required on netstandard2.0 (unannotated reference assemblies)
    // while modern targets ignore them.
    private static bool ComparerEquals<TValue>(IEqualityComparer<TValue> equality, TValue? left, TValue? right) where TValue : class {
#pragma warning disable RCS1249, IDE0022 // Build-verified: removing the operators fails netstandard2.0 (CS8604 ×2); RCS1249 fires only where annotations make them redundant (net6.0). The block body must stay to host it, so IDE0022's expression body is illegal here.
        return equality.Equals(left!, right!);
#pragma warning restore RCS1249, IDE0022
    }

    /// <summary>Formats a value invariantly for failure messages.</summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="value">The value to format.</param>
    /// <returns>The invariant text.</returns>
    private static string Format<TValue>(TValue value) => AssertionScope.FormatInvariant(value);
}

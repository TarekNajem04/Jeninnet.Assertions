//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>
/// Immutable, machine-readable description of a single soft-assertion failure collected by
/// <see cref="SoftAssertions"/>, ready for CI attachment or tooling via <see cref="SoftAssertions.ToReportJson"/>.
/// </summary>
/// <remarks>
/// Every collected failure carries the subject <see cref="Expression"/>, the <see cref="Category"/>
/// (the concrete assertion class that reported it) and the formatted <see cref="Message"/>. Values for
/// <see cref="Actual"/> and <see cref="Expected"/> are populated when the reporting assertion supplies them
/// (e.g. <c>BeTrue</c>/<c>BeFalse</c>); message-only failures keep those two entries <see langword="null"/>.
/// </remarks>
/// <param name="expression">The subject expression of the failed assertion.</param>
/// <param name="category">The concrete assertion class that reported the failure.</param>
/// <param name="message">The fully formatted failure message (including any <c>because</c> clause).</param>
/// <param name="actual">The formatted actual value, when the assertion reports one.</param>
/// <param name="expected">The formatted expected value, when the assertion reports one.</param>
/// <exception cref="ArgumentNullException"><paramref name="expression"/>, <paramref name="category"/> or <paramref name="message"/> is <see langword="null"/>.</exception>
public sealed class AssertionFailure(
    string expression,
    string category,
    string message,
    string? actual = null,
    string? expected = null) {
    /// <summary>Gets the subject expression of the failed assertion.</summary>
    public string Expression { get; } = expression ?? throw new ArgumentNullException(nameof(expression));

    /// <summary>Gets the concrete assertion class that reported the failure.</summary>
    public string Category { get; } = category ?? throw new ArgumentNullException(nameof(category));

    /// <summary>Gets the fully formatted failure message (including any <c>because</c> clause).</summary>
    public string Message { get; } = message ?? throw new ArgumentNullException(nameof(message));

    /// <summary>Gets the formatted actual value, or <see langword="null"/> when the verb does not report one.</summary>
    public string? Actual { get; } = actual;

    /// <summary>Gets the formatted expected value, or <see langword="null"/> when the verb does not report one.</summary>
    public string? Expected { get; } = expected;
}

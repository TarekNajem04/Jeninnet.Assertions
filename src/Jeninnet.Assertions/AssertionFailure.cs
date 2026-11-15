//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>
/// Immutable, machine-readable description of a single soft-assertion failure collected by
/// <see cref="SoftAssertions"/>, ready for CI attachment or tooling via <see cref="SoftAssertions.ToReport(AssertionReportFormat)"/>.
/// </summary>
/// <remarks>
/// Every collected failure carries the subject <see cref="Expression"/>, the <see cref="Category"/>
/// (the concrete assertion class that reported it) and the formatted <see cref="Message"/>. Values for
/// <see cref="Actual"/> and <see cref="Expected"/> are populated when the reporting assertion supplies them
/// (e.g. <c>BeTrue</c>/<c>BeFalse</c>); message-only failures keep those two entries <see langword="null"/>.
/// Optional <see cref="RuleId"/>/<see cref="Severity"/> and <see cref="File"/>/<see cref="Line"/>/<see cref="Column"/>
/// feed SARIF, JUnit, and GitHub annotations. Reports never scrub values automatically: callers are responsible
/// for keeping sensitive data out of <see cref="Message"/>/<see cref="Actual"/>/<see cref="Expected"/>.
/// </remarks>
/// <param name="expression">The subject expression of the failed assertion.</param>
/// <param name="category">The concrete assertion class that reported the failure.</param>
/// <param name="message">The fully formatted failure message (including any <c>because</c> clause).</param>
/// <param name="actual">The formatted actual value, when the assertion reports one.</param>
/// <param name="expected">The formatted expected value, when the assertion reports one.</param>
/// <param name="ruleId">The stable report rule ID; defaults to <paramref name="category"/>.</param>
/// <param name="severity">The report severity (<c>error</c>, <c>warning</c>, <c>note</c>); defaults to <c>error</c>.</param>
/// <param name="file">The source file for the failure location, when known.</param>
/// <param name="line">The one-based source line, when known.</param>
/// <param name="column">The one-based source column, when known.</param>
/// <exception cref="ArgumentNullException"><paramref name="expression"/>, <paramref name="category"/> or <paramref name="message"/> is <see langword="null"/>.</exception>
public sealed class AssertionFailure( // NOSONAR: public immutable report DTO; splitting the 10-field constructor would break callers.
    string expression,
    string category,
    string message,
    string? actual = null,
    string? expected = null,
    string? ruleId = null,
    string? severity = null,
    string? file = null,
    int? line = null,
    int? column = null) {
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

    /// <summary>Gets the stable report rule ID; defaults to <see cref="Category"/>.</summary>
    public string RuleId { get; } = Normalize(ruleId, category);

    /// <summary>Gets the report severity (<c>error</c>, <c>warning</c>, or <c>note</c>); defaults to <c>error</c>.</summary>
    public string Severity { get; } = Normalize(severity, "error");

    /// <summary>Gets the source file for the failure location, when known.</summary>
    public string? File { get; } = file;

    /// <summary>Gets the one-based source line for the failure location, when known.</summary>
    public int? Line { get; } = line;

    /// <summary>Gets the one-based source column for the failure location, when known.</summary>
    public int? Column { get; } = column;

    /// <summary>Creates a failure with the original five-field shape (no report metadata).</summary>
    /// <param name="expression">The subject expression of the failed assertion.</param>
    /// <param name="category">The concrete assertion class that reported the failure.</param>
    /// <param name="message">The fully formatted failure message.</param>
    /// <param name="actual">The formatted actual value, when reported.</param>
    /// <param name="expected">The formatted expected value, when reported.</param>
    /// <exception cref="ArgumentNullException"><paramref name="expression"/>, <paramref name="category"/> or <paramref name="message"/> is <see langword="null"/>.</exception>
    public AssertionFailure(string expression, string category, string message, string? actual, string? expected)
        : this(expression, category, message, actual, expected, null, null, null, null, null) {
    }

    /// <summary>Returns <paramref name="value"/> when non-blank, otherwise <paramref name="fallback"/>.</summary>
    /// <param name="value">The optional value.</param>
    /// <param name="fallback">The fallback value.</param>
    /// <returns>The normalized value.</returns>
    private static string Normalize(string? value, string fallback) =>
        value is null || value.Trim().Length == 0 ? fallback : value;
}

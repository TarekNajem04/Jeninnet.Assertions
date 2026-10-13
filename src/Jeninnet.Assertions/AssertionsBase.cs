//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Marker interface implemented by every assertion class in this library.</summary>
public interface IAssertions {
}

/// <summary>
/// Shared plumbing for all assertion classes: the caller expression that names the subject
/// under test and the uniform failure-reporting tail that feeds the active
/// <see cref="AssertionScope"/> (including the structured <see cref="AssertionFailure"/> used by
/// <see cref="SoftAssertions"/> and <see cref="SoftAssertions.ToReportJson"/>).
/// </summary>
/// <param name="expression">The caller expression captured by <c>Should()</c>.</param>
/// <param name="fallbackName">Display name used when the expression could not be captured.</param>
/// <remarks>
/// Shared members are intentionally hidden from the public surface (only the marker
/// <see cref="IAssertions"/> and this base type are public additions), so the microsurface
/// of each <c>XXXAssertions</c> class stays unchanged.
/// </remarks>
public abstract class AssertionsBase(string? expression, string fallbackName) : IAssertions {
    private readonly string _expression = expression ?? fallbackName;

    /// <summary>Gets the caller expression naming the subject under test (for extensions in this assembly).</summary>
    internal string Expression => _expression;

    /// <summary>
    /// Reports an assertion failure by appending the <paramref name="because"/> reason to <paramref name="message"/>
    /// and delegating to the active <see cref="AssertionScope"/>. Protected because it is shared plumbing for
    /// assertion classes, and hidden from the public microsurface.
    /// </summary>
    /// <param name="message">The formatted assertion message (already built against the subject expression).</param>
    /// <param name="because">The optional user-supplied reason, or <see langword="null"/>.</param>
    /// <param name="becauseArgs">Optional format arguments for <paramref name="because"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    protected void Fail(string message, string? because, object?[]? becauseArgs) {
        if(message is null) {
            throw new ArgumentNullException(nameof(message));
        }

        Report(message, because, becauseArgs, null, null);
    }

    /// <summary>
    /// Reports an assertion failure by appending the <paramref name="because"/> reason and wrapping
    /// <paramref name="innerException"/>. Protected because it is shared plumbing for assertion classes,
    /// and hidden from the public microsurface.
    /// </summary>
    /// <param name="message">The formatted assertion message (already built against the subject expression).</param>
    /// <param name="because">The optional user-supplied reason, or <see langword="null"/>.</param>
    /// <param name="becauseArgs">Optional format arguments for <paramref name="because"/>.</param>
    /// <param name="innerException">The exception that caused the failure, to preserve the failure chain.</param>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    protected void Fail(string message, string? because, object?[]? becauseArgs, Exception innerException) {
        if(message is null) {
            throw new ArgumentNullException(nameof(message));
        }

        Report(message, because, becauseArgs, null, null, innerException);
    }

    /// <summary>
    /// Reports an assertion failure with an already-formatted message. Protected because it is shared plumbing
    /// for assertion classes, and hidden from the public microsurface.
    /// </summary>
    /// <param name="message">The fully formatted assertion message (including diagnostics, when applicable).</param>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    protected void Fail(string message) {
        if(message is null) {
            throw new ArgumentNullException(nameof(message));
        }

        Report(message, null, null, null, null);
    }

    /// <summary>
    /// Reports an assertion failure and attaches the formatted <paramref name="actual"/> and
    /// <paramref name="expected"/> values so <see cref="SoftAssertions"/> can serialize them into a
    /// structured JSON report. Protected because it is shared plumbing for assertion classes, and hidden
    /// from the public microsurface.
    /// </summary>
    /// <param name="message">The formatted assertion message (already built against the subject expression).</param>
    /// <param name="because">The optional user-supplied reason, or <see langword="null"/>.</param>
    /// <param name="becauseArgs">Optional format arguments for <paramref name="because"/>.</param>
    /// <param name="actual">The formatted actual value, or <see langword="null"/> when not reported.</param>
    /// <param name="expected">The formatted expected value, or <see langword="null"/> when not reported.</param>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    protected void Fail(string message, string? because, object?[]? becauseArgs, string? actual, string? expected) {
        if(message is null) {
            throw new ArgumentNullException(nameof(message));
        }

        Report(message, because, becauseArgs, actual, expected);
    }

    /// <summary>Formats the reason clause and routes the structured failure to the active assertion scope.</summary>
    /// <param name="message">The formatted assertion message.</param>
    /// <param name="because">The optional user-supplied reason.</param>
    /// <param name="becauseArgs">Optional format arguments for <paramref name="because"/>.</param>
    /// <param name="actual">The formatted actual value, or <see langword="null"/>.</param>
    /// <param name="expected">The formatted expected value, or <see langword="null"/>.</param>
    /// <param name="innerException">The underlying exception to preserve, when any.</param>
    private void Report(string message, string? because, object?[]? becauseArgs, string? actual, string? expected, Exception? innerException = null) {
        var formatted = AssertionScope.AppendBecause(message, because, becauseArgs);
        AssertionScope.Fail(new AssertionFailure(Expression, GetType().Name, formatted, actual, expected), innerException);
    }
}

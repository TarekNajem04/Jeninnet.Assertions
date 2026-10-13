//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>
/// Provides access to a caught exception after a <c>Throw</c> or <c>ThrowAsync</c> assertion succeeds,
/// enabling further inspection of the exception instance.
/// </summary>
/// <typeparam name="TException">The type of the caught exception.</typeparam>
/// <param name="exception">The caught exception instance to expose.</param>
public class ExceptionAssertions<TException>(TException exception) where TException : Exception {
    /// <summary>Gets the caught exception instance.</summary>
    /// <exception cref="AssertionFailedException">Thrown if the exception instance is <see langword="null"/>.</exception>
    public TException Exception { get; } = exception ?? throw new AssertionFailedException("Exception cannot be null.");

    /// <summary>Gets the caught exception instance for continued assertions.</summary>
    public TException Which => Exception;

    private static string Format(string? value) => value is null ? "<null>" : $"'{value}'";

    /// <summary>Asserts that the exception message matches the wildcard <paramref name="pattern"/> (<c>*</c> and <c>?</c>).</summary>
    /// <param name="pattern">The wildcard pattern the message must match (case-sensitive, whole message).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pattern"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The message does not match.</exception>
    public AndConstraint<ExceptionAssertions<TException>> WithMessage(string pattern, string? because = null, params object?[] becauseArgs) {
        if(pattern is null) {
            throw new ArgumentNullException(nameof(pattern));
        }

        if(!TextMatching.WildcardMatch(Exception.Message, pattern, ignoreCase: false)) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.EXCEPTION_MESSAGE_MATCH, Format(pattern), Format(Exception.Message)), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that the exception message does not match the wildcard <paramref name="pattern"/> (<c>*</c> and <c>?</c>).</summary>
    /// <param name="pattern">The wildcard pattern the message must not match (case-sensitive, whole message).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="pattern"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The message matches.</exception>
    public AndConstraint<ExceptionAssertions<TException>> WithoutMessage(string pattern, string? because = null, params object?[] becauseArgs) {
        if(pattern is null) {
            throw new ArgumentNullException(nameof(pattern));
        }

        if(TextMatching.WildcardMatch(Exception.Message, pattern, ignoreCase: false)) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.EXCEPTION_MESSAGE_NOT_MATCH, Format(pattern), Format(Exception.Message)), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that the inner exception is of type <typeparamref name="TInner"/> (derived types accepted).</summary>
    /// <typeparam name="TInner">The expected inner exception type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The inner exception is missing or of a different type.</exception>
    public AndConstraint<ExceptionAssertions<TException>> WithInnerException<TInner>(string? because = null, params object?[] becauseArgs) where TInner : Exception {
        if(Exception.InnerException is not TInner) {
            var actual = Exception.InnerException?.GetType().Name ?? "<null>";
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.EXCEPTION_INNER_BE, typeof(TInner).Name, actual), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that the inner exception is exactly of type <typeparamref name="TInner"/> (derived types fail).</summary>
    /// <typeparam name="TInner">The expected exact inner exception type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The inner exception is missing or its type is not exactly <typeparamref name="TInner"/>.</exception>
    public AndConstraint<ExceptionAssertions<TException>> WithInnerExceptionExactly<TInner>(string? because = null, params object?[] becauseArgs) where TInner : Exception =>
        WithInnerExceptionExactly(typeof(TInner), because, becauseArgs);

    /// <summary>Asserts that the inner exception is exactly of type <paramref name="expectedType"/> (derived types fail).</summary>
    /// <param name="expectedType">The expected exact inner exception type.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expectedType"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="expectedType"/> is not an exception type.</exception>
    /// <exception cref="AssertionFailedException">The inner exception is missing or its type is not exactly <paramref name="expectedType"/>.</exception>
    public AndConstraint<ExceptionAssertions<TException>> WithInnerExceptionExactly(Type expectedType, string? because = null, params object?[] becauseArgs) {
        if(expectedType is null) {
            throw new ArgumentNullException(nameof(expectedType));
        }

        if(!typeof(Exception).IsAssignableFrom(expectedType)) {
            throw new ArgumentException("The expected type must be an exception type.", nameof(expectedType));
        }

        if(Exception.InnerException?.GetType() != expectedType) {
            var actual = Exception.InnerException?.GetType().Name ?? "<null>";
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.EXCEPTION_INNER_BE_EXACTLY, expectedType.Name, actual), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that the exception satisfies <paramref name="predicate"/>.</summary>
    /// <param name="predicate">The condition the exception must satisfy. Exceptions thrown by the predicate propagate.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The exception does not satisfy <paramref name="predicate"/>.</exception>
    public AndConstraint<ExceptionAssertions<TException>> Where(Expression<Func<TException, bool>> predicate, string? because = null, params object?[] becauseArgs) {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        if(!predicate.Compile().Invoke(Exception)) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.EXCEPTION_MATCH, predicate), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that the exception carries parameter name <paramref name="expected"/> (requires an <see cref="ArgumentException"/>).</summary>
    /// <param name="expected">The expected parameter name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The exception is not an <see cref="ArgumentException"/> with that parameter name.</exception>
    public AndConstraint<ExceptionAssertions<TException>> WithParameterName(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        var actual = (Exception as ArgumentException)?.ParamName;
        if(actual != expected) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.EXCEPTION_PARAM_NAME, Format(expected), Format(actual)), because, becauseArgs));
        }

        return new(this);
    }
}

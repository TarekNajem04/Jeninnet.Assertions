//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
#if NET8_0_OR_GREATER
namespace Jeninnet.Assertions;

public partial class StringAssertions {
    /// <summary>Asserts that the string parses into <typeparamref name="T"/> and exposes the parsed value.</summary>
    /// <typeparam name="T">The target type, which must implement <see cref="IParsable{TSelf}"/>.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndWhichConstraint{TParent, T}"/> wrapping the parsed value.</returns>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/> or cannot be parsed into <typeparamref name="T"/>.</exception>
    /// <remarks>The parsed value exposed through <c>Which</c> is only meaningful when the assertion succeeds.</remarks>
    public AndWhichConstraint<StringAssertions, T> BeParsableInto<T>(string? because = null, params object?[] becauseArgs) where T : IParsable<T> =>
        BeParsableInto<T>(CultureInfo.InvariantCulture, because, becauseArgs);

    /// <summary>Asserts that the string parses into <typeparamref name="T"/> using <paramref name="formatProvider"/>.</summary>
    /// <typeparam name="T">The target type, which must implement <see cref="IParsable{TSelf}"/>.</typeparam>
    /// <param name="formatProvider">The format provider used for parsing; <see langword="null"/> selects the invariant culture.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndWhichConstraint{TParent, T}"/> wrapping the parsed value.</returns>
    /// <exception cref="AssertionFailedException">The string is <see langword="null"/> or cannot be parsed into <typeparamref name="T"/>.</exception>
    /// <remarks>The parsed value exposed through <c>Which</c> is only meaningful when the assertion succeeds.</remarks>
    public AndWhichConstraint<StringAssertions, T> BeParsableInto<T>(IFormatProvider? formatProvider, string? because = null, params object?[] becauseArgs) where T : IParsable<T> {
        var provider = formatProvider ?? CultureInfo.InvariantCulture;
        if(_value is null) {
            Fail(Strings.Format(StringKeys.STRING_BE_PARSABLE_NULL, Expression, typeof(T).Name), because, becauseArgs);
            return new(this, default!);
        }

        try {
            return new(this, T.Parse(_value, provider));
        }
        catch(Exception exception) {
            Fail(Strings.Format(StringKeys.STRING_BE_PARSABLE, Expression, Format(_value), typeof(T).Name, ParseFailure(exception)), because, becauseArgs);
            return new(this, default!);
        }
    }

    /// <summary>Asserts that the string cannot be parsed into <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The target type, which must implement <see cref="IParsable{TSelf}"/>.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The string parses successfully into <typeparamref name="T"/>.</exception>
    public AndConstraint<StringAssertions> NotBeParsableInto<T>(string? because = null, params object?[] becauseArgs) where T : IParsable<T> =>
        NotBeParsableInto<T>(CultureInfo.InvariantCulture, because, becauseArgs);

    /// <summary>Asserts that the string cannot be parsed into <typeparamref name="T"/> using <paramref name="formatProvider"/>.</summary>
    /// <typeparam name="T">The target type, which must implement <see cref="IParsable{TSelf}"/>.</typeparam>
    /// <param name="formatProvider">The format provider used for parsing; <see langword="null"/> selects the invariant culture.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The string parses successfully into <typeparamref name="T"/>.</exception>
    public AndConstraint<StringAssertions> NotBeParsableInto<T>(IFormatProvider? formatProvider, string? because = null, params object?[] becauseArgs) where T : IParsable<T> {
        var provider = formatProvider ?? CultureInfo.InvariantCulture;
        if(T.TryParse(_value, provider, out _)) {
            Fail(Strings.Format(StringKeys.STRING_NOT_BE_PARSABLE, Expression, Format(_value), typeof(T).Name), because, becauseArgs);
        }

        return new(this);
    }

    private static string ParseFailure(Exception exception) {
        var message = exception.Message.Replace('\r', ' ').Replace('\n', ' ').Trim().TrimEnd('.');
        if(message.Length == 0) {
            return exception.GetType().Name;
        }

        return message.Length <= 200 ? message : message[..197] + "...";
    }
}
#endif

//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Provides assertion methods for verifying a <see cref="char"/> value and its Unicode category.</summary>
/// <param name="value">The character to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public sealed class CharAssertions(char value, string? expression = null) : AssertionsBase(expression, "char") {
    private readonly char _value = value;

    /// <summary>Asserts that the character equals <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected character.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The characters differ.</exception>
    public AndConstraint<CharAssertions> Be(char expected, string? because = null, params object?[] becauseArgs) {
        if(_value != expected) {
            Fail(Strings.Format(StringKeys.CHAR_BE, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the character does not equal <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The character the subject must not equal.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The characters are equal.</exception>
    public AndConstraint<CharAssertions> NotBe(char unexpected, string? because = null, params object?[] becauseArgs) {
        if(_value == unexpected) {
            Fail(Strings.Format(StringKeys.CHAR_NOT_BE, Expression, Format(unexpected)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the character is a Unicode letter.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The character is not a letter.</exception>
    public AndConstraint<CharAssertions> BeLetter(string? because = null, params object?[] becauseArgs) =>
        AssertCategory(char.IsLetter, StringKeys.CHAR_CATEGORY_LETTER, negated: false, because, becauseArgs);

    /// <summary>Asserts that the character is not a Unicode letter.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The character is a letter.</exception>
    public AndConstraint<CharAssertions> NotBeLetter(string? because = null, params object?[] becauseArgs) =>
        AssertCategory(char.IsLetter, StringKeys.CHAR_CATEGORY_LETTER, negated: true, because, becauseArgs);

    /// <summary>Asserts that the character is a decimal digit.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The character is not a digit.</exception>
    public AndConstraint<CharAssertions> BeDigit(string? because = null, params object?[] becauseArgs) =>
        AssertCategory(char.IsDigit, StringKeys.CHAR_CATEGORY_DIGIT, negated: false, because, becauseArgs);

    /// <summary>Asserts that the character is not a decimal digit.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The character is a digit.</exception>
    public AndConstraint<CharAssertions> NotBeDigit(string? because = null, params object?[] becauseArgs) =>
        AssertCategory(char.IsDigit, StringKeys.CHAR_CATEGORY_DIGIT, negated: true, because, becauseArgs);

    /// <summary>Asserts that the character is a letter or a decimal digit.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The character is neither a letter nor a digit.</exception>
    public AndConstraint<CharAssertions> BeLetterOrDigit(string? because = null, params object?[] becauseArgs) =>
        AssertCategory(char.IsLetterOrDigit, StringKeys.CHAR_CATEGORY_LETTER_OR_DIGIT, negated: false, because, becauseArgs);

    /// <summary>Asserts that the character is neither a letter nor a decimal digit.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The character is a letter or a digit.</exception>
    public AndConstraint<CharAssertions> NotBeLetterOrDigit(string? because = null, params object?[] becauseArgs) =>
        AssertCategory(char.IsLetterOrDigit, StringKeys.CHAR_CATEGORY_LETTER_OR_DIGIT, negated: true, because, becauseArgs);

    /// <summary>Asserts that the character is white space.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The character is not white space.</exception>
    public AndConstraint<CharAssertions> BeWhiteSpace(string? because = null, params object?[] becauseArgs) =>
        AssertCategory(char.IsWhiteSpace, StringKeys.CHAR_CATEGORY_WHITESPACE, negated: false, because, becauseArgs);

    /// <summary>Asserts that the character is not white space.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The character is white space.</exception>
    public AndConstraint<CharAssertions> NotBeWhiteSpace(string? because = null, params object?[] becauseArgs) =>
        AssertCategory(char.IsWhiteSpace, StringKeys.CHAR_CATEGORY_WHITESPACE, negated: true, because, becauseArgs);

    /// <summary>Asserts that the character is Unicode punctuation.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The character is not punctuation.</exception>
    public AndConstraint<CharAssertions> BePunctuation(string? because = null, params object?[] becauseArgs) =>
        AssertCategory(IsPunctuation, StringKeys.CHAR_CATEGORY_PUNCTUATION, negated: false, because, becauseArgs);

    /// <summary>Asserts that the character is not Unicode punctuation.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The character is punctuation.</exception>
    public AndConstraint<CharAssertions> NotBePunctuation(string? because = null, params object?[] becauseArgs) =>
        AssertCategory(IsPunctuation, StringKeys.CHAR_CATEGORY_PUNCTUATION, negated: true, because, becauseArgs);

    /// <summary>Asserts that the character is a Unicode symbol.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The character is not a symbol.</exception>
    public AndConstraint<CharAssertions> BeSymbol(string? because = null, params object?[] becauseArgs) =>
        AssertCategory(IsSymbol, StringKeys.CHAR_CATEGORY_SYMBOL, negated: false, because, becauseArgs);

    /// <summary>Asserts that the character is not a Unicode symbol.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The character is a symbol.</exception>
    public AndConstraint<CharAssertions> NotBeSymbol(string? because = null, params object?[] becauseArgs) =>
        AssertCategory(IsSymbol, StringKeys.CHAR_CATEGORY_SYMBOL, negated: true, because, becauseArgs);

    /// <summary>Asserts that the character is a control character.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The character is not a control character.</exception>
    public AndConstraint<CharAssertions> BeControl(string? because = null, params object?[] becauseArgs) =>
        AssertCategory(char.IsControl, StringKeys.CHAR_CATEGORY_CONTROL, negated: false, because, becauseArgs);

    /// <summary>Asserts that the character is not a control character.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The character is a control character.</exception>
    public AndConstraint<CharAssertions> NotBeControl(string? because = null, params object?[] becauseArgs) =>
        AssertCategory(char.IsControl, StringKeys.CHAR_CATEGORY_CONTROL, negated: true, because, becauseArgs);

    /// <summary>Asserts that the character is a surrogate code unit.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The character is not a surrogate.</exception>
    public AndConstraint<CharAssertions> BeSurrogate(string? because = null, params object?[] becauseArgs) =>
        AssertCategory(char.IsSurrogate, StringKeys.CHAR_CATEGORY_SURROGATE, negated: false, because, becauseArgs);

    /// <summary>Asserts that the character is not a surrogate code unit.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The character is a surrogate.</exception>
    public AndConstraint<CharAssertions> NotBeSurrogate(string? because = null, params object?[] becauseArgs) =>
        AssertCategory(char.IsSurrogate, StringKeys.CHAR_CATEGORY_SURROGATE, negated: true, because, becauseArgs);

    /// <summary>Asserts that the character is an uppercase letter.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The character is not an uppercase letter.</exception>
    public AndConstraint<CharAssertions> BeUpper(string? because = null, params object?[] becauseArgs) =>
        AssertCategory(char.IsUpper, StringKeys.CHAR_CATEGORY_UPPER, negated: false, because, becauseArgs);

    /// <summary>Asserts that the character is not an uppercase letter.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The character is an uppercase letter.</exception>
    public AndConstraint<CharAssertions> NotBeUpper(string? because = null, params object?[] becauseArgs) =>
        AssertCategory(char.IsUpper, StringKeys.CHAR_CATEGORY_UPPER, negated: true, because, becauseArgs);

    /// <summary>Asserts that the character is a lowercase letter.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The character is not a lowercase letter.</exception>
    public AndConstraint<CharAssertions> BeLower(string? because = null, params object?[] becauseArgs) =>
        AssertCategory(char.IsLower, StringKeys.CHAR_CATEGORY_LOWER, negated: false, because, becauseArgs);

    /// <summary>Asserts that the character is not a lowercase letter.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The character is a lowercase letter.</exception>
    public AndConstraint<CharAssertions> NotBeLower(string? because = null, params object?[] becauseArgs) =>
        AssertCategory(char.IsLower, StringKeys.CHAR_CATEGORY_LOWER, negated: true, because, becauseArgs);

    /// <summary>Asserts that the character is an ASCII character (<c>U+0000</c>–<c>U+007F</c>).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The character is not ASCII.</exception>
    public AndConstraint<CharAssertions> BeAscii(string? because = null, params object?[] becauseArgs) =>
        AssertCategory(static value => value <= '\u007F', StringKeys.CHAR_CATEGORY_ASCII, negated: false, because, becauseArgs);

    /// <summary>Asserts that the character is not an ASCII character.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The character is ASCII.</exception>
    public AndConstraint<CharAssertions> NotBeAscii(string? because = null, params object?[] becauseArgs) =>
        AssertCategory(static value => value <= '\u007F', StringKeys.CHAR_CATEGORY_ASCII, negated: true, because, becauseArgs);

    private AndConstraint<CharAssertions> AssertCategory(Func<char, bool> predicate, string categoryKey, bool negated, string? because, object?[] becauseArgs) {
        var matches = predicate(_value);
        if(matches == negated) {
            var category = Strings.Get(categoryKey);
            var message = negated
                ? Strings.Format(StringKeys.CHAR_NOT_BE_CATEGORY, Expression, category)
                : Strings.Format(StringKeys.CHAR_BE_CATEGORY, Expression, category, Format(_value));
            Fail(message, because, becauseArgs);
        }

        return new(this);
    }

    private static bool IsPunctuation(char value) => char.GetUnicodeCategory(value) is
        UnicodeCategory.ConnectorPunctuation or
        UnicodeCategory.DashPunctuation or
        UnicodeCategory.OpenPunctuation or
        UnicodeCategory.ClosePunctuation or
        UnicodeCategory.InitialQuotePunctuation or
        UnicodeCategory.FinalQuotePunctuation or
        UnicodeCategory.OtherPunctuation;

    private static bool IsSymbol(char value) => char.GetUnicodeCategory(value) is
        UnicodeCategory.MathSymbol or
        UnicodeCategory.CurrencySymbol or
        UnicodeCategory.ModifierSymbol or
        UnicodeCategory.OtherSymbol;

    private static string Format(char value) => value switch {
        '\0' => @"'\0'",
        '\n' => @"'\n'",
        '\r' => @"'\r'",
        '\t' => @"'\t'",
        '\\' => @"'\\'",
        '\'' => @"'\''",
        _ when char.IsControl(value) || char.IsSurrogate(value) || char.GetUnicodeCategory(value) == UnicodeCategory.Format =>
            $@"'\u{((int)value).ToString("X4", CultureInfo.InvariantCulture)}'",
        _ => $"'{value}'",
    };
}

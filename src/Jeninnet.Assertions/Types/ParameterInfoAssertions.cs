//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

/// <summary>Provides assertion methods for verifying <see cref="ParameterInfo"/> metadata.</summary>
/// <param name="value">The parameter to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
[method: RequiresUnreferencedCode(TrimmingMessages.TYPE_METADATA)]
public sealed class ParameterInfoAssertions(ParameterInfo? value, string? expression = null) : AssertionsBase(expression, "parameter") {
    private const string NULL_LITERAL = "<null>";
    private readonly ParameterInfo? _value = value;

    /// <summary>Asserts that the parameter is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The parameter is not <see langword="null"/>.</exception>
    public AndConstraint<ParameterInfoAssertions> BeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is not null) {
            Fail(Strings.Format(StringKeys.PARAMETER_BE_NULL, Expression, Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the parameter is not <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The parameter is <see langword="null"/>.</exception>
    public AndConstraint<ParameterInfoAssertions> NotBeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.PARAMETER_NOT_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the parameter has the name <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected parameter name (ordinal comparison).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The parameter has another name.</exception>
    public AndConstraint<ParameterInfoAssertions> BeNamed(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(!TryGetSubject(out var subject, because, becauseArgs)) {
            return new(this);
        }

        if(!string.Equals(subject.Name, expected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.PARAMETER_BE_NAMED, Expression, expected, subject.Name ?? NULL_LITERAL), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the parameter does not have the name <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The parameter name the subject must not have (ordinal comparison).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="unexpected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The parameter has that name.</exception>
    public AndConstraint<ParameterInfoAssertions> NotBeNamed(string unexpected, string? because = null, params object?[] becauseArgs) {
        if(unexpected is null) {
            throw new ArgumentNullException(nameof(unexpected));
        }

        if(!TryGetSubject(out var subject, because, becauseArgs)) {
            return new(this);
        }

        if(string.Equals(subject.Name, unexpected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.PARAMETER_NOT_BE_NAMED, Expression, unexpected), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the parameter type is exactly <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The expected parameter type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The parameter has another type.</exception>
    public AndConstraint<ParameterInfoAssertions> BeOfType<T>(string? because = null, params object?[] becauseArgs) =>
        BeOfType(typeof(T), because, becauseArgs);

    /// <summary>Asserts that the parameter type is exactly <paramref name="expectedType"/>.</summary>
    /// <param name="expectedType">The expected parameter type.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expectedType"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The parameter has another type.</exception>
    public AndConstraint<ParameterInfoAssertions> BeOfType(Type expectedType, string? because = null, params object?[] becauseArgs) {
        if(expectedType is null) {
            throw new ArgumentNullException(nameof(expectedType));
        }

        if(!TryGetSubject(out var subject, because, becauseArgs)) {
            return new(this);
        }

        if(subject.ParameterType != expectedType) {
            Fail(Strings.Format(StringKeys.PARAMETER_BE_OF_TYPE, Expression, Format(expectedType), Format(subject.ParameterType)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the parameter type is not exactly <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The parameter type the subject must not have.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The parameter has exactly that type.</exception>
    public AndConstraint<ParameterInfoAssertions> NotBeOfType<T>(string? because = null, params object?[] becauseArgs) =>
        NotBeOfType(typeof(T), because, becauseArgs);

    /// <summary>Asserts that the parameter type is not exactly <paramref name="unexpectedType"/>.</summary>
    /// <param name="unexpectedType">The parameter type the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="unexpectedType"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The parameter has exactly that type.</exception>
    public AndConstraint<ParameterInfoAssertions> NotBeOfType(Type unexpectedType, string? because = null, params object?[] becauseArgs) {
        if(unexpectedType is null) {
            throw new ArgumentNullException(nameof(unexpectedType));
        }

        if(!TryGetSubject(out var subject, because, becauseArgs)) {
            return new(this);
        }

        if(subject.ParameterType == unexpectedType) {
            Fail(Strings.Format(StringKeys.PARAMETER_NOT_BE_OF_TYPE, Expression, Format(unexpectedType)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the parameter is optional (<see cref="ParameterInfo.IsOptional"/>).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The parameter is required.</exception>
    public AndConstraint<ParameterInfoAssertions> BeOptional(string? because = null, params object?[] becauseArgs) {
        if(!TryGetSubject(out var subject, because, becauseArgs)) {
            return new(this);
        }

        if(!subject.IsOptional) {
            Fail(Strings.Format(StringKeys.PARAMETER_BE_OPTIONAL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the parameter is required (not <see cref="ParameterInfo.IsOptional"/>).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The parameter is optional.</exception>
    public AndConstraint<ParameterInfoAssertions> BeRequired(string? because = null, params object?[] becauseArgs) {
        if(!TryGetSubject(out var subject, because, becauseArgs)) {
            return new(this);
        }

        if(subject.IsOptional) {
            Fail(Strings.Format(StringKeys.PARAMETER_BE_REQUIRED, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the parameter has the default value <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected default value; <see langword="null"/> matches an explicit <see langword="null"/> default.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The parameter has no default value or a different one.</exception>
    public AndConstraint<ParameterInfoAssertions> HaveDefaultValue(object? expected, string? because = null, params object?[] becauseArgs) {
        if(!TryGetSubject(out var subject, because, becauseArgs)) {
            return new(this);
        }

        var expectedText = FormatDefault(expected);
        if(!subject.HasDefaultValue) {
            Fail(Strings.Format(StringKeys.PARAMETER_HAVE_NO_DEFAULT_VALUE, Expression, expectedText), because, becauseArgs);
        } else if(!Equals(subject.DefaultValue, expected)) {
            Fail(Strings.Format(StringKeys.PARAMETER_HAVE_DEFAULT_VALUE, Expression, expectedText, FormatDefault(subject.DefaultValue)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the parameter has no default value.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The parameter has a default value.</exception>
    public AndConstraint<ParameterInfoAssertions> NotHaveDefaultValue(string? because = null, params object?[] becauseArgs) {
        if(!TryGetSubject(out var subject, because, becauseArgs)) {
            return new(this);
        }

        if(subject.HasDefaultValue) {
            Fail(Strings.Format(StringKeys.PARAMETER_NOT_HAVE_DEFAULT_VALUE, Expression, FormatDefault(subject.DefaultValue)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the parameter is an <c>out</c> parameter.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The parameter is not an <c>out</c> parameter.</exception>
    public AndConstraint<ParameterInfoAssertions> BeOut(string? because = null, params object?[] becauseArgs) =>
        AssertModifier(StringKeys.PARAMETER_MODIFIER_OUT, IsOut, negated: false, because, becauseArgs);

    /// <summary>Asserts that the parameter is not an <c>out</c> parameter.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The parameter is an <c>out</c> parameter.</exception>
    public AndConstraint<ParameterInfoAssertions> NotBeOut(string? because = null, params object?[] becauseArgs) =>
        AssertModifier(StringKeys.PARAMETER_MODIFIER_OUT, IsOut, negated: true, because, becauseArgs);

    /// <summary>Asserts that the parameter is an <c>in</c> parameter.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The parameter is not an <c>in</c> parameter.</exception>
    public AndConstraint<ParameterInfoAssertions> BeIn(string? because = null, params object?[] becauseArgs) =>
        AssertModifier(StringKeys.PARAMETER_MODIFIER_IN, IsIn, negated: false, because, becauseArgs);

    /// <summary>Asserts that the parameter is not an <c>in</c> parameter.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The parameter is an <c>in</c> parameter.</exception>
    public AndConstraint<ParameterInfoAssertions> NotBeIn(string? because = null, params object?[] becauseArgs) =>
        AssertModifier(StringKeys.PARAMETER_MODIFIER_IN, IsIn, negated: true, because, becauseArgs);

    /// <summary>Asserts that the parameter is a <c>ref</c> parameter.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The parameter is not a <c>ref</c> parameter.</exception>
    public AndConstraint<ParameterInfoAssertions> BeRef(string? because = null, params object?[] becauseArgs) =>
        AssertModifier(StringKeys.PARAMETER_MODIFIER_REF, IsRef, negated: false, because, becauseArgs);

    /// <summary>Asserts that the parameter is not a <c>ref</c> parameter.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The parameter is a <c>ref</c> parameter.</exception>
    public AndConstraint<ParameterInfoAssertions> NotBeRef(string? because = null, params object?[] becauseArgs) =>
        AssertModifier(StringKeys.PARAMETER_MODIFIER_REF, IsRef, negated: true, because, becauseArgs);

    /// <summary>Asserts that the parameter is a <c>params</c> parameter.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The parameter is not a <c>params</c> parameter.</exception>
    public AndConstraint<ParameterInfoAssertions> BeParams(string? because = null, params object?[] becauseArgs) =>
        AssertModifier(StringKeys.PARAMETER_MODIFIER_PARAMS, IsParams, negated: false, because, becauseArgs);

    /// <summary>Asserts that the parameter is not a <c>params</c> parameter.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The parameter is a <c>params</c> parameter.</exception>
    public AndConstraint<ParameterInfoAssertions> NotBeParams(string? because = null, params object?[] becauseArgs) =>
        AssertModifier(StringKeys.PARAMETER_MODIFIER_PARAMS, IsParams, negated: true, because, becauseArgs);

    /// <summary>Asserts that the parameter is at <paramref name="expected"/> position.</summary>
    /// <param name="expected">The expected zero-based position (the return parameter is <c>-1</c>).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The parameter is at another position.</exception>
    public AndConstraint<ParameterInfoAssertions> HavePosition(int expected, string? because = null, params object?[] becauseArgs) {
        if(!TryGetSubject(out var subject, because, becauseArgs)) {
            return new(this);
        }

        if(subject.Position != expected) {
            Fail(Strings.Format(StringKeys.PARAMETER_HAVE_POSITION, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(subject.Position)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the parameter is not at <paramref name="unexpected"/> position.</summary>
    /// <param name="unexpected">The position the parameter must not occupy.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The parameter is at that position.</exception>
    public AndConstraint<ParameterInfoAssertions> NotHavePosition(int unexpected, string? because = null, params object?[] becauseArgs) {
        if(!TryGetSubject(out var subject, because, becauseArgs)) {
            return new(this);
        }

        if(subject.Position == unexpected) {
            Fail(Strings.Format(StringKeys.PARAMETER_NOT_HAVE_POSITION, Expression, AssertionScope.FormatInvariant(unexpected)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the parameter is decorated with <typeparamref name="TAttribute"/> and exposes the first matching attribute.</summary>
    /// <typeparam name="TAttribute">The expected attribute type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndWhichConstraint{TParent, T}"/> wrapping the first matching attribute.</returns>
    /// <exception cref="AssertionFailedException">The attribute is absent.</exception>
    public AndWhichConstraint<ParameterInfoAssertions, TAttribute> BeDecoratedWith<TAttribute>(string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        if(!TryGetSubject(out var subject, because, becauseArgs)) {
            return new(this, null!);
        }

        var attribute = subject.GetCustomAttributes(typeof(TAttribute), inherit: false).OfType<TAttribute>().FirstOrDefault();
        if(attribute is null) {
            Fail(Strings.Format(StringKeys.PARAMETER_BE_DECORATED, Expression, typeof(TAttribute).Name), because, becauseArgs);
            return new(this, null!);
        }

        return new(this, attribute);
    }

    /// <summary>Asserts that the parameter is not decorated with <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type that must be absent.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The attribute is present.</exception>
    public AndConstraint<ParameterInfoAssertions> NotBeDecoratedWith<TAttribute>(string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        if(!TryGetSubject(out var subject, because, becauseArgs)) {
            return new(this);
        }

        if(subject.IsDefined(typeof(TAttribute), inherit: false)) {
            Fail(Strings.Format(StringKeys.PARAMETER_NOT_BE_DECORATED, Expression, typeof(TAttribute).Name), because, becauseArgs);
        }

        return new(this);
    }

    private AndConstraint<ParameterInfoAssertions> AssertModifier(string modifierKey, Func<ParameterInfo, bool> predicate, bool negated, string? because, object?[] becauseArgs) {
        if(!TryGetSubject(out var subject, because, becauseArgs)) {
            return new(this);
        }

        var matches = predicate(subject);
        if(matches == negated) {
            var modifier = Strings.Get(modifierKey);
            var message = negated
                ? Strings.Format(StringKeys.PARAMETER_NOT_BE_MODIFIER, Expression, modifier)
                : Strings.Format(StringKeys.PARAMETER_BE_MODIFIER, Expression, modifier);
            Fail(message, because, becauseArgs);
        }

        return new(this);
    }

    private bool TryGetSubject(out ParameterInfo subject, string? because, object?[] becauseArgs) {
        subject = _value!;
        if(_value is not null) {
            return true;
        }

        Fail(Strings.Format(StringKeys.PARAMETER_IS_NULL, Expression), because, becauseArgs);
        return false;
    }

    private static bool IsOut(ParameterInfo value) =>
        value.ParameterType.IsByRef && (value.Attributes & ParameterAttributes.Out) != 0;

    private static bool IsIn(ParameterInfo value) =>
        value.ParameterType.IsByRef && (value.Attributes & ParameterAttributes.In) != 0;

    private static bool IsRef(ParameterInfo value) =>
        value.ParameterType.IsByRef && (value.Attributes & (ParameterAttributes.In | ParameterAttributes.Out)) == 0;

    private static bool IsParams(ParameterInfo value) =>
        value.IsDefined(typeof(ParamArrayAttribute), inherit: false);

    private static string Format(ParameterInfo? value) =>
        value is null ? NULL_LITERAL : $"{value.ParameterType.Name} {value.Name ?? "<return>"}";

    private static string Format(Type? value) => value is null ? NULL_LITERAL : value.Name;

    private static string FormatDefault(object? value) => value is null ? NULL_LITERAL : FormatValue(value);
}

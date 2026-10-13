//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

/// <summary>Provides assertion methods for verifying <see cref="ConstructorInfo"/> metadata.</summary>
/// <param name="value">The constructor to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class ConstructorInfoAssertions(ConstructorInfo? value, string? expression = null) : AssertionsBase(expression, "constructor") {
    private readonly ConstructorInfo? _value = value;

    /// <summary>Asserts that the constructor is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The constructor is not <see langword="null"/>.</exception>
    public AndConstraint<ConstructorInfoAssertions> BeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is not null) {
            Fail(Strings.Format(StringKeys.CTOR_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the constructor is not <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The constructor is <see langword="null"/>.</exception>
    public AndConstraint<ConstructorInfoAssertions> NotBeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the constructor declares a parameter named <paramref name="name"/> of type <paramref name="type"/>.</summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="type">The parameter type.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The constructor is <see langword="null"/> or has no such parameter.</exception>
    public AndConstraint<ConstructorInfoAssertions> HaveParameter(string name, Type type, string? because = null, params object?[] becauseArgs) {
        if(!MethodInfoAssertions.HasParameter(_value, name, type)) {
            Fail(Strings.Format(StringKeys.METHOD_HAVE_PARAMETER, Expression, name, type.Name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the constructor is public.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The constructor is <see langword="null"/> or not public.</exception>
    public AndConstraint<ConstructorInfoAssertions> BePublic(string? because = null, params object?[] becauseArgs) {
        if(_value?.IsPublic != true) {
            Fail(Strings.Format(StringKeys.CTOR_BE_PUBLIC, Expression, _value is null ? "found null" : "it was not"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the constructor is decorated with <typeparamref name="TAttribute"/> (including inherited attributes).</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The constructor is <see langword="null"/> or lacks the attribute.</exception>
    public AndConstraint<ConstructorInfoAssertions> BeDecoratedWith<TAttribute>(string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        if(_value is null || _value.GetCustomAttributes(typeof(TAttribute), inherit: true).Length == 0) {
            Fail(Strings.Format(StringKeys.METHOD_BE_DECORATED, Expression, typeof(TAttribute).Name, _value is null ? "found null" : "it was not"), because, becauseArgs);
        }

        return new(this);
    }
}

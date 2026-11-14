//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

public partial class MethodInfoAssertions {
    /// <summary>Asserts that the method is abstract.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The method is <see langword="null"/> or not abstract.</exception>
    public AndConstraint<MethodInfoAssertions> BeAbstract(string? because = null, params object?[] becauseArgs) {
        if(_value?.IsAbstract != true) {
            Fail(Strings.Format(StringKeys.MEMBER_BE_ABSTRACT, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method is not abstract.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The method is abstract.</exception>
    public AndConstraint<MethodInfoAssertions> NotBeAbstract(string? because = null, params object?[] becauseArgs) {
        if(_value?.IsAbstract == true) {
            Fail(Strings.Format(StringKeys.MEMBER_NOT_BE_ABSTRACT, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method overrides a virtual method declared by a base type.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The method is <see langword="null"/> or not an override.</exception>
    public AndConstraint<MethodInfoAssertions> BeOverride(string? because = null, params object?[] becauseArgs) {
        if(_value is null || !ReflectionMetadata.IsOverride(_value)) {
            Fail(Strings.Format(StringKeys.MEMBER_BE_OVERRIDE, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method does not override a virtual method declared by a base type.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The method is an override.</exception>
    public AndConstraint<MethodInfoAssertions> NotBeOverride(string? because = null, params object?[] becauseArgs) {
        if(_value is not null && ReflectionMetadata.IsOverride(_value)) {
            Fail(Strings.Format(StringKeys.MEMBER_NOT_BE_OVERRIDE, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method is final (a sealed override).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The method is <see langword="null"/> or not final.</exception>
    public AndConstraint<MethodInfoAssertions> BeFinal(string? because = null, params object?[] becauseArgs) {
        if(_value?.IsFinal != true) {
            Fail(Strings.Format(StringKeys.MEMBER_BE_FINAL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method is not final.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The method is final.</exception>
    public AndConstraint<MethodInfoAssertions> NotBeFinal(string? because = null, params object?[] becauseArgs) {
        if(_value?.IsFinal == true) {
            Fail(Strings.Format(StringKeys.MEMBER_NOT_BE_FINAL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method is generic.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The method is <see langword="null"/> or not generic.</exception>
    public AndConstraint<MethodInfoAssertions> BeGeneric(string? because = null, params object?[] becauseArgs) {
        if(_value?.IsGenericMethod != true) {
            Fail(Strings.Format(StringKeys.MEMBER_BE_GENERIC, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method is not generic.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The method is generic.</exception>
    public AndConstraint<MethodInfoAssertions> NotBeGeneric(string? because = null, params object?[] becauseArgs) {
        if(_value?.IsGenericMethod == true) {
            Fail(Strings.Format(StringKeys.MEMBER_NOT_BE_GENERIC, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method is an extension method.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The method is <see langword="null"/> or not an extension method.</exception>
    public AndConstraint<MethodInfoAssertions> BeExtension(string? because = null, params object?[] becauseArgs) {
        if(_value is null || !ReflectionMetadata.IsExtension(_value)) {
            Fail(Strings.Format(StringKeys.MEMBER_BE_EXTENSION, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method is not an extension method.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The method is an extension method.</exception>
    public AndConstraint<MethodInfoAssertions> NotBeExtension(string? because = null, params object?[] becauseArgs) {
        if(_value is not null && ReflectionMetadata.IsExtension(_value)) {
            Fail(Strings.Format(StringKeys.MEMBER_NOT_BE_EXTENSION, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method declares exactly the given parameter types, in order.</summary>
    /// <param name="parameterTypes">The exact parameter types (empty for a parameterless method).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="parameterTypes"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The method is <see langword="null"/> or has another parameter list.</exception>
    public AndConstraint<MethodInfoAssertions> HaveParameters(Type[] parameterTypes, string? because = null, params object?[] becauseArgs) {
        if(parameterTypes is null) {
            throw new ArgumentNullException(nameof(parameterTypes));
        }

        var actual = _value?.GetParameters();
        if(actual is null || !actual.Select(static parameter => parameter.ParameterType).SequenceEqual(parameterTypes)) {
            Fail(Strings.Format(StringKeys.MEMBER_HAVE_PARAMETERS, Expression, ReflectionMetadata.FormatParameters(parameterTypes), actual is null ? "<null>" : ReflectionMetadata.FormatParameters(actual)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method is decorated with <typeparamref name="TAttribute"/> matching <paramref name="predicate"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="predicate">The condition at least one attribute must satisfy.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The method is <see langword="null"/> or has no matching attribute.</exception>
    public AndConstraint<MethodInfoAssertions> BeDecoratedWith<TAttribute>(Expression<Func<TAttribute, bool>> predicate, string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        if(_value?.GetCustomAttributes(typeof(TAttribute), inherit: true).OfType<TAttribute>().Any(predicate.Compile()) != true) {
            Fail(Strings.Format(StringKeys.MEMBER_BE_DECORATED_MATCH, Expression, typeof(TAttribute).Name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method is not decorated with <typeparamref name="TAttribute"/> matching <paramref name="predicate"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="predicate">The condition no attribute may satisfy.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">A matching attribute is present.</exception>
    public AndConstraint<MethodInfoAssertions> NotBeDecoratedWith<TAttribute>(Expression<Func<TAttribute, bool>> predicate, string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        if(_value?.GetCustomAttributes(typeof(TAttribute), inherit: true).OfType<TAttribute>().Any(predicate.Compile()) == true) {
            Fail(Strings.Format(StringKeys.MEMBER_NOT_BE_DECORATED_MATCH, Expression, typeof(TAttribute).Name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method is the same reflected instance as <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected method instance.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The instances differ.</exception>
    public AndConstraint<MethodInfoAssertions> BeSameAs(MethodInfo? expected, string? because = null, params object?[] becauseArgs) {
        if(!ReferenceEquals(_value, expected)) {
            Fail(Strings.Format(StringKeys.MEMBER_BE_SAME_AS, Expression, expected?.Name ?? "<null>"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method is not the same reflected instance as <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The method instance the subject must not be.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The instances are the same.</exception>
    public AndConstraint<MethodInfoAssertions> NotBeSameAs(MethodInfo? unexpected, string? because = null, params object?[] becauseArgs) {
        if(ReferenceEquals(_value, unexpected)) {
            Fail(Strings.Format(StringKeys.MEMBER_NOT_BE_SAME_AS, Expression, unexpected?.Name ?? "<null>"), because, becauseArgs);
        }

        return new(this);
    }
}

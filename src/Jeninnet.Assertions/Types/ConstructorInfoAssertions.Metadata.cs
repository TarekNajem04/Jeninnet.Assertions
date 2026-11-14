//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

public partial class ConstructorInfoAssertions {
    private const string FOUND_NULL = "found null";
    private const string WAS_NOT = "it was not";

    /// <summary>Asserts that the constructor is private.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The constructor is <see langword="null"/> or not private.</exception>
    public AndConstraint<ConstructorInfoAssertions> BePrivate(string? because = null, params object?[] becauseArgs) =>
        HaveAccess(AccessModifier.Private, StringKeys.CTOR_BE_PRIVATE, because, becauseArgs);

    /// <summary>Asserts that the constructor is protected.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The constructor is <see langword="null"/> or not protected.</exception>
    public AndConstraint<ConstructorInfoAssertions> BeProtected(string? because = null, params object?[] becauseArgs) =>
        HaveAccess(AccessModifier.Protected, StringKeys.CTOR_BE_PROTECTED, because, becauseArgs);

    /// <summary>Asserts that the constructor is internal.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The constructor is <see langword="null"/> or not internal.</exception>
    public AndConstraint<ConstructorInfoAssertions> BeInternal(string? because = null, params object?[] becauseArgs) =>
        HaveAccess(AccessModifier.Internal, StringKeys.CTOR_BE_INTERNAL, because, becauseArgs);

    /// <summary>Asserts that the constructor is not public.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The constructor is public.</exception>
    public AndConstraint<ConstructorInfoAssertions> NotBePublic(string? because = null, params object?[] becauseArgs) {
        if(_value?.IsPublic == true) {
            Fail(Strings.Format(StringKeys.CTOR_NOT_BE_PUBLIC, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the constructor is a static constructor.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The constructor is <see langword="null"/> or not static.</exception>
    public AndConstraint<ConstructorInfoAssertions> BeStatic(string? because = null, params object?[] becauseArgs) {
        if(_value?.IsStatic != true) {
            Fail(Strings.Format(StringKeys.CTOR_BE_STATIC, Expression, _value is null ? FOUND_NULL : WAS_NOT), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the constructor declares exactly the given parameter types, in order.</summary>
    /// <param name="parameterTypes">The exact parameter types (empty for a parameterless constructor).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="parameterTypes"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The constructor is <see langword="null"/> or has another parameter list.</exception>
    public AndConstraint<ConstructorInfoAssertions> HaveParameters(Type[] parameterTypes, string? because = null, params object?[] becauseArgs) {
        if(parameterTypes is null) {
            throw new ArgumentNullException(nameof(parameterTypes));
        }

        var actual = _value?.GetParameters();
        if(actual is null || !actual.Select(static parameter => parameter.ParameterType).SequenceEqual(parameterTypes)) {
            Fail(Strings.Format(StringKeys.MEMBER_HAVE_PARAMETERS, Expression, ReflectionMetadata.FormatParameters(parameterTypes), actual is null ? "<null>" : ReflectionMetadata.FormatParameters(actual)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the constructor is decorated with <typeparamref name="TAttribute"/> matching <paramref name="predicate"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="predicate">The condition at least one attribute must satisfy.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The constructor is <see langword="null"/> or has no matching attribute.</exception>
    public AndConstraint<ConstructorInfoAssertions> BeDecoratedWith<TAttribute>(Expression<Func<TAttribute, bool>> predicate, string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        if(_value?.GetCustomAttributes(typeof(TAttribute), inherit: true).OfType<TAttribute>().Any(predicate.Compile()) != true) {
            Fail(Strings.Format(StringKeys.MEMBER_BE_DECORATED_MATCH, Expression, typeof(TAttribute).Name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the constructor is not decorated with <typeparamref name="TAttribute"/> matching <paramref name="predicate"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="predicate">The condition no attribute may satisfy.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">A matching attribute is present.</exception>
    public AndConstraint<ConstructorInfoAssertions> NotBeDecoratedWith<TAttribute>(Expression<Func<TAttribute, bool>> predicate, string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        if(_value?.GetCustomAttributes(typeof(TAttribute), inherit: true).OfType<TAttribute>().Any(predicate.Compile()) == true) {
            Fail(Strings.Format(StringKeys.MEMBER_NOT_BE_DECORATED_MATCH, Expression, typeof(TAttribute).Name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the constructor is the same reflected instance as <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected constructor instance.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The instances differ.</exception>
    public AndConstraint<ConstructorInfoAssertions> BeSameAs(ConstructorInfo? expected, string? because = null, params object?[] becauseArgs) {
        if(!ReferenceEquals(_value, expected)) {
            Fail(Strings.Format(StringKeys.MEMBER_BE_SAME_AS, Expression, expected?.Name ?? "<null>"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the constructor is not the same reflected instance as <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The constructor instance the subject must not be.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The instances are the same.</exception>
    public AndConstraint<ConstructorInfoAssertions> NotBeSameAs(ConstructorInfo? unexpected, string? because = null, params object?[] becauseArgs) {
        if(ReferenceEquals(_value, unexpected)) {
            Fail(Strings.Format(StringKeys.MEMBER_NOT_BE_SAME_AS, Expression, unexpected?.Name ?? "<null>"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Applies an access-modifier assertion with a constructor-specific message.</summary>
    /// <param name="expected">The expected declared accessibility.</param>
    /// <param name="messageKey">The localization key for the failure message.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    private AndConstraint<ConstructorInfoAssertions> HaveAccess(AccessModifier expected, string messageKey, string? because, object?[] becauseArgs) {
        if(_value is null || ReflectionMetadata.GetAccessModifier(_value) != expected) {
            var actual = _value is null ? FOUND_NULL : $"it was {AssertionScope.FormatInvariant(ReflectionMetadata.GetAccessModifier(_value))}";
            Fail(Strings.Format(messageKey, Expression, actual), because, becauseArgs);
        }

        return new(this);
    }
}

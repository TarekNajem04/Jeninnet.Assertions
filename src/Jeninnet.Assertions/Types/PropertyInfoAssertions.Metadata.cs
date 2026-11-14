//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

public partial class PropertyInfoAssertions {
    /// <summary>Asserts that the property is static (at least one accessor is static).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The property is <see langword="null"/> or not static.</exception>
    public AndConstraint<PropertyInfoAssertions> BeStatic(string? because = null, params object?[] becauseArgs) {
        if(!IsStatic(_value)) {
            Fail(Strings.Format(StringKeys.PROPERTY_BE_STATIC, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property is not static.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The property is static.</exception>
    public AndConstraint<PropertyInfoAssertions> NotBeStatic(string? because = null, params object?[] becauseArgs) {
        if(IsStatic(_value)) {
            Fail(Strings.Format(StringKeys.PROPERTY_NOT_BE_STATIC, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property is abstract (at least one accessor is abstract).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The property is <see langword="null"/> or not abstract.</exception>
    public AndConstraint<PropertyInfoAssertions> BeAbstract(string? because = null, params object?[] becauseArgs) {
        if(!IsAbstract(_value)) {
            Fail(Strings.Format(StringKeys.MEMBER_BE_ABSTRACT, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property is not abstract.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The property is abstract.</exception>
    public AndConstraint<PropertyInfoAssertions> NotBeAbstract(string? because = null, params object?[] becauseArgs) {
        if(IsAbstract(_value)) {
            Fail(Strings.Format(StringKeys.MEMBER_NOT_BE_ABSTRACT, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property overrides a virtual property declared by a base type.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The property is <see langword="null"/> or not an override.</exception>
    public AndConstraint<PropertyInfoAssertions> BeOverride(string? because = null, params object?[] becauseArgs) {
        if(_value is null || !ReflectionMetadata.IsOverride(_value)) {
            Fail(Strings.Format(StringKeys.MEMBER_BE_OVERRIDE, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property does not override a virtual property declared by a base type.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The property is an override.</exception>
    public AndConstraint<PropertyInfoAssertions> NotBeOverride(string? because = null, params object?[] becauseArgs) {
        if(_value is not null && ReflectionMetadata.IsOverride(_value)) {
            Fail(Strings.Format(StringKeys.MEMBER_NOT_BE_OVERRIDE, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property is required (C# 11 <c>required</c> member).</summary>
    /// <remarks>Detection reads <c>RequiredMemberAttribute</c> by full name, so it works on every target framework.</remarks>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The property is <see langword="null"/> or not required.</exception>
    public AndConstraint<PropertyInfoAssertions> BeRequired(string? because = null, params object?[] becauseArgs) {
        if(_value is null || !ReflectionMetadata.IsRequired(_value)) {
            Fail(Strings.Format(StringKeys.PROPERTY_BE_REQUIRED, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property is not required.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The property is required.</exception>
    public AndConstraint<PropertyInfoAssertions> NotBeRequired(string? because = null, params object?[] becauseArgs) {
        if(_value is not null && ReflectionMetadata.IsRequired(_value)) {
            Fail(Strings.Format(StringKeys.PROPERTY_NOT_BE_REQUIRED, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property has an init-only setter.</summary>
    /// <remarks>Detection reads the <c>IsExternalInit</c> required modifier on the setter, so it works on every target framework.</remarks>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The property is <see langword="null"/> or has a regular setter.</exception>
    public AndConstraint<PropertyInfoAssertions> BeInitOnly(string? because = null, params object?[] becauseArgs) {
        if(_value is null || !ReflectionMetadata.IsInitOnly(_value)) {
            Fail(Strings.Format(StringKeys.PROPERTY_BE_INIT_ONLY, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property does not have an init-only setter.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The property has an init-only setter.</exception>
    public AndConstraint<PropertyInfoAssertions> NotBeInitOnly(string? because = null, params object?[] becauseArgs) {
        if(_value is not null && ReflectionMetadata.IsInitOnly(_value)) {
            Fail(Strings.Format(StringKeys.PROPERTY_NOT_BE_INIT_ONLY, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property has exactly the given index parameter types, in order.</summary>
    /// <param name="parameterTypes">The exact index parameter types (empty for a non-indexed property).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="parameterTypes"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The property is <see langword="null"/> or has another index parameter list.</exception>
    public AndConstraint<PropertyInfoAssertions> HaveIndexParameters(Type[] parameterTypes, string? because = null, params object?[] becauseArgs) {
        if(parameterTypes is null) {
            throw new ArgumentNullException(nameof(parameterTypes));
        }

        var actual = _value?.GetIndexParameters();
        if(actual is null || !actual.Select(static parameter => parameter.ParameterType).SequenceEqual(parameterTypes)) {
            Fail(Strings.Format(StringKeys.PROPERTY_HAVE_INDEX_PARAMETERS, Expression, ReflectionMetadata.FormatParameters(parameterTypes), actual is null ? "<null>" : ReflectionMetadata.FormatParameters(actual)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property is decorated with <typeparamref name="TAttribute"/> matching <paramref name="predicate"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="predicate">The condition at least one attribute must satisfy.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The property is <see langword="null"/> or has no matching attribute.</exception>
    public AndConstraint<PropertyInfoAssertions> BeDecoratedWith<TAttribute>(Expression<Func<TAttribute, bool>> predicate, string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        if(_value?.GetCustomAttributes(typeof(TAttribute), inherit: true).OfType<TAttribute>().Any(predicate.Compile()) != true) {
            Fail(Strings.Format(StringKeys.MEMBER_BE_DECORATED_MATCH, Expression, typeof(TAttribute).Name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property is not decorated with <typeparamref name="TAttribute"/> matching <paramref name="predicate"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="predicate">The condition no attribute may satisfy.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">A matching attribute is present.</exception>
    public AndConstraint<PropertyInfoAssertions> NotBeDecoratedWith<TAttribute>(Expression<Func<TAttribute, bool>> predicate, string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        if(_value?.GetCustomAttributes(typeof(TAttribute), inherit: true).OfType<TAttribute>().Any(predicate.Compile()) == true) {
            Fail(Strings.Format(StringKeys.MEMBER_NOT_BE_DECORATED_MATCH, Expression, typeof(TAttribute).Name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property is the same reflected instance as <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected property instance.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The instances differ.</exception>
    public AndConstraint<PropertyInfoAssertions> BeSameAs(PropertyInfo? expected, string? because = null, params object?[] becauseArgs) {
        if(!ReferenceEquals(_value, expected)) {
            Fail(Strings.Format(StringKeys.MEMBER_BE_SAME_AS, Expression, expected?.Name ?? "<null>"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property is not the same reflected instance as <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The property instance the subject must not be.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The instances are the same.</exception>
    public AndConstraint<PropertyInfoAssertions> NotBeSameAs(PropertyInfo? unexpected, string? because = null, params object?[] becauseArgs) {
        if(ReferenceEquals(_value, unexpected)) {
            Fail(Strings.Format(StringKeys.MEMBER_NOT_BE_SAME_AS, Expression, unexpected?.Name ?? "<null>"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Determines whether either property accessor is static.</summary>
    /// <param name="property">The property to inspect (may be <see langword="null"/>).</param>
    /// <returns><see langword="true"/> when the getter or setter is static.</returns>
    private static bool IsStatic(PropertyInfo? property) =>
        property?.GetMethod?.IsStatic == true || property?.SetMethod?.IsStatic == true;

    /// <summary>Determines whether either property accessor is abstract.</summary>
    /// <param name="property">The property to inspect (may be <see langword="null"/>).</param>
    /// <returns><see langword="true"/> when the getter or setter is abstract.</returns>
    private static bool IsAbstract(PropertyInfo? property) =>
        property?.GetMethod?.IsAbstract == true || property?.SetMethod?.IsAbstract == true;
}

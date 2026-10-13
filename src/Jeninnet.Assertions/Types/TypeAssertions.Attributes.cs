//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

public partial class TypeAssertions {
    /// <summary>Asserts that the type is decorated with <typeparamref name="TAttribute"/> (including inherited attributes).</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or lacks the attribute.</exception>
    public AndConstraint<TypeAssertions> BeDecoratedWith<TAttribute>(string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        if(_value is null || _value.GetCustomAttributes(typeof(TAttribute), inherit: true).Length == 0) {
            Fail(Strings.Format(StringKeys.TYPE_BE_DECORATED, Expression, typeof(TAttribute).Name, FoundSuffix()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type is not decorated with <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type that must be absent.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type carries the attribute.</exception>
    public AndConstraint<TypeAssertions> NotBeDecoratedWith<TAttribute>(string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        if(_value is not null && _value.GetCustomAttributes(typeof(TAttribute), inherit: true).Length != 0) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_BE_DECORATED, Expression, typeof(TAttribute).Name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type or any of its base types is decorated with <typeparamref name="TAttribute"/>
    /// (unlike <see cref="BeDecoratedWith{TAttribute}(string?, object?[])"/>, attributes marked non-inherited are found on base types too).</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">Neither the type nor any base type carries the attribute.</exception>
    public AndConstraint<TypeAssertions> BeDecoratedWithOrInherit<TAttribute>(string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        var decorated = false;
        for(var current = _value; current is not null && !decorated; current = current.BaseType) {
            decorated = current.GetCustomAttributes(typeof(TAttribute), inherit: false).Length != 0;
        }

        if(!decorated) {
            Fail(Strings.Format(StringKeys.TYPE_BE_DECORATED_OR_INHERIT, Expression, typeof(TAttribute).Name, FoundSuffix()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type carries <typeparamref name="TAttribute"/> (including inherited attributes) satisfying <paramref name="predicate"/>.</summary>
    /// <remarks>A <see langword="null"/> predicate needs an explicit cast to disambiguate it from the <c>because</c> overload.</remarks>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="predicate">The condition the attribute must satisfy.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The attribute is missing or does not satisfy <paramref name="predicate"/>.</exception>
    public AndConstraint<TypeAssertions> BeDecoratedWith<TAttribute>(Expression<Func<TAttribute, bool>> predicate, string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        var attribute = _value?.GetCustomAttributes(typeof(TAttribute), inherit: true).OfType<TAttribute>().FirstOrDefault();
        if(attribute is null) {
            Fail(Strings.Format(StringKeys.TYPE_BE_DECORATED, Expression, typeof(TAttribute).Name, FoundSuffix()), because, becauseArgs);
        } else if(!predicate.Compile().Invoke(attribute)) {
            Fail(Strings.Format(StringKeys.TYPE_BE_DECORATED, Expression, typeof(TAttribute).Name, "it did not match the predicate"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that neither the type nor any base type carries <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type that must be absent.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The attribute is found on the type or a base type.</exception>
    public AndConstraint<TypeAssertions> NotBeDecoratedWithOrInherit<TAttribute>(string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        var decorated = false;
        for(var current = _value; current is not null && !decorated; current = current.BaseType) {
            decorated = current.GetCustomAttributes(typeof(TAttribute), inherit: false).Length != 0;
        }

        if(decorated) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_BE_DECORATED_OR_INHERIT, Expression, typeof(TAttribute).Name), because, becauseArgs);
        }

        return new(this);
    }
}

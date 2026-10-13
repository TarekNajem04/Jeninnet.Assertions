//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

public partial class TypeAssertions {
    /// <summary>Asserts that the type is assignable to <typeparamref name="TExpected"/>.</summary>
    /// <typeparam name="TExpected">The type the subject must be assignable to.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or not assignable.</exception>
    public AndConstraint<TypeAssertions> BeAssignableTo<TExpected>(string? because = null, params object?[] becauseArgs) {
        if(_value is null || !typeof(TExpected).IsAssignableFrom(_value)) {
            Fail(Strings.Format(StringKeys.TYPE_BE_ASSIGNABLE_TO, Expression, typeof(TExpected).Name, FoundSuffix()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type is not assignable to <typeparamref name="TExpected"/>.</summary>
    /// <typeparam name="TExpected">The type the subject must not be assignable to.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is assignable.</exception>
    public AndConstraint<TypeAssertions> NotBeAssignableTo<TExpected>(string? because = null, params object?[] becauseArgs) {
        if(_value is not null && typeof(TExpected).IsAssignableFrom(_value)) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_BE_ASSIGNABLE_TO, Expression, typeof(TExpected).Name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type implements <typeparamref name="TInterface"/>.</summary>
    /// <typeparam name="TInterface">The interface the subject must implement.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or does not implement the interface.</exception>
    public AndConstraint<TypeAssertions> Implement<TInterface>(string? because = null, params object?[] becauseArgs) =>
        Implement(typeof(TInterface), because, becauseArgs);

    /// <summary>Asserts that the type does not implement <typeparamref name="TInterface"/>.</summary>
    /// <typeparam name="TInterface">The interface the subject must not implement.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type implements the interface.</exception>
    public AndConstraint<TypeAssertions> NotImplement<TInterface>(string? because = null, params object?[] becauseArgs) =>
        NotImplement(typeof(TInterface), because, becauseArgs);

    /// <summary>Asserts that the type implements <paramref name="interfaceType"/>.</summary>
    /// <param name="interfaceType">The interface the subject must implement.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="interfaceType"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or does not implement the interface.</exception>
    public AndConstraint<TypeAssertions> Implement(Type interfaceType, string? because = null, params object?[] becauseArgs) {
        if(interfaceType is null) {
            throw new ArgumentNullException(nameof(interfaceType));
        }

        if(_value is null || !interfaceType.IsAssignableFrom(_value)) {
            Fail(Strings.Format(StringKeys.TYPE_IMPLEMENT, Expression, Format(interfaceType), FoundSuffix()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type does not implement <paramref name="interfaceType"/>.</summary>
    /// <param name="interfaceType">The interface the subject must not implement.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="interfaceType"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The type implements the interface.</exception>
    public AndConstraint<TypeAssertions> NotImplement(Type interfaceType, string? because = null, params object?[] becauseArgs) {
        if(interfaceType is null) {
            throw new ArgumentNullException(nameof(interfaceType));
        }

        if(_value is not null && interfaceType.IsAssignableFrom(_value)) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_IMPLEMENT, Expression, Format(interfaceType)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type derives from <typeparamref name="TBase"/> (excluding the base itself).</summary>
    /// <typeparam name="TBase">The base type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or does not derive from the base.</exception>
    public AndConstraint<TypeAssertions> BeDerivedFrom<TBase>(string? because = null, params object?[] becauseArgs) =>
        BeDerivedFrom(typeof(TBase), because, becauseArgs);

    /// <summary>Asserts that the type does not derive from <typeparamref name="TBase"/>.</summary>
    /// <typeparam name="TBase">The base type the subject must not derive from.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type derives from the base.</exception>
    public AndConstraint<TypeAssertions> NotBeDerivedFrom<TBase>(string? because = null, params object?[] becauseArgs) =>
        NotBeDerivedFrom(typeof(TBase), because, becauseArgs);

    /// <summary>Asserts that the type derives from <paramref name="baseType"/> (excluding the base itself).</summary>
    /// <param name="baseType">The base type.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="baseType"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or does not derive from the base.</exception>
    public AndConstraint<TypeAssertions> BeDerivedFrom(Type baseType, string? because = null, params object?[] becauseArgs) {
        if(baseType is null) {
            throw new ArgumentNullException(nameof(baseType));
        }

        if(_value is null || _value == baseType || !baseType.IsAssignableFrom(_value)) {
            Fail(Strings.Format(StringKeys.TYPE_BE_DERIVED_FROM, Expression, Format(baseType), FoundSuffix()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type does not derive from <paramref name="baseType"/>.</summary>
    /// <param name="baseType">The base type the subject must not derive from.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="baseType"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The type derives from the base.</exception>
    public AndConstraint<TypeAssertions> NotBeDerivedFrom(Type baseType, string? because = null, params object?[] becauseArgs) {
        if(baseType is null) {
            throw new ArgumentNullException(nameof(baseType));
        }

        if(_value is not null && _value != baseType && baseType.IsAssignableFrom(_value)) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_BE_DERIVED_FROM, Expression, Format(baseType)), because, becauseArgs);
        }

        return new(this);
    }
}

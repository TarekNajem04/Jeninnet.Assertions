//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

/// <summary>Provides assertion methods for verifying <see cref="Type"/> metadata.</summary>
/// <param name="value">The type to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public partial class TypeAssertions(Type? value, string? expression = null) : AssertionsBase(expression, "type") {
    private readonly Type? _value = value;
    private const string FOUND_NULL = "found null";
    private const string WAS_NOT = "it was not";

    private static string Format(Type? value) => value is null ? "<null>" : value.FullName ?? value.Name;

    /// <summary>Asserts that the type is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is not <see langword="null"/>.</exception>
    public AndConstraint<TypeAssertions> BeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is not null) {
            Fail(Strings.Format(StringKeys.TYPE_BE_NULL, Expression, Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type is not <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/>.</exception>
    public AndConstraint<TypeAssertions> NotBeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type is sealed.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or not sealed.</exception>
    public AndConstraint<TypeAssertions> BeSealed(string? because = null, params object?[] becauseArgs) {
        if(_value?.IsSealed != true) {
            Fail(Strings.Format(StringKeys.TYPE_BE_SEALED, Expression, _value is null ? FOUND_NULL : WAS_NOT), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type is not sealed.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is sealed.</exception>
    public AndConstraint<TypeAssertions> NotBeSealed(string? because = null, params object?[] becauseArgs) {
        if(_value?.IsSealed == true) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_BE_SEALED, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type is abstract.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or not abstract.</exception>
    public AndConstraint<TypeAssertions> BeAbstract(string? because = null, params object?[] becauseArgs) {
        if(_value?.IsAbstract != true) {
            Fail(Strings.Format(StringKeys.TYPE_BE_ABSTRACT, Expression, _value is null ? FOUND_NULL : WAS_NOT), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type is not abstract.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is abstract.</exception>
    public AndConstraint<TypeAssertions> NotBeAbstract(string? because = null, params object?[] becauseArgs) {
        if(_value?.IsAbstract == true) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_BE_ABSTRACT, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type is declared with the given <paramref name="accessibility"/>.</summary>
    /// <param name="accessibility">The expected declared accessibility.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or has another accessibility.</exception>
    public AndConstraint<TypeAssertions> HaveAccessModifier(AccessModifier accessibility, string? because = null, params object?[] becauseArgs) {
        if(_value is null || GetAccessModifier(_value) != accessibility) {
            Fail(Strings.Format(StringKeys.TYPE_HAVE_ACCESS_MODIFIER, Expression, AssertionScope.FormatInvariant(accessibility), _value is null ? "<null>" : AssertionScope.FormatInvariant(GetAccessModifier(_value))), because, becauseArgs);
        }

        return new(this);
    }

    private string FoundSuffix() => _value is null ? FOUND_NULL : $"found {Format(_value)}";

    /// <summary>Maps a type's declared accessibility to the assertion vocabulary.</summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns>The declared accessibility, handling top-level and nested (including protected internal) cases.</returns>
    private static AccessModifier GetAccessModifier(Type type) {
        if(!type.IsNested) {
            return type.IsPublic ? AccessModifier.Public : AccessModifier.Internal;
        }

        if(type.IsNestedPublic) {
            return AccessModifier.Public;
        }

        if(type.IsNestedPrivate) {
            return AccessModifier.Private;
        }

        if(type.IsNestedFamORAssem) {
            return AccessModifier.ProtectedInternal;
        }

        if(type.IsNestedFamANDAssem) {
            return AccessModifier.PrivateProtected;
        }

        return type.IsNestedFamily ? AccessModifier.Protected : AccessModifier.Internal;
    }

    /// <summary>Asserts that the type is static (abstract and sealed).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or not static.</exception>
    public AndConstraint<TypeAssertions> BeStatic(string? because = null, params object?[] becauseArgs) {
        if(_value?.IsAbstract != true || _value?.IsSealed != true) {
            Fail(Strings.Format(StringKeys.TYPE_BE_STATIC, Expression, _value is null ? FOUND_NULL : WAS_NOT), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type is not static.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is static.</exception>
    public AndConstraint<TypeAssertions> NotBeStatic(string? because = null, params object?[] becauseArgs) {
        if(_value?.IsAbstract == true && _value.IsSealed) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_BE_STATIC, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type is not declared with the given <paramref name="accessibility"/>.</summary>
    /// <param name="accessibility">The declared accessibility the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or has that accessibility.</exception>
    public AndConstraint<TypeAssertions> NotHaveAccessModifier(AccessModifier accessibility, string? because = null, params object?[] becauseArgs) {
        if(_value is null || GetAccessModifier(_value) == accessibility) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_HAVE_ACCESS_MODIFIER, Expression, AssertionScope.FormatInvariant(accessibility), _value is null ? "<null>" : AssertionScope.FormatInvariant(GetAccessModifier(_value))), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type is identical to <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected type identity.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/> (use <c>BeNull</c> instead).</exception>
    /// <exception cref="AssertionFailedException">The types differ.</exception>
    public AndConstraint<TypeAssertions> Be(Type expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(_value != expected) {
            Fail(Strings.Format(StringKeys.TYPE_BE, Expression, Format(expected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type is not identical to <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The type identity the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="unexpected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The types are identical.</exception>
    public AndConstraint<TypeAssertions> NotBe(Type unexpected, string? because = null, params object?[] becauseArgs) {
        if(unexpected is null) {
            throw new ArgumentNullException(nameof(unexpected));
        }

        if(_value == unexpected) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_BE, Expression, Format(unexpected), Format(_value)), because, becauseArgs);
        }

        return new(this);
    }
}

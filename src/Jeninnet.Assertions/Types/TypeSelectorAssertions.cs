//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

/// <summary>Provides assertion methods that must hold for every type in a <see cref="TypeSelector"/> selection.</summary>
/// <param name="types">The selected types.</param>
public class TypeSelectorAssertions(Type[] types) {
    private readonly Type[] _types = types ?? throw new ArgumentNullException(nameof(types));
    private const string EMPTY_NAMESPACE_MESSAGE = "Namespace name must not be empty.";

    /// <summary>Gets the selected types that the assertions apply to.</summary>
    public IEnumerable<Type> Subject => _types;

    /// <summary>Asserts that the selection contains exactly <paramref name="expected"/> types.</summary>
    /// <param name="expected">The expected selection count.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The selection count differs.</exception>
    public AndConstraint<TypeSelectorAssertions> HaveCount(int expected, string? because = null, params object?[] becauseArgs) {
        if(_types.Length != expected) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.SELECTOR_HAVE_COUNT, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_types.Length)), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected type is sealed.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected type is not sealed.</exception>
    public AndConstraint<TypeSelectorAssertions> BeSealed(string? because = null, params object?[] becauseArgs) {
        var offender = _types.FirstOrDefault(static type => !type.IsSealed);
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.TYPE_OFFENDER_BE_SEALED, offender.FullName), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected type is abstract.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected type is not abstract.</exception>
    public AndConstraint<TypeSelectorAssertions> BeAbstract(string? because = null, params object?[] becauseArgs) {
        var offender = _types.FirstOrDefault(static type => !type.IsAbstract);
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.TYPE_OFFENDER_BE_ABSTRACT, offender.FullName), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected type declares a public method named <paramref name="name"/>.</summary>
    /// <param name="name">The method name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">A selected type has no such method.</exception>
    public AndConstraint<TypeSelectorAssertions> HaveMethod(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        var offender = _types.FirstOrDefault(type => !type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .Any(m => string.Equals(m.Name, name, StringComparison.Ordinal)));
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.SELECTOR_HAVE_METHOD, offender.FullName, name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected type declares a public property named <paramref name="name"/>.</summary>
    /// <param name="name">The property name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">A selected type has no such property.</exception>
    public AndConstraint<TypeSelectorAssertions> HaveProperty(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        var offender = _types.FirstOrDefault(type => type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static) is null);
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.SELECTOR_HAVE_PROPERTY, offender.FullName, name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that no selected type is sealed.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected type is sealed.</exception>
    public AndConstraint<TypeSelectorAssertions> NotBeSealed(string? because = null, params object?[] becauseArgs) {
        var offender = _types.FirstOrDefault(static type => type.IsSealed);
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.TYPE_OFFENDER_NOT_BE_SEALED, offender.FullName), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected type is decorated with <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected type lacks the attribute.</exception>
    public AndConstraint<TypeSelectorAssertions> BeDecoratedWith<TAttribute>(string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        var offender = _types.FirstOrDefault(static type => type.GetCustomAttributes(typeof(TAttribute), inherit: false).Length == 0);
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.TYPE_OFFENDER_BE_DECORATED, offender.FullName, typeof(TAttribute).Name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that no selected type is decorated with <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type that must be absent.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected type carries the attribute.</exception>
    public AndConstraint<TypeSelectorAssertions> NotBeDecoratedWith<TAttribute>(string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        var offender = _types.FirstOrDefault(static type => type.GetCustomAttributes(typeof(TAttribute), inherit: false).Length != 0);
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.TYPE_OFFENDER_NOT_BE_DECORATED, offender.FullName, typeof(TAttribute).Name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected type is decorated with <typeparamref name="TAttribute"/> or inherits it from a base type.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected type neither carries nor inherits the attribute.</exception>
    public AndConstraint<TypeSelectorAssertions> BeDecoratedWithOrInherit<TAttribute>(string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        var offender = _types.FirstOrDefault(type => !IsDecoratedOrInherited(type, typeof(TAttribute)));
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.TYPE_OFFENDER_BE_DECORATED_OR_INHERIT, offender.FullName, typeof(TAttribute).Name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that no selected type is decorated with <typeparamref name="TAttribute"/> or inherits it.</summary>
    /// <typeparam name="TAttribute">The attribute type that must be absent.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected type carries or inherits the attribute.</exception>
    public AndConstraint<TypeSelectorAssertions> NotBeDecoratedWithOrInherit<TAttribute>(string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        var offender = _types.FirstOrDefault(type => IsDecoratedOrInherited(type, typeof(TAttribute)));
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.TYPE_OFFENDER_NOT_BE_DECORATED_OR_INHERIT, offender.FullName, typeof(TAttribute).Name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected type is in exactly <paramref name="namespaceName"/>.</summary>
    /// <param name="namespaceName">The namespace name (exact match).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="namespaceName"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="namespaceName"/> is empty or whitespace.</exception>
    /// <exception cref="AssertionFailedException">A selected type is in another namespace.</exception>
    public AndConstraint<TypeSelectorAssertions> BeInNamespace(string namespaceName, string? because = null, params object?[] becauseArgs) {
        if(namespaceName is null) {
            throw new ArgumentNullException(nameof(namespaceName));
        }

        if(namespaceName.Trim().Length == 0) {
            throw new ArgumentException(EMPTY_NAMESPACE_MESSAGE, nameof(namespaceName));
        }

        var offender = _types.FirstOrDefault(type => !string.Equals(type.Namespace, namespaceName, StringComparison.Ordinal));
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.SELECTOR_BE_IN_NAMESPACE, offender.FullName, namespaceName), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that no selected type is in exactly <paramref name="namespaceName"/>.</summary>
    /// <param name="namespaceName">The namespace name (exact match).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="namespaceName"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="namespaceName"/> is empty or whitespace.</exception>
    /// <exception cref="AssertionFailedException">A selected type is in that namespace.</exception>
    public AndConstraint<TypeSelectorAssertions> NotBeInNamespace(string namespaceName, string? because = null, params object?[] becauseArgs) {
        if(namespaceName is null) {
            throw new ArgumentNullException(nameof(namespaceName));
        }

        if(namespaceName.Trim().Length == 0) {
            throw new ArgumentException(EMPTY_NAMESPACE_MESSAGE, nameof(namespaceName));
        }

        var offender = _types.FirstOrDefault(type => string.Equals(type.Namespace, namespaceName, StringComparison.Ordinal));
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.SELECTOR_NOT_BE_IN_NAMESPACE, offender.FullName, namespaceName), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected type is under <paramref name="namespaceName"/> (the namespace itself or a child).</summary>
    /// <param name="namespaceName">The root namespace name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="namespaceName"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="namespaceName"/> is empty or whitespace.</exception>
    /// <exception cref="AssertionFailedException">A selected type is outside that namespace.</exception>
    public AndConstraint<TypeSelectorAssertions> BeUnderNamespace(string namespaceName, string? because = null, params object?[] becauseArgs) {
        if(namespaceName is null) {
            throw new ArgumentNullException(nameof(namespaceName));
        }

        if(namespaceName.Trim().Length == 0) {
            throw new ArgumentException(EMPTY_NAMESPACE_MESSAGE, nameof(namespaceName));
        }

        var offender = _types.FirstOrDefault(type => !IsUnderNamespace(type.Namespace, namespaceName));
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.SELECTOR_BE_UNDER_NAMESPACE, offender.FullName, namespaceName), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that no selected type is under <paramref name="namespaceName"/>.</summary>
    /// <param name="namespaceName">The root namespace name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="namespaceName"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="namespaceName"/> is empty or whitespace.</exception>
    /// <exception cref="AssertionFailedException">A selected type is under that namespace.</exception>
    public AndConstraint<TypeSelectorAssertions> NotBeUnderNamespace(string namespaceName, string? because = null, params object?[] becauseArgs) {
        if(namespaceName is null) {
            throw new ArgumentNullException(nameof(namespaceName));
        }

        if(namespaceName.Trim().Length == 0) {
            throw new ArgumentException(EMPTY_NAMESPACE_MESSAGE, nameof(namespaceName));
        }

        var offender = _types.FirstOrDefault(type => IsUnderNamespace(type.Namespace, namespaceName));
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.SELECTOR_NOT_BE_UNDER_NAMESPACE, offender.FullName, namespaceName), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Determines whether a type or any base type carries the attribute.</summary>
    /// <param name="type">The type to inspect.</param>
    /// <param name="attributeType">The attribute type to look for.</param>
    /// <returns><see langword="true"/> when the attribute is declared on the type or an ancestor.</returns>
    private static bool IsDecoratedOrInherited(Type type, Type attributeType) {
        for(var current = type; current is not null; current = current.BaseType) {
            if(current.GetCustomAttributes(attributeType, inherit: false).Length != 0) {
                return true;
            }
        }

        return false;
    }

    /// <summary>Determines whether a namespace equals or nests under a root namespace.</summary>
    /// <param name="actual">The namespace to test (may be <see langword="null"/>).</param>
    /// <param name="root">The root namespace.</param>
    /// <returns><see langword="true"/> when the namespace matches or starts with the root followed by a dot.</returns>
    private static bool IsUnderNamespace(string? actual, string root) =>
        string.Equals(actual, root, StringComparison.Ordinal)
            || (actual is not null && actual.StartsWith(root + ".", StringComparison.Ordinal));
}

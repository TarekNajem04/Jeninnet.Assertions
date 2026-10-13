//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

/// <summary>Provides assertion methods that must hold for every property in a <see cref="PropertyInfoSelector"/> selection.</summary>
/// <param name="properties">The selected properties.</param>
public class PropertyInfoSelectorAssertions(PropertyInfo[] properties) {
    private readonly PropertyInfo[] _properties = properties ?? throw new ArgumentNullException(nameof(properties));

    /// <summary>Gets the selected properties that the assertions apply to.</summary>
    public IEnumerable<PropertyInfo> SubjectProperties => _properties;

    private static string Display(Type? type) => type?.FullName ?? type?.Name ?? "<null>";

    /// <summary>Asserts that the selection contains exactly <paramref name="expected"/> properties.</summary>
    /// <param name="expected">The expected selection count.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The selection count differs.</exception>
    public AndConstraint<PropertyInfoSelectorAssertions> HaveCount(int expected, string? because = null, params object?[] becauseArgs) {
        if(_properties.Length != expected) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_HAVE_COUNT, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_properties.Length)), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected property is decorated with <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected property lacks the attribute.</exception>
    public AndConstraint<PropertyInfoSelectorAssertions> HaveAttribute<TAttribute>(string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        var offender = _properties.FirstOrDefault(static property => property.GetCustomAttributes(typeof(TAttribute), inherit: false).Length == 0);
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_BE_DECORATED, offender.Name, typeof(TAttribute).Name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that no selected property is decorated with <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type that must be absent.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected property carries the attribute.</exception>
    public AndConstraint<PropertyInfoSelectorAssertions> NotHaveAttribute<TAttribute>(string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        var offender = _properties.FirstOrDefault(static property => property.GetCustomAttributes(typeof(TAttribute), inherit: false).Length != 0);
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_NOT_BE_DECORATED, offender.Name, typeof(TAttribute).Name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected property is decorated with <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected property lacks the attribute.</exception>
    public AndConstraint<PropertyInfoSelectorAssertions> BeDecoratedWith<TAttribute>(string? because = null, params object?[] becauseArgs) where TAttribute : Attribute =>
        HaveAttribute<TAttribute>(because, becauseArgs);

    /// <summary>Asserts that no selected property is decorated with <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type that must be absent.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected property carries the attribute.</exception>
    public AndConstraint<PropertyInfoSelectorAssertions> NotBeDecoratedWith<TAttribute>(string? because = null, params object?[] becauseArgs) where TAttribute : Attribute =>
        NotHaveAttribute<TAttribute>(because, becauseArgs);

    /// <summary>Asserts that every selected property is decorated with <typeparamref name="TAttribute"/> or inherits it from a base property.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected property neither carries nor inherits the attribute.</exception>
    public AndConstraint<PropertyInfoSelectorAssertions> HaveAttributeOrInherit<TAttribute>(string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        var offender = _properties.FirstOrDefault(static property => property.GetCustomAttributes(typeof(TAttribute), inherit: true).Length == 0);
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_BE_DECORATED_OR_INHERIT, offender.Name, typeof(TAttribute).Name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected property is readable and has no setter.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected property is not read-only.</exception>
    public AndConstraint<PropertyInfoSelectorAssertions> BeReadOnly(string? because = null, params object?[] becauseArgs) {
        var offender = _properties.FirstOrDefault(static property => !property.CanRead || property.CanWrite);
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_BE_READ_ONLY, offender.Name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected property is writable and has no getter.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected property is not write-only.</exception>
    public AndConstraint<PropertyInfoSelectorAssertions> BeWriteOnly(string? because = null, params object?[] becauseArgs) {
        var offender = _properties.FirstOrDefault(static property => !property.CanWrite || property.CanRead);
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_BE_WRITE_ONLY, offender.Name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected property has a setter.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected property has no setter.</exception>
    public AndConstraint<PropertyInfoSelectorAssertions> BeWritable(string? because = null, params object?[] becauseArgs) {
        var offender = _properties.FirstOrDefault(static property => !property.CanWrite);
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_BE_WRITABLE, offender.Name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that no selected property has a setter.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected property has a setter.</exception>
    public AndConstraint<PropertyInfoSelectorAssertions> NotBeWritable(string? because = null, params object?[] becauseArgs) {
        var offender = _properties.FirstOrDefault(static property => property.CanWrite);
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_NOT_BE_WRITABLE, offender.Name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected property is virtual (its getter or setter is virtual).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected property is not virtual.</exception>
    public AndConstraint<PropertyInfoSelectorAssertions> BeVirtual(string? because = null, params object?[] becauseArgs) {
        var offender = _properties.FirstOrDefault(static property => !IsVirtual(property));
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_BE_VIRTUAL, offender.Name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that no selected property is virtual (its getter or setter is virtual).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected property is virtual.</exception>
    public AndConstraint<PropertyInfoSelectorAssertions> NotBeVirtual(string? because = null, params object?[] becauseArgs) {
        var offender = _properties.FirstOrDefault(static property => IsVirtual(property));
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_NOT_BE_VIRTUAL, offender.Name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected property has the given <paramref name="accessibility"/> (determined by the getter, then the setter).</summary>
    /// <param name="accessibility">The expected declared accessibility.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected property has another accessibility.</exception>
    public AndConstraint<PropertyInfoSelectorAssertions> HaveAccessModifier(AccessModifier accessibility, string? because = null, params object?[] becauseArgs) {
        var offender = _properties.FirstOrDefault(property => {
            var accessor = property.GetMethod ?? property.SetMethod;
            return accessor is null || MethodInfoAssertions.GetAccessModifier(accessor) != accessibility;
        });
        if(offender is not null) {
            var accessor = offender.GetMethod ?? offender.SetMethod;
            var actual = accessor is null ? "<null>" : AssertionScope.FormatInvariant(MethodInfoAssertions.GetAccessModifier(accessor));
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_HAVE_ACCESS_MODIFIER, offender.Name, AssertionScope.FormatInvariant(accessibility), actual), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected property is declared in <typeparamref name="TDeclaring"/>.</summary>
    /// <typeparam name="TDeclaring">The expected declaring type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected property is declared elsewhere.</exception>
    public AndConstraint<PropertyInfoSelectorAssertions> HaveDeclaringType<TDeclaring>(string? because = null, params object?[] becauseArgs) =>
        HaveDeclaringType(typeof(TDeclaring), because, becauseArgs);

    /// <summary>Asserts that every selected property is declared in <paramref name="declaringType"/>.</summary>
    /// <param name="declaringType">The expected declaring type.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">A selected property is declared elsewhere.</exception>
    public AndConstraint<PropertyInfoSelectorAssertions> HaveDeclaringType(Type declaringType, string? because = null, params object?[] becauseArgs) {
        if(declaringType is null) {
            throw new ArgumentNullException(nameof(declaringType));
        }

        var offender = _properties.FirstOrDefault(property => property.DeclaringType != declaringType);
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_HAVE_DECLARING_TYPE, offender.Name, Display(declaringType), Display(offender.DeclaringType)), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected property has a getter (public or non-public).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected property has no getter.</exception>
    public AndConstraint<PropertyInfoSelectorAssertions> HaveGetter(string? because = null, params object?[] becauseArgs) {
        var offender = _properties.FirstOrDefault(static property => property.GetGetMethod(nonPublic: true) is null);
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_HAVE_GETTER, offender.Name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected property has a setter (public or non-public).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected property has no setter.</exception>
    public AndConstraint<PropertyInfoSelectorAssertions> HaveSetter(string? because = null, params object?[] becauseArgs) {
        var offender = _properties.FirstOrDefault(static property => property.GetSetMethod(nonPublic: true) is null);
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_HAVE_SETTER, offender.Name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Determines whether either property accessor is virtual.</summary>
    /// <param name="property">The property to inspect.</param>
    /// <returns><see langword="true"/> when the getter or setter is virtual.</returns>
    private static bool IsVirtual(PropertyInfo property) =>
        property.GetMethod?.IsVirtual == true || property.SetMethod?.IsVirtual == true;
}

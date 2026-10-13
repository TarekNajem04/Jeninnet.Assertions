//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

/// <summary>Provides assertion methods for verifying <see cref="PropertyInfo"/> metadata.</summary>
/// <param name="value">The property to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class PropertyInfoAssertions(PropertyInfo? value, string? expression = null) : AssertionsBase(expression, "property") {
    private readonly PropertyInfo? _value = value;
    private const string FOUND_NULL = "found null";
    private const string WAS_NOT = "it was not";

    /// <summary>Asserts that the property is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The property is not <see langword="null"/>.</exception>
    public AndConstraint<PropertyInfoAssertions> BeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is not null) {
            Fail(Strings.Format(StringKeys.TYPE_BE_NULL, Expression, _value.Name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property is not <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The property is <see langword="null"/>.</exception>
    public AndConstraint<PropertyInfoAssertions> NotBeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property returns <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The expected return type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The property is <see langword="null"/> or has another type.</exception>
    public AndConstraint<PropertyInfoAssertions> Return<T>(string? because = null, params object?[] becauseArgs) => Return(typeof(T), because, becauseArgs);

    /// <summary>Asserts that the property returns <paramref name="returnType"/>.</summary>
    /// <param name="returnType">The expected return type.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="returnType"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The property is <see langword="null"/> or has another type.</exception>
    public AndConstraint<PropertyInfoAssertions> Return(Type returnType, string? because = null, params object?[] becauseArgs) {
        if(returnType is null) {
            throw new ArgumentNullException(nameof(returnType));
        }

        if(_value is null || _value.PropertyType != returnType) {
            Fail(Strings.Format(StringKeys.PROPERTY_HAVE_RETURN_TYPE, Expression, returnType.Name, _value?.PropertyType.Name ?? "<null>"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property does not return <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The return type the subject must not have.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The property returns <typeparamref name="T"/>.</exception>
    public AndConstraint<PropertyInfoAssertions> NotReturn<T>(string? because = null, params object?[] becauseArgs) => NotReturn(typeof(T), because, becauseArgs);

    /// <summary>Asserts that the property does not return <paramref name="returnType"/>.</summary>
    /// <param name="returnType">The return type the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="returnType"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The property returns <paramref name="returnType"/>.</exception>
    public AndConstraint<PropertyInfoAssertions> NotReturn(Type returnType, string? because = null, params object?[] becauseArgs) {
        if(returnType is null) {
            throw new ArgumentNullException(nameof(returnType));
        }

        if(_value is not null && _value.PropertyType == returnType) {
            Fail(Strings.Format(StringKeys.PROPERTY_NOT_HAVE_RETURN_TYPE, Expression, returnType.Name, _value.PropertyType.Name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property is readable.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The property is <see langword="null"/> or write-only.</exception>
    public AndConstraint<PropertyInfoAssertions> BeReadable(string? because = null, params object?[] becauseArgs) {
        if(_value?.CanRead != true) {
            Fail(Strings.Format(StringKeys.PROPERTY_BE_READABLE, Expression, _value is null ? FOUND_NULL : WAS_NOT), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property is writable.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The property is <see langword="null"/> or read-only.</exception>
    public AndConstraint<PropertyInfoAssertions> BeWritable(string? because = null, params object?[] becauseArgs) {
        if(_value?.CanWrite != true) {
            Fail(Strings.Format(StringKeys.PROPERTY_BE_WRITABLE, Expression, _value is null ? FOUND_NULL : WAS_NOT), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property is not readable.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The property is readable.</exception>
    public AndConstraint<PropertyInfoAssertions> NotBeReadable(string? because = null, params object?[] becauseArgs) {
        if(_value?.CanRead == true) {
            Fail(Strings.Format(StringKeys.PROPERTY_NOT_BE_READABLE, Expression, "it was"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property is not writable.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The property is writable.</exception>
    public AndConstraint<PropertyInfoAssertions> NotBeWritable(string? because = null, params object?[] becauseArgs) {
        if(_value?.CanWrite == true) {
            Fail(Strings.Format(StringKeys.PROPERTY_NOT_BE_WRITABLE, Expression, "it was"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property is virtual (its getter or setter is virtual).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The property is <see langword="null"/> or not virtual.</exception>
    public AndConstraint<PropertyInfoAssertions> BeVirtual(string? because = null, params object?[] becauseArgs) {
        if(!IsVirtual(_value)) {
            Fail(Strings.Format(StringKeys.PROPERTY_BE_VIRTUAL, Expression, _value is null ? FOUND_NULL : WAS_NOT), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property is not virtual.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The property is virtual.</exception>
    public AndConstraint<PropertyInfoAssertions> NotBeVirtual(string? because = null, params object?[] becauseArgs) {
        if(IsVirtual(_value)) {
            Fail(Strings.Format(StringKeys.PROPERTY_NOT_BE_VIRTUAL, Expression, "it was"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property is readable with the given <paramref name="accessibility"/> (determined by the getter).</summary>
    /// <param name="accessibility">The expected getter accessibility.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The property is <see langword="null"/>, not readable, or has another accessibility.</exception>
    public AndConstraint<PropertyInfoAssertions> BeReadable(AccessModifier accessibility, string? because = null, params object?[] becauseArgs) {
        var reader = _value?.GetMethod;
        if(reader is null) {
            Fail(Strings.Format(StringKeys.PROPERTY_BE_READABLE, Expression, _value is null ? FOUND_NULL : WAS_NOT), because, becauseArgs);
        } else if(MethodInfoAssertions.GetAccessModifier(reader) != accessibility) {
            Fail(Strings.Format(StringKeys.PROPERTY_BE_READABLE_ACCESS, Expression, AssertionScope.FormatInvariant(accessibility), AssertionScope.FormatInvariant(MethodInfoAssertions.GetAccessModifier(reader))), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property is writable with the given <paramref name="accessibility"/> (determined by the setter).</summary>
    /// <param name="accessibility">The expected setter accessibility.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The property is <see langword="null"/>, not writable, or has another accessibility.</exception>
    public AndConstraint<PropertyInfoAssertions> BeWritable(AccessModifier accessibility, string? because = null, params object?[] becauseArgs) {
        var writer = _value?.SetMethod;
        if(writer is null) {
            Fail(Strings.Format(StringKeys.PROPERTY_BE_WRITABLE, Expression, _value is null ? FOUND_NULL : WAS_NOT), because, becauseArgs);
        } else if(MethodInfoAssertions.GetAccessModifier(writer) != accessibility) {
            Fail(Strings.Format(StringKeys.PROPERTY_BE_WRITABLE_ACCESS, Expression, AssertionScope.FormatInvariant(accessibility), AssertionScope.FormatInvariant(MethodInfoAssertions.GetAccessModifier(writer))), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Determines whether either property accessor is virtual.</summary>
    /// <param name="property">The property to inspect (may be <see langword="null"/>).</param>
    /// <returns><see langword="true"/> when the getter or setter is virtual.</returns>
    private static bool IsVirtual(PropertyInfo? property) =>
        property?.GetMethod?.IsVirtual == true || property?.SetMethod?.IsVirtual == true;

    /// <summary>Asserts that the property is decorated with <typeparamref name="TAttribute"/> (including inherited attributes).</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The property is <see langword="null"/> or lacks the attribute.</exception>
    public AndConstraint<PropertyInfoAssertions> BeDecoratedWith<TAttribute>(string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        if(_value is null || _value.GetCustomAttributes(typeof(TAttribute), inherit: true).Length == 0) {
            Fail(Strings.Format(StringKeys.METHOD_BE_DECORATED, Expression, typeof(TAttribute).Name, _value is null ? FOUND_NULL : WAS_NOT), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the property is declared with the given <paramref name="accessibility"/> (determined by the getter when present, otherwise the setter).</summary>
    /// <param name="accessibility">The expected declared accessibility.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The property is <see langword="null"/>, accessor-less, or has another accessibility.</exception>
    public AndConstraint<PropertyInfoAssertions> HaveAccessModifier(AccessModifier accessibility, string? because = null, params object?[] becauseArgs) {
        var accessor = _value?.GetMethod ?? _value?.SetMethod;
        if(accessor is null || MethodInfoAssertions.GetAccessModifier(accessor) != accessibility) {
            Fail(Strings.Format(StringKeys.PROPERTY_HAVE_ACCESS_MODIFIER, Expression, AssertionScope.FormatInvariant(accessibility), accessor is null ? "<null>" : AssertionScope.FormatInvariant(MethodInfoAssertions.GetAccessModifier(accessor))), because, becauseArgs);
        }

        return new(this);
    }
}

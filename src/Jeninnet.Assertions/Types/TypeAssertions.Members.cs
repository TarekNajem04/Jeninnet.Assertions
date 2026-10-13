//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

public partial class TypeAssertions {
    /// <summary>Asserts that the type declares a public method named <paramref name="name"/> and exposes the first match for further assertions.</summary>
    /// <param name="name">The method name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the first matching method.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or has no such method.</exception>
    public WhichConstraint<MethodInfo> HaveMethod(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        var method = (_value?.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .FirstOrDefault(m => string.Equals(m.Name, name, StringComparison.Ordinal))) ?? throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.TYPE_HAVE_METHOD, Expression, name), because, becauseArgs));
        return new WhichConstraint<MethodInfo>(method);
    }

    /// <summary>Asserts that the type declares a public method named <paramref name="name"/> with the given parameter types and exposes it for further assertions.</summary>
    /// <param name="name">The method name.</param>
    /// <param name="parameterTypes">The exact parameter types.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the matching method.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="parameterTypes"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or has no such method.</exception>
    public WhichConstraint<MethodInfo> HaveMethod(string name, Type[] parameterTypes, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(parameterTypes is null) {
            throw new ArgumentNullException(nameof(parameterTypes));
        }

        var method = (_value?.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .FirstOrDefault(m => string.Equals(m.Name, name, StringComparison.Ordinal) && ParametersMatch(m, parameterTypes))) ?? throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.TYPE_HAVE_METHOD, Expression, FormatSignature(name, parameterTypes)), because, becauseArgs));
        return new WhichConstraint<MethodInfo>(method);
    }

    /// <summary>Asserts that the type has no public method named <paramref name="name"/>. A <see langword="null"/> subject passes.</summary>
    /// <param name="name">The method name that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The type has such a method.</exception>
    public AndConstraint<TypeAssertions> NotHaveMethod(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(_value?.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                .Any(m => string.Equals(m.Name, name, StringComparison.Ordinal)) == true) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_HAVE_METHOD, Expression, name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type declares a public property named <paramref name="name"/> and exposes it for further assertions.</summary>
    /// <param name="name">The property name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the property.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or has no such property.</exception>
    public WhichConstraint<PropertyInfo> HaveProperty(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        var property = (_value?.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) ?? throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.TYPE_HAVE_PROPERTY, Expression, name), because, becauseArgs));
        return new WhichConstraint<PropertyInfo>(property);
    }

    /// <summary>Asserts that the type has no public property named <paramref name="name"/>. A <see langword="null"/> subject passes.</summary>
    /// <param name="name">The property name that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The type has such a property.</exception>
    public AndConstraint<TypeAssertions> NotHaveProperty(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(_value?.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static) is not null) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_HAVE_PROPERTY, Expression, name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type declares a public constructor with the given parameter types and exposes it for further assertions.</summary>
    /// <param name="parameterTypes">The exact parameter types (empty for the default constructor).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the constructor.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="parameterTypes"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or has no such constructor.</exception>
    public WhichConstraint<ConstructorInfo> HaveConstructor(Type[] parameterTypes, string? because = null, params object?[] becauseArgs) {
        if(parameterTypes is null) {
            throw new ArgumentNullException(nameof(parameterTypes));
        }

        var constructor = _value?.GetConstructor(BindingFlags.Public | BindingFlags.Instance, binder: null, parameterTypes, modifiers: null);
        if(constructor is null) {
            var signature = parameterTypes.Length == 0 ? "()" : $"({string.Join(", ", parameterTypes.Select(static t => t.Name))})";
            throw new AssertionFailedException(AssertionScope.AppendBecause(Strings.Format(StringKeys.TYPE_HAVE_CONSTRUCTOR, Expression, signature), because, becauseArgs));
        }

        return new WhichConstraint<ConstructorInfo>(constructor);
    }

    /// <summary>Asserts that the type declares a public constructor with the given parameter types and exposes it for further assertions.</summary>
    /// <param name="parameterTypes">The exact parameter types (empty for the default constructor).</param>
    /// <returns>A <see cref="WhichConstraint{T}"/> wrapping the constructor.</returns>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or has no such constructor.</exception>
    public WhichConstraint<ConstructorInfo> HaveConstructor(params Type[] parameterTypes) => HaveConstructor(parameterTypes, null, []);

    /// <summary>Determines whether a method's parameter list exactly matches the given types.</summary>
    /// <param name="method">The method to inspect.</param>
    /// <param name="parameterTypes">The expected parameter types.</param>
    /// <returns><see langword="true"/> when counts and types match positionally.</returns>
    private static bool ParametersMatch(MethodInfo method, Type[] parameterTypes) {
        var actual = method.GetParameters();
        if(actual.Length != parameterTypes.Length) {
            return false;
        }

        for(var index = 0; index < actual.Length; index++) {
            if(actual[index].ParameterType != parameterTypes[index]) {
                return false;
            }
        }

        return true;
    }

    /// <summary>Formats a method signature for failure messages.</summary>
    /// <param name="name">The method name.</param>
    /// <param name="parameterTypes">The parameter types.</param>
    /// <returns>The name with comma-separated simple type names.</returns>
    private static string FormatSignature(string name, Type[] parameterTypes) =>
        $"{name}({string.Join(", ", parameterTypes.Select(static t => t.Name))})";

    /// <summary>Asserts that the type declares a public parameterless constructor.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or lacks a public parameterless constructor.</exception>
    public AndConstraint<TypeAssertions> HaveDefaultConstructor(string? because = null, params object?[] becauseArgs) {
        if(_value?.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null) is null) {
            Fail(Strings.Format(StringKeys.TYPE_HAVE_DEFAULT_CONSTRUCTOR, Expression, FoundSuffix()), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type declares no public parameterless constructor.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type declares a public parameterless constructor.</exception>
    public AndConstraint<TypeAssertions> NotHaveDefaultConstructor(string? because = null, params object?[] becauseArgs) {
        if(_value?.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null) is not null) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_HAVE_DEFAULT_CONSTRUCTOR, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type declares no public instance constructor with the given parameter types.</summary>
    /// <param name="parameterTypes">The exact constructor parameter types that must be absent.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="parameterTypes"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">Such a constructor exists.</exception>
    public AndConstraint<TypeAssertions> NotHaveConstructor(params Type[] parameterTypes) {
        if(parameterTypes is null) {
            throw new ArgumentNullException(nameof(parameterTypes));
        }

        if(_value?.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, parameterTypes, null) is not null) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_HAVE_CONSTRUCTOR, Expression, FormatSignature("ctor", parameterTypes)));
        }

        return new(this);
    }

    /// <summary>Asserts that the type declares an indexer of type <paramref name="indexerType"/> with the given index parameter types.</summary>
    /// <param name="indexerType">The expected indexer (element) type.</param>
    /// <param name="indexParameterTypes">The exact index parameter types.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="indexerType"/> or <paramref name="indexParameterTypes"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">No matching indexer exists.</exception>
    public AndConstraint<TypeAssertions> HaveIndexer(Type indexerType, Type[] indexParameterTypes, string? because = null, params object?[] becauseArgs) {
        if(indexerType is null) {
            throw new ArgumentNullException(nameof(indexerType));
        }

        if(indexParameterTypes is null) {
            throw new ArgumentNullException(nameof(indexParameterTypes));
        }

        var found = _value?.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .Any(property => property.GetIndexParameters().Length == indexParameterTypes.Length
                && property.PropertyType == indexerType
                && property.GetIndexParameters().Select(static parameter => parameter.ParameterType).SequenceEqual(indexParameterTypes));
        if(found is not true) {
            Fail(Strings.Format(StringKeys.TYPE_HAVE_INDEXER, Expression, indexerType.Name, string.Join(", ", indexParameterTypes.Select(static t => t.Name))), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type declares no indexer with the given index parameter types.</summary>
    /// <param name="indexParameterTypes">The index parameter types that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="indexParameterTypes"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">Such an indexer exists.</exception>
    public AndConstraint<TypeAssertions> NotHaveIndexer(Type[] indexParameterTypes, string? because = null, params object?[] becauseArgs) {
        if(indexParameterTypes is null) {
            throw new ArgumentNullException(nameof(indexParameterTypes));
        }

        var found = _value is not null && _value.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .Any(property => property.GetIndexParameters().Length == indexParameterTypes.Length
                && property.GetIndexParameters().Select(static parameter => parameter.ParameterType).SequenceEqual(indexParameterTypes));
        if(found) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_HAVE_INDEXER, Expression, string.Join(", ", indexParameterTypes.Select(static t => t.Name))), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type declares an implicit conversion operator involving <paramref name="targetType"/>.</summary>
    /// <param name="targetType">The converted-from or converted-to type.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="targetType"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">No such operator exists.</exception>
    public AndConstraint<TypeAssertions> HaveImplicitConversionOperator(Type targetType, string? because = null, params object?[] becauseArgs) {
        if(targetType is null) {
            throw new ArgumentNullException(nameof(targetType));
        }

        if(!HasConversionOperator("op_Implicit", targetType)) {
            Fail(Strings.Format(StringKeys.TYPE_HAVE_IMPLICIT_CONVERSION, Expression, Format(targetType)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type declares no implicit conversion operator involving <paramref name="targetType"/>.</summary>
    /// <param name="targetType">The type that must not participate in an implicit conversion.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="targetType"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">Such an operator exists.</exception>
    public AndConstraint<TypeAssertions> NotHaveImplicitConversionOperator(Type targetType, string? because = null, params object?[] becauseArgs) {
        if(targetType is null) {
            throw new ArgumentNullException(nameof(targetType));
        }

        if(HasConversionOperator("op_Implicit", targetType)) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_HAVE_IMPLICIT_CONVERSION, Expression, Format(targetType)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type declares an explicit conversion operator involving <paramref name="targetType"/>.</summary>
    /// <param name="targetType">The converted-from or converted-to type.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="targetType"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">No such operator exists.</exception>
    public AndConstraint<TypeAssertions> HaveExplicitConversionOperator(Type targetType, string? because = null, params object?[] becauseArgs) {
        if(targetType is null) {
            throw new ArgumentNullException(nameof(targetType));
        }

        if(!HasConversionOperator("op_Explicit", targetType)) {
            Fail(Strings.Format(StringKeys.TYPE_HAVE_EXPLICIT_CONVERSION, Expression, Format(targetType)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type declares no explicit conversion operator involving <paramref name="targetType"/>.</summary>
    /// <param name="targetType">The type that must not participate in an explicit conversion.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="targetType"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">Such an operator exists.</exception>
    public AndConstraint<TypeAssertions> NotHaveExplicitConversionOperator(Type targetType, string? because = null, params object?[] becauseArgs) {
        if(targetType is null) {
            throw new ArgumentNullException(nameof(targetType));
        }

        if(HasConversionOperator("op_Explicit", targetType)) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_HAVE_EXPLICIT_CONVERSION, Expression, Format(targetType)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type declares an explicitly-implemented method named <paramref name="name"/> (dotted or simple name).</summary>
    /// <param name="name">The method name, optionally qualified (e.g. <c>IWorker.Work</c>).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">No such explicit method exists.</exception>
    public AndConstraint<TypeAssertions> HaveExplicitMethod(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        var found = _value is not null && _value.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .Any(method => MatchesExplicitName(method.Name, name));
        if(!found) {
            Fail(Strings.Format(StringKeys.TYPE_HAVE_EXPLICIT_METHOD, Expression, name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type declares no explicitly-implemented method named <paramref name="name"/>.</summary>
    /// <param name="name">The method name, optionally qualified.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">Such an explicit method exists.</exception>
    public AndConstraint<TypeAssertions> NotHaveExplicitMethod(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        var found = _value is not null && _value.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .Any(method => MatchesExplicitName(method.Name, name));
        if(found) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_HAVE_EXPLICIT_METHOD, Expression, name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type declares an explicitly-implemented property named <paramref name="name"/> (dotted or simple name).</summary>
    /// <param name="name">The property name, optionally qualified (e.g. <c>IWorker.Load</c>).</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">No such explicit property exists.</exception>
    public AndConstraint<TypeAssertions> HaveExplicitProperty(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        var found = _value is not null && _value.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .Any(property => MatchesExplicitName(property.Name, name));
        if(!found) {
            Fail(Strings.Format(StringKeys.TYPE_HAVE_EXPLICIT_PROPERTY, Expression, name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type declares no explicitly-implemented property named <paramref name="name"/>.</summary>
    /// <param name="name">The property name, optionally qualified.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">Such an explicit property exists.</exception>
    public AndConstraint<TypeAssertions> NotHaveExplicitProperty(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        var found = _value is not null && _value.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .Any(property => MatchesExplicitName(property.Name, name));
        if(found) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_HAVE_EXPLICIT_PROPERTY, Expression, name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Determines whether the type declares a public static conversion operator toward or from a target type.</summary>
    /// <param name="operatorName">The operator method name (e.g. <c>op_Implicit</c>).</param>
    /// <param name="targetType">The conversion target type.</param>
    /// <returns><see langword="true"/> when a matching conversion operator exists.</returns>
    private bool HasConversionOperator(string operatorName, Type targetType) {
        if(_value is null) {
            return false;
        }

        return _value.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Any(method => string.Equals(method.Name, operatorName, StringComparison.Ordinal)
                && (method.ReturnType == targetType || method.GetParameters().FirstOrDefault()?.ParameterType == targetType));
    }

    /// <summary>Matches an explicit interface implementation name against a simple member name.</summary>
    /// <param name="actualName">The reflected member name (e.g. <c>IFoo.Bar</c>).</param>
    /// <param name="expectedName">The simple member name to match.</param>
    /// <returns><see langword="true"/> when the names are equal or the actual name ends with the simple name.</returns>
    private static bool MatchesExplicitName(string actualName, string expectedName) =>
        actualName.IndexOf('.') >= 0
            && (string.Equals(actualName, expectedName, StringComparison.Ordinal)
                || actualName.EndsWith("." + expectedName, StringComparison.Ordinal));
}

//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

/// <summary>Provides assertion methods for verifying <see cref="MethodInfo"/> metadata.</summary>
/// <param name="value">The method to assert on.</param>
/// <param name="expression">The caller expression naming the subject under test (captured by <c>Should()</c>).</param>
public class MethodInfoAssertions(MethodInfo? value, string? expression = null) : AssertionsBase(expression, "method") {
    private readonly MethodInfo? _value = value;
    private const string FOUND_NULL = "found null";
    private const string WAS_NOT = "it was not";

    /// <summary>Asserts that the method is <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The method is not <see langword="null"/>.</exception>
    public AndConstraint<MethodInfoAssertions> BeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is not null) {
            Fail(Strings.Format(StringKeys.TYPE_BE_NULL, Expression, _value.Name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method is not <see langword="null"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The method is <see langword="null"/>.</exception>
    public AndConstraint<MethodInfoAssertions> NotBeNull(string? because = null, params object?[] becauseArgs) {
        if(_value is null) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_BE_NULL, Expression), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method returns <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The expected return type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The method is <see langword="null"/> or returns another type.</exception>
    public AndConstraint<MethodInfoAssertions> Return<T>(string? because = null, params object?[] becauseArgs) => Return(typeof(T), because, becauseArgs);

    /// <summary>Asserts that the method returns <paramref name="returnType"/>.</summary>
    /// <param name="returnType">The expected return type.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="returnType"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The method is <see langword="null"/> or returns another type.</exception>
    public AndConstraint<MethodInfoAssertions> Return(Type returnType, string? because = null, params object?[] becauseArgs) {
        if(returnType is null) {
            throw new ArgumentNullException(nameof(returnType));
        }

        if(_value is null || _value.ReturnType != returnType) {
            Fail(Strings.Format(StringKeys.METHOD_HAVE_RETURN_TYPE, Expression, returnType.Name, _value?.ReturnType.Name ?? "<null>"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method does not return <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The return type the subject must not have.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The method returns <typeparamref name="T"/>.</exception>
    public AndConstraint<MethodInfoAssertions> NotReturn<T>(string? because = null, params object?[] becauseArgs) => NotReturn(typeof(T), because, becauseArgs);

    /// <summary>Asserts that the method does not return <paramref name="returnType"/>.</summary>
    /// <param name="returnType">The return type the subject must not have.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="returnType"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The method returns <paramref name="returnType"/>.</exception>
    public AndConstraint<MethodInfoAssertions> NotReturn(Type returnType, string? because = null, params object?[] becauseArgs) {
        if(returnType is null) {
            throw new ArgumentNullException(nameof(returnType));
        }

        if(_value is not null && _value.ReturnType == returnType) {
            Fail(Strings.Format(StringKeys.METHOD_NOT_HAVE_RETURN_TYPE, Expression, returnType.Name, _value.ReturnType.Name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method returns void.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The method is <see langword="null"/> or returns a value.</exception>
    public AndConstraint<MethodInfoAssertions> ReturnVoid(string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value.ReturnType != typeof(void)) {
            Fail(Strings.Format(StringKeys.METHOD_RETURN_VOID, Expression, _value?.ReturnType.Name ?? "<null>"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method does not return void.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The method returns void.</exception>
    public AndConstraint<MethodInfoAssertions> NotReturnVoid(string? because = null, params object?[] becauseArgs) {
        if(_value is not null && _value.ReturnType == typeof(void)) {
            Fail(Strings.Format(StringKeys.METHOD_NOT_RETURN_VOID, Expression, _value.ReturnType.Name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method is not virtual.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The method is virtual.</exception>
    public AndConstraint<MethodInfoAssertions> NotBeVirtual(string? because = null, params object?[] becauseArgs) {
        if(_value?.IsVirtual == true) {
            Fail(Strings.Format(StringKeys.METHOD_NOT_BE_VIRTUAL, Expression, "it was"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method is asynchronous (carries an async state machine).</summary>
    /// <remarks>Detection relies on <see cref="AsyncStateMachineAttribute"/>; hand-written state machines
    /// and proxies may not be recognized.</remarks>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The method is <see langword="null"/> or not asynchronous.</exception>
    public AndConstraint<MethodInfoAssertions> BeAsync(string? because = null, params object?[] becauseArgs) {
        if(_value is null || !Attribute.IsDefined(_value, typeof(AsyncStateMachineAttribute), inherit: false)) {
            Fail(Strings.Format(StringKeys.METHOD_BE_ASYNC, Expression, _value is null ? FOUND_NULL : WAS_NOT), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method is not asynchronous.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The method carries an async state machine.</exception>
    public AndConstraint<MethodInfoAssertions> NotBeAsync(string? because = null, params object?[] becauseArgs) {
        if(_value is not null && Attribute.IsDefined(_value, typeof(AsyncStateMachineAttribute), inherit: false)) {
            Fail(Strings.Format(StringKeys.METHOD_NOT_BE_ASYNC, Expression, "it was"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method declares a parameter named <paramref name="name"/> of type <paramref name="type"/>.</summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="type">The parameter type.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The method is <see langword="null"/> or has no such parameter.</exception>
    public AndConstraint<MethodInfoAssertions> HaveParameter(string name, Type type, string? because = null, params object?[] becauseArgs) {
        if(!HasParameter(_value, name, type)) {
            Fail(Strings.Format(StringKeys.METHOD_HAVE_PARAMETER, Expression, name, type.Name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method is virtual.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The method is <see langword="null"/> or not virtual.</exception>
    public AndConstraint<MethodInfoAssertions> BeVirtual(string? because = null, params object?[] becauseArgs) {
        if(_value?.IsVirtual != true) {
            Fail(Strings.Format(StringKeys.METHOD_BE_VIRTUAL, Expression, _value is null ? FOUND_NULL : WAS_NOT), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method is static.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The method is <see langword="null"/> or not static.</exception>
    public AndConstraint<MethodInfoAssertions> BeStatic(string? because = null, params object?[] becauseArgs) {
        if(_value?.IsStatic != true) {
            Fail(Strings.Format(StringKeys.METHOD_BE_STATIC, Expression, _value is null ? FOUND_NULL : WAS_NOT), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method is not static.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The method is static.</exception>
    public AndConstraint<MethodInfoAssertions> NotBeStatic(string? because = null, params object?[] becauseArgs) {
        if(_value?.IsStatic == true) {
            Fail(Strings.Format(StringKeys.METHOD_NOT_BE_STATIC, Expression, "it was"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method is decorated with <typeparamref name="TAttribute"/> (including inherited attributes).</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The method is <see langword="null"/> or lacks the attribute.</exception>
    public AndConstraint<MethodInfoAssertions> BeDecoratedWith<TAttribute>(string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        if(_value is null || _value.GetCustomAttributes(typeof(TAttribute), inherit: true).Length == 0) {
            Fail(Strings.Format(StringKeys.METHOD_BE_DECORATED, Expression, typeof(TAttribute).Name, _value is null ? FOUND_NULL : WAS_NOT), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the method is declared with the given <paramref name="accessibility"/>.</summary>
    /// <param name="accessibility">The expected declared accessibility.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The method is <see langword="null"/> or has another accessibility.</exception>
    public AndConstraint<MethodInfoAssertions> HaveAccessModifier(AccessModifier accessibility, string? because = null, params object?[] becauseArgs) {
        if(_value is null || GetAccessModifier(_value) != accessibility) {
            Fail(Strings.Format(StringKeys.METHOD_HAVE_ACCESS_MODIFIER, Expression, AssertionScope.FormatInvariant(accessibility), _value is null ? "<null>" : AssertionScope.FormatInvariant(GetAccessModifier(_value))), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Gets the declared accessibility of a method (shared with property accessors).</summary>
    /// <param name="method">The method to inspect.</param>
    /// <returns>The declared <see cref="AccessModifier"/>.</returns>
    internal static AccessModifier GetAccessModifier(MethodInfo method) {
        if(method.IsPublic) {
            return AccessModifier.Public;
        }

        if(method.IsPrivate) {
            return AccessModifier.Private;
        }

        if(method.IsFamilyOrAssembly) {
            return AccessModifier.ProtectedInternal;
        }

        if(method.IsFamilyAndAssembly) {
            return AccessModifier.PrivateProtected;
        }

        return method.IsFamily ? AccessModifier.Protected : AccessModifier.Internal;
    }

    /// <summary>Determines whether a method or constructor declares a parameter with the given name and type (shared with constructor assertions).</summary>
    /// <param name="member">The method or constructor to inspect (may be <see langword="null"/>).</param>
    /// <param name="name">The parameter name.</param>
    /// <param name="type">The parameter type.</param>
    /// <returns><see langword="true"/> when a parameter matches both the name and the type.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="type"/> is <see langword="null"/>.</exception>
    internal static bool HasParameter(MethodBase? member, string name, Type type) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(type is null) {
            throw new ArgumentNullException(nameof(type));
        }

        return member?.GetParameters().Any(parameter => string.Equals(parameter.Name, name, StringComparison.Ordinal) && parameter.ParameterType == type) == true;
    }
}

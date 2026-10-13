//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

/// <summary>Provides assertion methods that must hold for every method in a <see cref="MethodInfoSelector"/> selection.</summary>
/// <param name="methods">The selected methods.</param>
public class MethodInfoSelectorAssertions(MethodInfo[] methods) {
    private readonly MethodInfo[] _methods = methods ?? throw new ArgumentNullException(nameof(methods));

    /// <summary>Gets the selected methods that the assertions apply to.</summary>
    public IEnumerable<MethodInfo> SubjectMethods => _methods;

    private static string Display(Type? type) => type?.FullName ?? type?.Name ?? "<null>";

    /// <summary>Asserts that the selection contains exactly <paramref name="expected"/> methods.</summary>
    /// <param name="expected">The expected selection count.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The selection count differs.</exception>
    public AndConstraint<MethodInfoSelectorAssertions> HaveCount(int expected, string? because = null, params object?[] becauseArgs) {
        if(_methods.Length != expected) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_HAVE_COUNT, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(_methods.Length)), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected method is decorated with <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected method lacks the attribute.</exception>
    public AndConstraint<MethodInfoSelectorAssertions> HaveAttribute<TAttribute>(string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        var offender = _methods.FirstOrDefault(static method => method.GetCustomAttributes(typeof(TAttribute), inherit: false).Length == 0);
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_BE_DECORATED, offender.Name, typeof(TAttribute).Name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that no selected method is decorated with <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type that must be absent.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected method carries the attribute.</exception>
    public AndConstraint<MethodInfoSelectorAssertions> NotHaveAttribute<TAttribute>(string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        var offender = _methods.FirstOrDefault(static method => method.GetCustomAttributes(typeof(TAttribute), inherit: false).Length != 0);
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_NOT_BE_DECORATED, offender.Name, typeof(TAttribute).Name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected method is decorated with <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected method lacks the attribute.</exception>
    public AndConstraint<MethodInfoSelectorAssertions> BeDecoratedWith<TAttribute>(string? because = null, params object?[] becauseArgs) where TAttribute : Attribute =>
        HaveAttribute<TAttribute>(because, becauseArgs);

    /// <summary>Asserts that every selected method is decorated with an attribute of type <typeparamref name="TAttribute"/> matching <paramref name="predicate"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="predicate">The condition the attribute must satisfy.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">A selected method lacks a matching attribute.</exception>
    public AndConstraint<MethodInfoSelectorAssertions> BeDecoratedWith<TAttribute>(Expression<Func<TAttribute, bool>> predicate, string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        var compiled = predicate.Compile();
        var offender = _methods.FirstOrDefault(method => !method.GetCustomAttributes(typeof(TAttribute), inherit: false).OfType<TAttribute>().Any(compiled));
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_BE_DECORATED_PREDICATE, offender.Name, typeof(TAttribute).Name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that no selected method is decorated with <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type that must be absent.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected method carries the attribute.</exception>
    public AndConstraint<MethodInfoSelectorAssertions> NotBeDecoratedWith<TAttribute>(string? because = null, params object?[] becauseArgs) where TAttribute : Attribute =>
        NotHaveAttribute<TAttribute>(because, becauseArgs);

    /// <summary>Asserts that no selected method is decorated with an attribute of type <typeparamref name="TAttribute"/> matching <paramref name="predicate"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type that must be absent.</typeparam>
    /// <param name="predicate">The condition the attribute must not satisfy.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">A selected method carries a matching attribute.</exception>
    public AndConstraint<MethodInfoSelectorAssertions> NotBeDecoratedWith<TAttribute>(Expression<Func<TAttribute, bool>> predicate, string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        var compiled = predicate.Compile();
        var offender = _methods.FirstOrDefault(method => method.GetCustomAttributes(typeof(TAttribute), inherit: false).OfType<TAttribute>().Any(compiled));
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_NOT_BE_DECORATED_PREDICATE, offender.Name, typeof(TAttribute).Name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected method is decorated with <typeparamref name="TAttribute"/> or inherits it from a base method.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected method neither carries nor inherits the attribute.</exception>
    public AndConstraint<MethodInfoSelectorAssertions> HaveAttributeOrInherit<TAttribute>(string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        var offender = _methods.FirstOrDefault(static method => method.GetCustomAttributes(typeof(TAttribute), inherit: true).Length == 0);
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_BE_DECORATED_OR_INHERIT, offender.Name, typeof(TAttribute).Name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected method returns <typeparamref name="TReturn"/>.</summary>
    /// <typeparam name="TReturn">The expected return type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected method returns another type.</exception>
    public AndConstraint<MethodInfoSelectorAssertions> Return<TReturn>(string? because = null, params object?[] becauseArgs) {
        var offender = _methods.FirstOrDefault(static method => method.ReturnType != typeof(TReturn));
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_RETURN, offender.Name, Display(typeof(TReturn)), Display(offender.ReturnType)), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected method returns <see langword="void"/>.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected method returns a value.</exception>
    public AndConstraint<MethodInfoSelectorAssertions> ReturnVoid(string? because = null, params object?[] becauseArgs) {
        var offender = _methods.FirstOrDefault(static method => method.ReturnType != typeof(void));
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_RETURN_VOID, offender.Name, Display(offender.ReturnType)), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected method is asynchronous (carries an async state machine).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected method is not asynchronous.</exception>
    public AndConstraint<MethodInfoSelectorAssertions> BeAsync(string? because = null, params object?[] becauseArgs) {
        var offender = _methods.FirstOrDefault(static method => !Attribute.IsDefined(method, typeof(AsyncStateMachineAttribute), inherit: false));
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_BE_ASYNC, offender.Name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that no selected method is asynchronous (carries an async state machine).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected method is asynchronous.</exception>
    public AndConstraint<MethodInfoSelectorAssertions> NotBeAsync(string? because = null, params object?[] becauseArgs) {
        var offender = _methods.FirstOrDefault(static method => Attribute.IsDefined(method, typeof(AsyncStateMachineAttribute), inherit: false));
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_NOT_BE_ASYNC, offender.Name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected method is virtual.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected method is not virtual.</exception>
    public AndConstraint<MethodInfoSelectorAssertions> BeVirtual(string? because = null, params object?[] becauseArgs) {
        var offender = _methods.FirstOrDefault(static method => !method.IsVirtual);
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_BE_VIRTUAL, offender.Name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that no selected method is virtual.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected method is virtual.</exception>
    public AndConstraint<MethodInfoSelectorAssertions> NotBeVirtual(string? because = null, params object?[] becauseArgs) {
        var offender = _methods.FirstOrDefault(static method => method.IsVirtual);
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_NOT_BE_VIRTUAL, offender.Name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected method is static.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected method is not static.</exception>
    public AndConstraint<MethodInfoSelectorAssertions> BeStatic(string? because = null, params object?[] becauseArgs) {
        var offender = _methods.FirstOrDefault(static method => !method.IsStatic);
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_BE_STATIC, offender.Name), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected method has the given <paramref name="accessibility"/>.</summary>
    /// <param name="accessibility">The expected declared accessibility.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected method has another accessibility.</exception>
    public AndConstraint<MethodInfoSelectorAssertions> HaveAccessModifier(AccessModifier accessibility, string? because = null, params object?[] becauseArgs) {
        var offender = _methods.FirstOrDefault(method => MethodInfoAssertions.GetAccessModifier(method) != accessibility);
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_HAVE_ACCESS_MODIFIER, offender.Name, AssertionScope.FormatInvariant(accessibility), AssertionScope.FormatInvariant(MethodInfoAssertions.GetAccessModifier(offender))), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected method has the given <paramref name="accessibility"/>.</summary>
    /// <param name="accessibility">The expected declared accessibility.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected method has another accessibility.</exception>
    public AndConstraint<MethodInfoSelectorAssertions> Be(AccessModifier accessibility, string? because = null, params object?[] becauseArgs) =>
        HaveAccessModifier(accessibility, because, becauseArgs);

    /// <summary>Asserts that no selected method has the given <paramref name="accessibility"/>.</summary>
    /// <param name="accessibility">The accessibility that must be absent.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected method has that accessibility.</exception>
    public AndConstraint<MethodInfoSelectorAssertions> NotBe(AccessModifier accessibility, string? because = null, params object?[] becauseArgs) {
        var offender = _methods.FirstOrDefault(method => MethodInfoAssertions.GetAccessModifier(method) == accessibility);
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_NOT_HAVE_ACCESS_MODIFIER, offender.Name, AssertionScope.FormatInvariant(accessibility)), because, becauseArgs));
        }

        return new(this);
    }

    /// <summary>Asserts that every selected method is declared in <typeparamref name="TDeclaring"/>.</summary>
    /// <typeparam name="TDeclaring">The expected declaring type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">A selected method is declared elsewhere.</exception>
    public AndConstraint<MethodInfoSelectorAssertions> HaveDeclaringType<TDeclaring>(string? because = null, params object?[] becauseArgs) =>
        HaveDeclaringType(typeof(TDeclaring), because, becauseArgs);

    /// <summary>Asserts that every selected method is declared in <paramref name="declaringType"/>.</summary>
    /// <param name="declaringType">The expected declaring type.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="declaringType"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">A selected method is declared elsewhere.</exception>
    public AndConstraint<MethodInfoSelectorAssertions> HaveDeclaringType(Type declaringType, string? because = null, params object?[] becauseArgs) {
        if(declaringType is null) {
            throw new ArgumentNullException(nameof(declaringType));
        }

        var offender = _methods.FirstOrDefault(method => method.DeclaringType != declaringType);
        if(offender is not null) {
            AssertionScope.Fail(AssertionScope.AppendBecause(Strings.Format(StringKeys.MEMBER_HAVE_DECLARING_TYPE, offender.Name, Display(declaringType), Display(offender.DeclaringType)), because, becauseArgs));
        }

        return new(this);
    }
}

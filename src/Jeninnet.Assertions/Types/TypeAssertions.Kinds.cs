//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

public partial class TypeAssertions {
    /// <summary>Asserts that the type is a class.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or not a class.</exception>
    public AndConstraint<TypeAssertions> BeClass(string? because = null, params object?[] becauseArgs) =>
        BeKind(static type => type.IsClass, "class", because, becauseArgs);

    /// <summary>Asserts that the type is not a class.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is a class.</exception>
    public AndConstraint<TypeAssertions> NotBeClass(string? because = null, params object?[] becauseArgs) =>
        NotBeKind(static type => type.IsClass, "class", because, becauseArgs);

    /// <summary>Asserts that the type is an interface.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or not an interface.</exception>
    public AndConstraint<TypeAssertions> BeInterface(string? because = null, params object?[] becauseArgs) =>
        BeKind(static type => type.IsInterface, "interface", because, becauseArgs);

    /// <summary>Asserts that the type is not an interface.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is an interface.</exception>
    public AndConstraint<TypeAssertions> NotBeInterface(string? because = null, params object?[] becauseArgs) =>
        NotBeKind(static type => type.IsInterface, "interface", because, becauseArgs);

    /// <summary>Asserts that the type is an enum.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or not an enum.</exception>
    public AndConstraint<TypeAssertions> BeEnum(string? because = null, params object?[] becauseArgs) =>
        BeKind(static type => type.IsEnum, "enum", because, becauseArgs);

    /// <summary>Asserts that the type is not an enum.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is an enum.</exception>
    public AndConstraint<TypeAssertions> NotBeEnum(string? because = null, params object?[] becauseArgs) =>
        NotBeKind(static type => type.IsEnum, "enum", because, becauseArgs);

    /// <summary>Asserts that the type is a delegate.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or not a delegate.</exception>
    public AndConstraint<TypeAssertions> BeDelegate(string? because = null, params object?[] becauseArgs) =>
        BeKind(ReflectionMetadata.IsDelegate, "delegate", because, becauseArgs);

    /// <summary>Asserts that the type is not a delegate.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is a delegate.</exception>
    public AndConstraint<TypeAssertions> NotBeDelegate(string? because = null, params object?[] becauseArgs) =>
        NotBeKind(ReflectionMetadata.IsDelegate, "delegate", because, becauseArgs);

    /// <summary>Asserts that the type is a value type (struct or enum).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or not a value type.</exception>
    public AndConstraint<TypeAssertions> BeValueType(string? because = null, params object?[] becauseArgs) =>
        BeKind(static type => type.IsValueType, "value type", because, becauseArgs);

    /// <summary>Asserts that the type is not a value type.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is a value type.</exception>
    public AndConstraint<TypeAssertions> NotBeValueType(string? because = null, params object?[] becauseArgs) =>
        NotBeKind(static type => type.IsValueType, "value type", because, becauseArgs);

    /// <summary>Asserts that the type is a record (class or struct), detected from compiler-generated members.</summary>
    /// <remarks>Detection uses the compiler-generated <c>&lt;Clone&gt;$</c> or <c>PrintMembers</c> members; hand-written
    /// look-alikes can be misclassified.</remarks>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or not a record.</exception>
    public AndConstraint<TypeAssertions> BeRecord(string? because = null, params object?[] becauseArgs) =>
        BeKind(ReflectionMetadata.IsRecord, "record", because, becauseArgs);

    /// <summary>Asserts that the type is not a record.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is a record.</exception>
    public AndConstraint<TypeAssertions> NotBeRecord(string? because = null, params object?[] becauseArgs) =>
        NotBeKind(ReflectionMetadata.IsRecord, "record", because, becauseArgs);

    /// <summary>Asserts that the type is generic (a constructed or open generic type).</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or not generic.</exception>
    public AndConstraint<TypeAssertions> BeGeneric(string? because = null, params object?[] becauseArgs) =>
        BeKind(static type => type.IsGenericType, "generic type", because, becauseArgs);

    /// <summary>Asserts that the type is not generic.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is generic.</exception>
    public AndConstraint<TypeAssertions> NotBeGeneric(string? because = null, params object?[] becauseArgs) =>
        NotBeKind(static type => type.IsGenericType, "generic type", because, becauseArgs);

    /// <summary>Asserts that the type is nested inside another type.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or not nested.</exception>
    public AndConstraint<TypeAssertions> BeNested(string? because = null, params object?[] becauseArgs) =>
        BeKind(static type => type.IsNested, "nested type", because, becauseArgs);

    /// <summary>Asserts that the type is not nested.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is nested.</exception>
    public AndConstraint<TypeAssertions> NotBeNested(string? because = null, params object?[] becauseArgs) =>
        NotBeKind(static type => type.IsNested, "nested type", because, becauseArgs);

    /// <summary>Asserts that the type is immutable from its public surface.</summary>
    /// <remarks>Heuristic: public instance fields must be readonly and public instance setters must be init-only or absent.
    /// Inherited public state is included; private mutable state is ignored. This is a structural check, not a proof.</remarks>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or exposes mutable state.</exception>
    public AndConstraint<TypeAssertions> BeImmutable(string? because = null, params object?[] becauseArgs) =>
        BeKind(IsImmutable, "immutable type", because, becauseArgs);

    /// <summary>Asserts that the type is not immutable from its public surface.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is structurally immutable.</exception>
    public AndConstraint<TypeAssertions> NotBeImmutable(string? because = null, params object?[] becauseArgs) =>
        NotBeKind(IsImmutable, "immutable type", because, becauseArgs);

    /// <summary>Asserts that the type is public.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is not public.</exception>
    public AndConstraint<TypeAssertions> BePublic(string? because = null, params object?[] becauseArgs) =>
        HaveAccessModifier(AccessModifier.Public, because, becauseArgs);

    /// <summary>Asserts that the type is internal.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is not internal.</exception>
    public AndConstraint<TypeAssertions> BeInternal(string? because = null, params object?[] becauseArgs) =>
        HaveAccessModifier(AccessModifier.Internal, because, becauseArgs);

    /// <summary>Asserts that the type is private.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is not private.</exception>
    public AndConstraint<TypeAssertions> BePrivate(string? because = null, params object?[] becauseArgs) =>
        HaveAccessModifier(AccessModifier.Private, because, becauseArgs);

    /// <summary>Asserts that the type is protected.</summary>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is not protected.</exception>
    public AndConstraint<TypeAssertions> BeProtected(string? because = null, params object?[] becauseArgs) =>
        HaveAccessModifier(AccessModifier.Protected, because, becauseArgs);

    /// <summary>Asserts that the type is the same reflected instance as <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected type instance.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The instances differ.</exception>
    public AndConstraint<TypeAssertions> BeSameAs(Type? expected, string? because = null, params object?[] becauseArgs) {
        if(!ReferenceEquals(_value, expected)) {
            Fail(Strings.Format(StringKeys.TYPE_BE_SAME_AS, Expression, Format(expected)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type is not the same reflected instance as <paramref name="unexpected"/>.</summary>
    /// <param name="unexpected">The type instance the subject must not be.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The instances are the same.</exception>
    public AndConstraint<TypeAssertions> NotBeSameAs(Type? unexpected, string? because = null, params object?[] becauseArgs) {
        if(ReferenceEquals(_value, unexpected)) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_BE_SAME_AS, Expression, Format(unexpected)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type is assignable from <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The candidate type that must be assignable to the subject.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or not assignable from <typeparamref name="T"/>.</exception>
    public AndConstraint<TypeAssertions> BeAssignableFrom<T>(string? because = null, params object?[] becauseArgs) {
        if(_value is null || !_value.IsAssignableFrom(typeof(T))) {
            Fail(Strings.Format(StringKeys.TYPE_BE_ASSIGNABLE_FROM, Expression, Format(typeof(T)), _value is null ? FOUND_NULL : WAS_NOT), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type is a subclass of <typeparamref name="TBase"/> (excluding the base itself).</summary>
    /// <typeparam name="TBase">The base class.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or not a subclass.</exception>
    public AndConstraint<TypeAssertions> BeSubclassOf<TBase>(string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value == typeof(TBase) || !_value.IsSubclassOf(typeof(TBase))) {
            Fail(Strings.Format(StringKeys.TYPE_BE_SUBCLASS_OF, Expression, Format(typeof(TBase)), _value is null ? FOUND_NULL : WAS_NOT), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type is a base of <typeparamref name="TDerived"/> (excluding the type itself).</summary>
    /// <typeparam name="TDerived">The derived type.</typeparam>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or not a base.</exception>
    public AndConstraint<TypeAssertions> BeBaseOf<TDerived>(string? because = null, params object?[] becauseArgs) {
        if(_value is null || _value == typeof(TDerived) || !_value.IsAssignableFrom(typeof(TDerived))) {
            Fail(Strings.Format(StringKeys.TYPE_BE_BASE_OF, Expression, Format(typeof(TDerived)), _value is null ? FOUND_NULL : WAS_NOT), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type has the simple name <paramref name="expected"/> (ordinal).</summary>
    /// <param name="expected">The expected simple name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or has another name.</exception>
    public AndConstraint<TypeAssertions> HaveName(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(!string.Equals(_value?.Name, expected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.TYPE_HAVE_NAME, Expression, expected, _value?.Name ?? "<null>"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type has the full name <paramref name="expected"/> (ordinal).</summary>
    /// <param name="expected">The expected full name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or has another full name.</exception>
    public AndConstraint<TypeAssertions> HaveFullName(string expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(!string.Equals(_value?.FullName, expected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.TYPE_HAVE_FULL_NAME, Expression, expected, _value?.FullName ?? "<null>"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type is declared in <paramref name="expected"/> (ordinal; null matches the global namespace).</summary>
    /// <param name="expected">The expected namespace, or <see langword="null"/> for the global namespace.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or lives in another namespace.</exception>
    public AndConstraint<TypeAssertions> HaveNamespace(string? expected, string? because = null, params object?[] becauseArgs) {
        if(!string.Equals(_value?.Namespace, expected, StringComparison.Ordinal)) {
            Fail(Strings.Format(StringKeys.TYPE_HAVE_NAMESPACE, Expression, expected ?? "<null>", _value?.Namespace ?? "<null>"), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type has the direct base type <paramref name="expected"/>.</summary>
    /// <param name="expected">The expected direct base type.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expected"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or has another base type.</exception>
    public AndConstraint<TypeAssertions> HaveBaseType(Type expected, string? because = null, params object?[] becauseArgs) {
        if(expected is null) {
            throw new ArgumentNullException(nameof(expected));
        }

        if(_value?.BaseType != expected) {
            Fail(Strings.Format(StringKeys.TYPE_HAVE_BASE_TYPE, Expression, Format(expected), Format(_value?.BaseType)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type has exactly <paramref name="expected"/> generic parameters.</summary>
    /// <param name="expected">The expected generic parameter count.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="expected"/> is negative.</exception>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or has another count.</exception>
    public AndConstraint<TypeAssertions> HaveGenericParameter(int expected, string? because = null, params object?[] becauseArgs) {
        if(expected < 0) {
            throw new ArgumentOutOfRangeException(nameof(expected), "Generic parameter count must not be negative.");
        }

        var actual = _value?.GetGenericArguments().Length ?? 0;
        if(_value is null || actual != expected) {
            Fail(Strings.Format(StringKeys.TYPE_HAVE_GENERIC_PARAMETER, Expression, AssertionScope.FormatInvariant(expected), AssertionScope.FormatInvariant(actual)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type declares a generic parameter named <paramref name="name"/> (ordinal).</summary>
    /// <param name="name">The expected generic parameter name.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">The type is <see langword="null"/> or has no such generic parameter.</exception>
    public AndConstraint<TypeAssertions> HaveGenericParameter(string name, string? because = null, params object?[] becauseArgs) {
        if(name is null) {
            throw new ArgumentNullException(nameof(name));
        }

        if(_value?.GetGenericArguments().Any(argument => string.Equals(argument.Name, name, StringComparison.Ordinal)) != true) {
            Fail(Strings.Format(StringKeys.TYPE_HAVE_GENERIC_PARAMETER_NAME, Expression, name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Asserts that the type is not decorated with <typeparamref name="TAttribute"/> matching <paramref name="predicate"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="predicate">The condition the attribute must not satisfy.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    /// <returns>An <see cref="AndConstraint{TParent}"/> for chaining further assertions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    /// <exception cref="AssertionFailedException">A matching attribute is present.</exception>
    public AndConstraint<TypeAssertions> NotBeDecoratedWith<TAttribute>(Expression<Func<TAttribute, bool>> predicate, string? because = null, params object?[] becauseArgs) where TAttribute : Attribute {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        if(_value?.GetCustomAttributes(typeof(TAttribute), inherit: true).OfType<TAttribute>().Any(predicate.Compile()) == true) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_BE_DECORATED_PREDICATE, Expression, typeof(TAttribute).Name), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Applies a kind predicate and reports the expected and actual kinds.</summary>
    /// <param name="predicate">The kind predicate.</param>
    /// <param name="kind">The expected kind name used in the message.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    private AndConstraint<TypeAssertions> BeKind(Func<Type, bool> predicate, string kind, string? because, object?[] becauseArgs) {
        if(_value is null || !predicate(_value)) {
            Fail(Strings.Format(StringKeys.TYPE_BE_KIND, Expression, kind, KindOf(_value)), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Applies a negated kind predicate and reports the expected kind.</summary>
    /// <param name="predicate">The kind predicate.</param>
    /// <param name="kind">The kind name used in the message.</param>
    /// <param name="because">The reason for the assertion, appended to the failure message.</param>
    /// <param name="becauseArgs">Optional arguments used to format <paramref name="because"/>.</param>
    private AndConstraint<TypeAssertions> NotBeKind(Func<Type, bool> predicate, string kind, string? because, object?[] becauseArgs) {
        if(_value is not null && predicate(_value)) {
            Fail(Strings.Format(StringKeys.TYPE_NOT_BE_KIND, Expression, kind), because, becauseArgs);
        }

        return new(this);
    }

    /// <summary>Names the reflected kind of a type for diagnostics.</summary>
    /// <param name="type">The type to classify (may be <see langword="null"/>).</param>
    /// <returns>A short kind name, or <c>&lt;null&gt;</c>.</returns>
    private static string KindOf(Type? type) {
        if(type is null) {
            return "<null>";
        }

        if(type.IsEnum) {
            return "enum";
        }

        if(type.IsInterface) {
            return "interface";
        }

        if(ReflectionMetadata.IsDelegate(type)) {
            return "delegate";
        }

        if(ReflectionMetadata.IsRecord(type)) {
            return "record";
        }

        if(type.IsClass) {
            return "class";
        }

        return type.IsValueType ? "value type" : "type";
    }

    /// <summary>Checks whether a type is structurally immutable from its public surface.</summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns><see langword="true"/> when public fields are readonly and public setters are init-only or absent.</returns>
    private static bool IsImmutable(Type type) => ReflectionMetadata.IsImmutable(type);
}

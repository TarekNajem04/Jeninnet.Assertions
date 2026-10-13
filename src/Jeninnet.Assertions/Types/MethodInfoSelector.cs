//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

/// <summary>Selects the declared methods of one or more types down to an architecture-relevant subset. Each filter returns a new selector.</summary>
/// <remarks>Only methods declared on the given types are considered (inherited members are excluded);
/// property/event accessors and operators (special-name methods) are never selected.</remarks>
public class MethodInfoSelector : IEnumerable<MethodInfo> {
    private readonly MethodInfo[] _methods;

    /// <summary>Creates a selector over the declared methods of <paramref name="type"/>.</summary>
    /// <param name="type">The type whose declared methods are selected.</param>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
    public MethodInfoSelector(Type type)
        : this([type ?? throw new ArgumentNullException(nameof(type))]) {
    }

    /// <summary>Creates a selector over the declared methods of <paramref name="types"/>.</summary>
    /// <param name="types">The types whose declared methods are selected.</param>
    /// <exception cref="ArgumentNullException"><paramref name="types"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="types"/> contains <see langword="null"/>.</exception>
    public MethodInfoSelector(IEnumerable<Type> types) {
        if(types is null) {
            throw new ArgumentNullException(nameof(types));
        }

        var resolved = types.ToArray();
        if(resolved.Any(static type => type is null)) {
            throw new ArgumentException("The types must not contain null.", nameof(types));
        }

        _methods = [.. resolved
            .SelectMany(static type => type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            .Where(static method => (method.Attributes & MethodAttributes.SpecialName) == 0)];
    }

    private MethodInfoSelector(MethodInfo[] methods) => _methods = methods;

    /// <summary>Keeps public methods.</summary>
    /// <returns>A new selector with the filtered methods.</returns>
    public MethodInfoSelector ThatArePublic() =>
        new(_methods.Where(static method => method.IsPublic).ToArray());

    /// <summary>Keeps private methods.</summary>
    /// <returns>A new selector with the filtered methods.</returns>
    public MethodInfoSelector ThatArePrivate() =>
        new(_methods.Where(static method => method.IsPrivate).ToArray());

    /// <summary>Keeps protected methods.</summary>
    /// <returns>A new selector with the filtered methods.</returns>
    public MethodInfoSelector ThatAreProtected() =>
        new(_methods.Where(static method => method.IsFamily).ToArray());

    /// <summary>Keeps internal methods.</summary>
    /// <returns>A new selector with the filtered methods.</returns>
    public MethodInfoSelector ThatAreInternal() =>
        new(_methods.Where(static method => method.IsAssembly).ToArray());

    /// <summary>Keeps public or internal methods.</summary>
    /// <returns>A new selector with the filtered methods.</returns>
    public MethodInfoSelector ThatArePublicOrInternal() =>
        new(_methods.Where(static method => method.IsPublic || method.IsAssembly).ToArray());

    /// <summary>Keeps protected or protected-internal methods.</summary>
    /// <returns>A new selector with the filtered methods.</returns>
    public MethodInfoSelector ThatAreProtectedOrInternal() =>
        new(_methods.Where(static method => method.IsFamily || method.IsFamilyOrAssembly).ToArray());

    /// <summary>Keeps static methods.</summary>
    /// <returns>A new selector with the filtered methods.</returns>
    public MethodInfoSelector ThatAreStatic() =>
        new(_methods.Where(static method => method.IsStatic).ToArray());

    /// <summary>Keeps instance methods.</summary>
    /// <returns>A new selector with the filtered methods.</returns>
    public MethodInfoSelector ThatAreNotStatic() =>
        new(_methods.Where(static method => !method.IsStatic).ToArray());

    /// <summary>Keeps virtual methods.</summary>
    /// <returns>A new selector with the filtered methods.</returns>
    public MethodInfoSelector ThatAreVirtual() =>
        new(_methods.Where(static method => method.IsVirtual).ToArray());

    /// <summary>Keeps non-virtual methods.</summary>
    /// <returns>A new selector with the filtered methods.</returns>
    public MethodInfoSelector ThatAreNotVirtual() =>
        new(_methods.Where(static method => !method.IsVirtual).ToArray());

    /// <summary>Keeps abstract methods.</summary>
    /// <returns>A new selector with the filtered methods.</returns>
    public MethodInfoSelector ThatAreAbstract() =>
        new(_methods.Where(static method => method.IsAbstract).ToArray());

    /// <summary>Keeps non-abstract methods.</summary>
    /// <returns>A new selector with the filtered methods.</returns>
    public MethodInfoSelector ThatAreNotAbstract() =>
        new(_methods.Where(static method => !method.IsAbstract).ToArray());

    /// <summary>Keeps async methods (carrying an async state machine).</summary>
    /// <returns>A new selector with the filtered methods.</returns>
    public MethodInfoSelector ThatAreAsync() =>
        new(_methods.Where(static method => Attribute.IsDefined(method, typeof(AsyncStateMachineAttribute), inherit: false)).ToArray());

    /// <summary>Keeps non-async methods.</summary>
    /// <returns>A new selector with the filtered methods.</returns>
    public MethodInfoSelector ThatAreNotAsync() =>
        new(_methods.Where(static method => !Attribute.IsDefined(method, typeof(AsyncStateMachineAttribute), inherit: false)).ToArray());

    /// <summary>Keeps methods returning <typeparamref name="TReturn"/>.</summary>
    /// <typeparam name="TReturn">The return type.</typeparam>
    /// <returns>A new selector with the filtered methods.</returns>
    public MethodInfoSelector ThatReturn<TReturn>() =>
        new(_methods.Where(static method => method.ReturnType == typeof(TReturn)).ToArray());

    /// <summary>Keeps methods not returning <typeparamref name="TReturn"/>.</summary>
    /// <typeparam name="TReturn">The return type.</typeparam>
    /// <returns>A new selector with the filtered methods.</returns>
    public MethodInfoSelector ThatDoNotReturn<TReturn>() =>
        new(_methods.Where(static method => method.ReturnType != typeof(TReturn)).ToArray());

    /// <summary>Keeps methods returning <see langword="void"/>.</summary>
    /// <returns>A new selector with the filtered methods.</returns>
    public MethodInfoSelector ThatReturnVoid() =>
        new(_methods.Where(static method => method.ReturnType == typeof(void)).ToArray());

    /// <summary>Keeps methods returning a value.</summary>
    /// <returns>A new selector with the filtered methods.</returns>
    public MethodInfoSelector ThatDoNotReturnVoid() =>
        new(_methods.Where(static method => method.ReturnType != typeof(void)).ToArray());

    /// <summary>Keeps methods decorated with <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <returns>A new selector with the filtered methods.</returns>
    public MethodInfoSelector ThatAreDecoratedWith<TAttribute>() where TAttribute : Attribute =>
        new(_methods.Where(static method => method.GetCustomAttributes(typeof(TAttribute), inherit: false).Length != 0).ToArray());

    /// <summary>Keeps methods not decorated with <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <returns>A new selector with the filtered methods.</returns>
    public MethodInfoSelector ThatAreNotDecoratedWith<TAttribute>() where TAttribute : Attribute =>
        new(_methods.Where(static method => method.GetCustomAttributes(typeof(TAttribute), inherit: false).Length == 0).ToArray());

    /// <summary>Keeps methods decorated with <typeparamref name="TAttribute"/> or inheriting it from a base method.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <returns>A new selector with the filtered methods.</returns>
    public MethodInfoSelector ThatAreDecoratedWithOrInherit<TAttribute>() where TAttribute : Attribute =>
        new(_methods.Where(static method => method.GetCustomAttributes(typeof(TAttribute), inherit: true).Length != 0).ToArray());

    /// <summary>Keeps methods neither decorated with <typeparamref name="TAttribute"/> nor inheriting it.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <returns>A new selector with the filtered methods.</returns>
    public MethodInfoSelector ThatAreNotDecoratedWithOrInherit<TAttribute>() where TAttribute : Attribute =>
        new(_methods.Where(static method => method.GetCustomAttributes(typeof(TAttribute), inherit: true).Length == 0).ToArray());

    /// <summary>Keeps methods satisfying <paramref name="predicate"/> (the single predicate filter; there is no separate <c>Where</c>).</summary>
    /// <param name="predicate">The condition a method must satisfy to be kept.</param>
    /// <returns>A new selector with the filtered methods.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public MethodInfoSelector ThatSatisfy(Func<MethodInfo, bool> predicate) {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        return new(_methods.Where(predicate.Invoke).ToArray());
    }

    /// <summary>Selects the return types of the selected methods for further type selection.</summary>
    /// <returns>A <see cref="TypeSelector"/> over the return types.</returns>
    public TypeSelector ReturnTypes() =>
        new(_methods.Select(static method => method.ReturnType));

    /// <summary>Gets the selected methods.</summary>
    /// <returns>The selected methods.</returns>
    public MethodInfo[] ToArray() => [.. _methods];

    /// <summary>Returns an enumerator that iterates through the selected methods.</summary>
    /// <returns>An enumerator over the selected methods.</returns>
    public IEnumerator<MethodInfo> GetEnumerator() => ((IEnumerable<MethodInfo>)_methods).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>Creates assertions over the selected methods.</summary>
    /// <returns>A <see cref="MethodInfoSelectorAssertions"/> for the selected methods.</returns>
    public MethodInfoSelectorAssertions Should() => new([.. _methods]);
}

//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

/// <summary>Selects the declared properties of one or more types down to an architecture-relevant subset. Each filter returns a new selector.</summary>
/// <remarks>Only properties declared on the given types are considered (inherited members are excluded).
/// Accessibility filters match when any accessor (getter or setter) has the requested accessibility.</remarks>
public class PropertyInfoSelector : IEnumerable<PropertyInfo> {
    private readonly PropertyInfo[] _properties;

    /// <summary>Creates a selector over the declared properties of <paramref name="type"/>.</summary>
    /// <param name="type">The type whose declared properties are selected.</param>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
    public PropertyInfoSelector(Type type)
        : this([type ?? throw new ArgumentNullException(nameof(type))]) {
    }

    /// <summary>Creates a selector over the declared properties of <paramref name="types"/>.</summary>
    /// <param name="types">The types whose declared properties are selected.</param>
    /// <exception cref="ArgumentNullException"><paramref name="types"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="types"/> contains <see langword="null"/>.</exception>
    public PropertyInfoSelector(IEnumerable<Type> types) {
        if(types is null) {
            throw new ArgumentNullException(nameof(types));
        }

        var resolved = types.ToArray();
        if(resolved.Any(static type => type is null)) {
            throw new ArgumentException("The types must not contain null.", nameof(types));
        }

        _properties = [.. resolved
            .SelectMany(static type => type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))];
    }

    private PropertyInfoSelector(PropertyInfo[] properties) => _properties = properties;

    /// <summary>Keeps properties with a public accessor.</summary>
    /// <returns>A new selector with the filtered properties.</returns>
    public PropertyInfoSelector ThatArePublic() =>
        new(_properties.Where(static property => Accessors(property).Any(static accessor => accessor.IsPublic)).ToArray());

    /// <summary>Keeps properties with a private accessor.</summary>
    /// <returns>A new selector with the filtered properties.</returns>
    public PropertyInfoSelector ThatArePrivate() =>
        new(_properties.Where(static property => Accessors(property).Any(static accessor => accessor.IsPrivate)).ToArray());

    /// <summary>Keeps properties with a protected accessor.</summary>
    /// <returns>A new selector with the filtered properties.</returns>
    public PropertyInfoSelector ThatAreProtected() =>
        new(_properties.Where(static property => Accessors(property).Any(static accessor => accessor.IsFamily)).ToArray());

    /// <summary>Keeps properties with an internal accessor.</summary>
    /// <returns>A new selector with the filtered properties.</returns>
    public PropertyInfoSelector ThatAreInternal() =>
        new(_properties.Where(static property => Accessors(property).Any(static accessor => accessor.IsAssembly)).ToArray());

    /// <summary>Keeps properties with a public or internal accessor.</summary>
    /// <returns>A new selector with the filtered properties.</returns>
    public PropertyInfoSelector ThatArePublicOrInternal() =>
        new(_properties.Where(static property => Accessors(property).Any(static accessor => accessor.IsPublic || accessor.IsAssembly)).ToArray());

    /// <summary>Keeps properties with a protected or protected-internal accessor.</summary>
    /// <returns>A new selector with the filtered properties.</returns>
    public PropertyInfoSelector ThatAreProtectedOrInternal() =>
        new(_properties.Where(static property => Accessors(property).Any(static accessor => accessor.IsFamily || accessor.IsFamilyOrAssembly)).ToArray());

    /// <summary>Keeps static properties.</summary>
    /// <returns>A new selector with the filtered properties.</returns>
    public PropertyInfoSelector ThatAreStatic() =>
        new(_properties.Where(static property => IsStatic(property)).ToArray());

    /// <summary>Keeps instance properties.</summary>
    /// <returns>A new selector with the filtered properties.</returns>
    public PropertyInfoSelector ThatAreNotStatic() =>
        new(_properties.Where(static property => !IsStatic(property)).ToArray());

    /// <summary>Keeps virtual properties.</summary>
    /// <returns>A new selector with the filtered properties.</returns>
    public PropertyInfoSelector ThatAreVirtual() =>
        new(_properties.Where(static property => IsVirtual(property)).ToArray());

    /// <summary>Keeps non-virtual properties.</summary>
    /// <returns>A new selector with the filtered properties.</returns>
    public PropertyInfoSelector ThatAreNotVirtual() =>
        new(_properties.Where(static property => !IsVirtual(property)).ToArray());

    /// <summary>Keeps abstract properties.</summary>
    /// <returns>A new selector with the filtered properties.</returns>
    public PropertyInfoSelector ThatAreAbstract() =>
        new(_properties.Where(static property => IsAbstract(property)).ToArray());

    /// <summary>Keeps non-abstract properties.</summary>
    /// <returns>A new selector with the filtered properties.</returns>
    public PropertyInfoSelector ThatAreNotAbstract() =>
        new(_properties.Where(static property => !IsAbstract(property)).ToArray());

    /// <summary>Keeps readable properties without a setter.</summary>
    /// <returns>A new selector with the filtered properties.</returns>
    public PropertyInfoSelector ThatAreReadOnly() =>
        new(_properties.Where(static property => property.CanRead && !property.CanWrite).ToArray());

    /// <summary>Keeps writable properties without a getter.</summary>
    /// <returns>A new selector with the filtered properties.</returns>
    public PropertyInfoSelector ThatAreWriteOnly() =>
        new(_properties.Where(static property => property.CanWrite && !property.CanRead).ToArray());

    /// <summary>Keeps properties with a setter.</summary>
    /// <returns>A new selector with the filtered properties.</returns>
    public PropertyInfoSelector ThatAreWritable() =>
        new(_properties.Where(static property => property.CanWrite).ToArray());

    /// <summary>Keeps properties of type <typeparamref name="TReturn"/>.</summary>
    /// <typeparam name="TReturn">The property type.</typeparam>
    /// <returns>A new selector with the filtered properties.</returns>
    public PropertyInfoSelector OfType<TReturn>() =>
        new(_properties.Where(static property => property.PropertyType == typeof(TReturn)).ToArray());

    /// <summary>Keeps properties not of type <typeparamref name="TReturn"/>.</summary>
    /// <typeparam name="TReturn">The property type.</typeparam>
    /// <returns>A new selector with the filtered properties.</returns>
    public PropertyInfoSelector NotOfType<TReturn>() =>
        new(_properties.Where(static property => property.PropertyType != typeof(TReturn)).ToArray());

    /// <summary>Keeps properties decorated with <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <returns>A new selector with the filtered properties.</returns>
    public PropertyInfoSelector ThatAreDecoratedWith<TAttribute>() where TAttribute : Attribute =>
        new(_properties.Where(static property => property.GetCustomAttributes(typeof(TAttribute), inherit: false).Length != 0).ToArray());

    /// <summary>Keeps properties not decorated with <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <returns>A new selector with the filtered properties.</returns>
    public PropertyInfoSelector ThatAreNotDecoratedWith<TAttribute>() where TAttribute : Attribute =>
        new(_properties.Where(static property => property.GetCustomAttributes(typeof(TAttribute), inherit: false).Length == 0).ToArray());

    /// <summary>Keeps properties decorated with <typeparamref name="TAttribute"/> or inheriting it from a base property.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <returns>A new selector with the filtered properties.</returns>
    public PropertyInfoSelector ThatAreDecoratedWithOrInherit<TAttribute>() where TAttribute : Attribute =>
        new(_properties.Where(static property => property.GetCustomAttributes(typeof(TAttribute), inherit: true).Length != 0).ToArray());

    /// <summary>Keeps properties neither decorated with <typeparamref name="TAttribute"/> nor inheriting it.</summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <returns>A new selector with the filtered properties.</returns>
    public PropertyInfoSelector ThatAreNotDecoratedWithOrInherit<TAttribute>() where TAttribute : Attribute =>
        new(_properties.Where(static property => property.GetCustomAttributes(typeof(TAttribute), inherit: true).Length == 0).ToArray());

    /// <summary>Keeps properties satisfying <paramref name="predicate"/> (the single predicate filter; there is no separate <c>Where</c>).</summary>
    /// <param name="predicate">The condition a property must satisfy to be kept.</param>
    /// <returns>A new selector with the filtered properties.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public PropertyInfoSelector ThatSatisfy(Func<PropertyInfo, bool> predicate) {
        if(predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        return new(_properties.Where(predicate.Invoke).ToArray());
    }

    /// <summary>Selects the types of the selected properties for further type selection.</summary>
    /// <returns>A <see cref="TypeSelector"/> over the property types.</returns>
    public TypeSelector ReturnTypes() =>
        new(_properties.Select(static property => property.PropertyType));

    /// <summary>Gets the selected properties.</summary>
    /// <returns>The selected properties.</returns>
    public PropertyInfo[] ToArray() => [.. _properties];

    /// <summary>Returns an enumerator that iterates through the selected properties.</summary>
    /// <returns>An enumerator over the selected properties.</returns>
    public IEnumerator<PropertyInfo> GetEnumerator() => ((IEnumerable<PropertyInfo>)_properties).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>Creates assertions over the selected properties.</summary>
    /// <returns>A <see cref="PropertyInfoSelectorAssertions"/> for the selected properties.</returns>
    public PropertyInfoSelectorAssertions Should() => new([.. _properties]);

    /// <summary>Collects the accessor methods of a property (getter and/or setter).</summary>
    /// <param name="property">The property to inspect.</param>
    /// <returns>The available accessors, in getter-then-setter order.</returns>
    private static MethodInfo[] Accessors(PropertyInfo property) =>
        [.. new MethodInfo?[] { property.GetMethod, property.SetMethod }.OfType<MethodInfo>()];

    /// <summary>Determines whether the property is static (either accessor is static).</summary>
    /// <param name="property">The property to inspect.</param>
    /// <returns><see langword="true"/> when the getter or setter is static.</returns>
    private static bool IsStatic(PropertyInfo property) =>
        (property.GetMethod ?? property.SetMethod)?.IsStatic == true;

    /// <summary>Determines whether either property accessor is virtual.</summary>
    /// <param name="property">The property to inspect.</param>
    /// <returns><see langword="true"/> when the getter or setter is virtual.</returns>
    private static bool IsVirtual(PropertyInfo property) =>
        property.GetMethod?.IsVirtual == true || property.SetMethod?.IsVirtual == true;

    /// <summary>Determines whether the property is abstract (either accessor is abstract).</summary>
    /// <param name="property">The property to inspect.</param>
    /// <returns><see langword="true"/> when the getter or setter is abstract.</returns>
    private static bool IsAbstract(PropertyInfo property) =>
        property.GetMethod?.IsAbstract == true || property.SetMethod?.IsAbstract == true;
}

//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

/// <summary>Provides shared reflection helpers for type and member assertion families.</summary>
internal static class ReflectionMetadata {
    private const string IS_EXTERNAL_INIT = "System.Runtime.CompilerServices.IsExternalInit";
    private const string REQUIRED_MEMBER = "System.Runtime.CompilerServices.RequiredMemberAttribute";

    /// <summary>Gets the declared accessibility of a method or constructor.</summary>
    /// <param name="method">The method or constructor to inspect.</param>
    /// <returns>The declared <see cref="AccessModifier"/>.</returns>
    internal static AccessModifier GetAccessModifier(MethodBase method) {
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

    /// <summary>Determines whether a method or constructor declares a parameter with the given name and type.</summary>
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

    /// <summary>Determines whether a method overrides a virtual method declared by a base type.</summary>
    /// <param name="method">The method to inspect.</param>
    /// <returns><see langword="true"/> when the method is virtual and has a base definition in another type.</returns>
    internal static bool IsOverride(MethodInfo method) =>
        method.IsVirtual && method.GetBaseDefinition().DeclaringType != method.DeclaringType;

    /// <summary>Determines whether a property overrides a virtual property declared by a base type.</summary>
    /// <param name="property">The property to inspect.</param>
    /// <returns><see langword="true"/> when either accessor overrides a base accessor.</returns>
    internal static bool IsOverride(PropertyInfo property) =>
        (property.GetMethod is { } getter && IsOverride(getter))
        || (property.SetMethod is { } setter && IsOverride(setter));

    /// <summary>Determines whether a method is an extension method.</summary>
    /// <param name="method">The method to inspect.</param>
    /// <returns><see langword="true"/> when the method carries <see cref="ExtensionAttribute"/>.</returns>
    internal static bool IsExtension(MethodInfo method) =>
        method.IsDefined(typeof(ExtensionAttribute), inherit: false);

    /// <summary>Detects an init-only setter through the <c>IsExternalInit</c> required modifier.</summary>
    /// <param name="property">The property to inspect.</param>
    /// <returns><see langword="true"/> when the setter is init-only.</returns>
    internal static bool IsInitOnly(PropertyInfo property) =>
        property.SetMethod?.ReturnParameter.GetRequiredCustomModifiers()
            .Any(static modifier => string.Equals(modifier.FullName, IS_EXTERNAL_INIT, StringComparison.Ordinal)) == true;

    /// <summary>Detects a required property through <c>RequiredMemberAttribute</c> by full name (TFM-independent).</summary>
    /// <param name="property">The property to inspect.</param>
    /// <returns><see langword="true"/> when the property or an accessor carries the required-member marker.</returns>
    internal static bool IsRequired(PropertyInfo property) =>
        HasAttribute(property, REQUIRED_MEMBER)
        || HasAttribute(property.GetMethod, REQUIRED_MEMBER)
        || HasAttribute(property.SetMethod, REQUIRED_MEMBER);

    /// <summary>Determines whether a type is a concrete delegate type.</summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns><see langword="true"/> for delegate types other than the two delegate roots.</returns>
    internal static bool IsDelegate(Type type) =>
        type != typeof(Delegate)
        && type != typeof(MulticastDelegate)
        && typeof(Delegate).IsAssignableFrom(type);

    /// <summary>Detects a record class or record struct from compiler-generated members.</summary>
    /// <remarks>Record classes expose <c>&lt;Clone&gt;$</c>; record structs expose a compiler-generated
    /// <c>PrintMembers</c>. Hand-written look-alikes can be misclassified.</remarks>
    /// <param name="type">The type to inspect.</param>
    /// <returns><see langword="true"/> when a record marker is present.</returns>
    internal static bool IsRecord(Type type) {
        if(type.GetMethod("<Clone>$", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) is not null) {
            return true;
        }

        return type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Any(method => string.Equals(method.Name, "PrintMembers", StringComparison.Ordinal)
                && method.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false));
    }

    /// <summary>Formats parameter types for failure messages.</summary>
    /// <param name="parameterTypes">The parameter types.</param>
    /// <returns>The comma-separated simple type names, or <c>none</c> when the list is empty.</returns>
    internal static string FormatParameters(Type[] parameterTypes) =>
        parameterTypes.Length == 0 ? "none" : string.Join(", ", parameterTypes.Select(static type => type.Name));

    /// <summary>Formats reflected parameters for failure messages.</summary>
    /// <param name="parameters">The reflected parameters.</param>
    /// <returns>The comma-separated simple type names, or <c>none</c> when the list is empty.</returns>
    internal static string FormatParameters(ParameterInfo[] parameters) =>
        parameters.Length == 0 ? "none" : string.Join(", ", parameters.Select(static parameter => parameter.ParameterType.Name));

    /// <summary>Determines whether a member carries an attribute identified by full name.</summary>
    /// <param name="member">The member to inspect (may be <see langword="null"/>).</param>
    /// <param name="attributeFullName">The attribute type full name.</param>
    /// <returns><see langword="true"/> when a non-inherited attribute matches.</returns>
    private static bool HasAttribute(MemberInfo? member, string attributeFullName) =>
        member?.GetCustomAttributes(inherit: false)
            .Any(attribute => string.Equals(attribute.GetType().FullName, attributeFullName, StringComparison.Ordinal)) == true;
}

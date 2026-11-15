//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
#if NETSTANDARD2_0
namespace System.Diagnostics.CodeAnalysis;

/// <summary>Polyfill for the trimming analyzer attribute unavailable on .NET Standard 2.0.</summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
internal sealed class DoesNotReturnAttribute : Attribute;

/// <summary>Polyfill for the trimming analyzer attribute unavailable on .NET Standard 2.0.</summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false)]
internal sealed class RequiresUnreferencedCodeAttribute(string message) : Attribute {
    /// <summary>Gets the explanation for the requirement.</summary>
    public string Message { get; } = message;

    /// <summary>Gets or sets an optional documentation URL.</summary>
    public string? Url { get; set; }
}

/// <summary>Polyfill for the AOT analyzer attribute unavailable on .NET Standard 2.0.</summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false)]
internal sealed class RequiresDynamicCodeAttribute(string message) : Attribute {
    /// <summary>Gets the explanation for the requirement.</summary>
    public string Message { get; } = message;

    /// <summary>Gets or sets an optional documentation URL.</summary>
    public string? Url { get; set; }
}

/// <summary>Polyfill for the trimming analyzer attribute unavailable on .NET Standard 2.0.</summary>
[AttributeUsage(
    AttributeTargets.Field | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter |
    AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class |
    AttributeTargets.Interface | AttributeTargets.Struct,
    Inherited = false)]
internal sealed class DynamicallyAccessedMembersAttribute(DynamicallyAccessedMemberTypes memberTypes) : Attribute {
    /// <summary>Gets the member types that must be preserved.</summary>
    public DynamicallyAccessedMemberTypes MemberTypes { get; } = memberTypes;
}

/// <summary>Polyfill for the trimming analyzer member kinds unavailable on .NET Standard 2.0.</summary>
internal enum DynamicallyAccessedMemberTypes {
    /// <summary>No members are required.</summary>
    None = 0,
    /// <summary>Public parameterless constructors.</summary>
    PublicParameterlessConstructor = 1,
    /// <summary>Public constructors.</summary>
    PublicConstructors = 3,
    /// <summary>Non-public constructors.</summary>
    NonPublicConstructors = 4,
    /// <summary>Public methods.</summary>
    PublicMethods = 8,
    /// <summary>Non-public methods.</summary>
    NonPublicMethods = 16,
    /// <summary>Public fields.</summary>
    PublicFields = 32,
    /// <summary>Non-public fields.</summary>
    NonPublicFields = 64,
    /// <summary>Public properties.</summary>
    PublicProperties = 128,
    /// <summary>Non-public properties.</summary>
    NonPublicProperties = 256,
    /// <summary>Public events.</summary>
    PublicEvents = 512,
    /// <summary>Non-public events.</summary>
    NonPublicEvents = 1024,
    /// <summary>All public members.</summary>
    Public = PublicConstructors | PublicMethods | PublicFields | PublicProperties | PublicEvents,
    /// <summary>All non-public members.</summary>
    NonPublic = NonPublicConstructors | NonPublicMethods | NonPublicFields | NonPublicProperties | NonPublicEvents,
    /// <summary>All members.</summary>
    All = Public | NonPublic,
}
#endif

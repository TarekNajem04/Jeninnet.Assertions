//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Types;

/// <summary>Specifies the declared accessibility of a type or member, for <c>HaveAccessModifier</c> assertions.</summary>
public enum AccessModifier {
    /// <summary>The member is accessible everywhere (<c>public</c>).</summary>
    Public = 0,

    /// <summary>The member is accessible only within its declaring type (<c>private</c>).</summary>
    Private = 1,

    /// <summary>The member is accessible within its declaring type and derived types (<c>protected</c>).</summary>
    Protected = 2,

    /// <summary>The member is accessible within its assembly (<c>internal</c>).</summary>
    Internal = 3,

    /// <summary>The member is accessible within its assembly or from derived types (<c>protected internal</c>).</summary>
    ProtectedInternal = 4,

    /// <summary>The member is accessible from derived types within its assembly (<c>private protected</c>).</summary>
    PrivateProtected = 5,
}

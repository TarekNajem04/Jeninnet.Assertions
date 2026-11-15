//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
#if NET6_0
namespace System.Diagnostics.CodeAnalysis;

/// <summary>Polyfill for the AOT analyzer attribute that is internal on .NET 6.</summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false)]
internal sealed class RequiresDynamicCodeAttribute(string message) : Attribute {
    /// <summary>Gets the explanation for the requirement.</summary>
    public string Message { get; } = message;

    /// <summary>Gets or sets an optional documentation URL.</summary>
    public string? Url { get; set; }
}
#endif

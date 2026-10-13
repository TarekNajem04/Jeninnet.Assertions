//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
#if NETSTANDARD2_0
// Polyfills must live in the BCL namespaces the compiler looks them up in, so the folder/namespace split is intentional.
#pragma warning disable IDE0130, IDE0161, RCS1141, RCS1251
namespace System.Runtime.CompilerServices;

/// <summary>Compiler polyfill: captures the caller expression for <c>CallerArgumentExpression</c> on .NET Standard 2.0.</summary>
/// <param name="parameterName">The name of the parameter whose expression is captured.</param>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
internal sealed class CallerArgumentExpressionAttribute(string parameterName) : Attribute {
    /// <summary>Gets the name of the parameter whose expression is captured.</summary>
    public string ParameterName { get; } = parameterName;
}

// Required compiler shape for records/init accessors; intentionally memberless.
#pragma warning disable S2094

/// <summary>Compiler polyfill: enables <c>init</c> accessors on .NET Standard 2.0.</summary>
internal static class IsExternalInit {
}
#pragma warning restore S2094, IDE0130, IDE0161, RCS1141, RCS1251
#endif

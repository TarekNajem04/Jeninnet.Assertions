//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Marks an extension method as a custom assertion built on this library (naming convention for user extensions and tooling).</summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
#pragma warning disable RCS1251 // Marker attribute: the empty body is required to hold the attribute declaration.
public sealed class CustomAssertionAttribute : Attribute {
}
#pragma warning restore RCS1251

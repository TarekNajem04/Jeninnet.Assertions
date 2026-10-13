//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>Marks an assembly as containing only custom assertions built on this library (naming convention for user extensions and tooling).</summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
#pragma warning disable RCS1251 // Marker attribute: the empty body is required to hold the attribute declaration.
public sealed class CustomAssertionsAssemblyAttribute : Attribute {
}
#pragma warning restore RCS1251

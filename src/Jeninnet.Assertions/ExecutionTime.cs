//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions;

/// <summary>A measured execution duration, produced by <c>ExecutionTime()</c> and asserted through <c>Should()</c>.</summary>
/// <param name="Elapsed">The measured execution time.</param>
public readonly record struct ExecutionTime(TimeSpan Elapsed);

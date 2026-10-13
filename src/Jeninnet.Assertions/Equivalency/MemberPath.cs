//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Equivalency;

/// <summary>Parses and formats member paths used by equivalency inclusion/exclusion rules.</summary>
/// <remarks>
/// Supported selectors: member chains (<c>x.Address.City</c>) and constant indexers
/// (<c>x.Tags[0]</c>, <c>x.Map["key"]</c>). Paths render dotted with bracketed indices/keys
/// (e.g. <c>Address.City</c>, <c>Tags[0]</c>).
/// </remarks>
internal static class MemberPath {
    /// <summary>Parses a member-access expression into root-relative path segments.</summary>
    /// <param name="selector">The lambda whose body must be a member access on its parameter.</param>
    /// <returns>The path segments from the root.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The body is not a member access on the parameter, selects nothing, or uses a non-constant indexer.</exception>
    internal static string[] Parse(LambdaExpression selector) {
        if(selector is null) {
            throw new ArgumentNullException(nameof(selector));
        }

        var segments = new List<string>();
        var current = selector.Body;
        while(true) {
            current = Unwrap(current);
            if(current is MemberExpression member) {
                segments.Add(member.Member.Name);
                current = member.Expression;
                continue;
            }

            if(current is MethodCallExpression call
                && string.Equals(call.Method.Name, "get_Item", StringComparison.Ordinal)
                && call.Method.IsSpecialName
                && call.Arguments.Count == 1
                && call.Object is not null) {
                segments.Add(FormatIndex(EvaluateConstant(call.Arguments[0])));
                current = call.Object;
                continue;
            }

            break;
        }

        if(current is not ParameterExpression) {
            throw new ArgumentException("The selector must be a member access on the expectation parameter.", nameof(selector));
        }

        if(segments.Count == 0) {
            throw new ArgumentException("The selector must access at least one member.", nameof(selector));
        }

        segments.Reverse();
        return [.. segments];
    }

    /// <summary>Formats path segments in dotted/bracketed notation.</summary>
    /// <param name="segments">The path segments from the root.</param>
    /// <returns>The formatted path (e.g. <c>Address.City</c>, <c>Tags[0]</c>).</returns>
    internal static string Format(IReadOnlyList<string> segments) {
        var builder = new StringBuilder(segments[0]);
        for(var index = 1; index < segments.Count; index++) {
            // The char overload is unavailable on .NET Standard 2.0; ordinal comparison keeps this correct everywhere.
#pragma warning disable CA1865 // Build-verified: removing fails net6.0/net8.0/net10.0.
            if(segments[index].StartsWith("[", StringComparison.Ordinal)) {
                builder.Append(segments[index]);
            } else {
                builder.Append('.').Append(segments[index]);
            }
        }
#pragma warning restore CA1865

        return builder.ToString();
    }

    /// <summary>Strips conversion nodes from an expression to reach its underlying operand.</summary>
    /// <param name="expression">The expression to unwrap.</param>
    /// <returns>The unwrapped expression, or <see langword="null"/> when the input was null.</returns>
    private static Expression? Unwrap(Expression? expression) {
        while(expression is UnaryExpression unary
            && (unary.NodeType == ExpressionType.Convert || unary.NodeType == ExpressionType.ConvertChecked)) {
            expression = unary.Operand;
        }

        return expression;
    }

    /// <summary>Evaluates an indexer argument that must be a compile-time constant.</summary>
    /// <param name="expression">The indexer argument expression.</param>
    /// <returns>The constant value.</returns>
    /// <exception cref="ArgumentException"><paramref name="expression"/> is not a constant.</exception>
    private static object? EvaluateConstant(Expression expression) {
        expression = Unwrap(expression) ?? throw new ArgumentException("The indexer argument must be a constant.", nameof(expression));
        if(expression is ConstantExpression constant) {
            return constant.Value;
        }

        throw new ArgumentException("The indexer argument must be a constant.", nameof(expression));
    }

    /// <summary>Determines whether <paramref name="segment"/> addresses a collection index or key.</summary>
    /// <param name="segment">A path segment.</param>
    /// <returns><see langword="true"/> for bracketed segments such as <c>[0]</c> or <c>["key"]</c>.</returns>
    internal static bool IsIndex(string segment) =>
        // The char overload is unavailable on .NET Standard 2.0.
#pragma warning disable CA1865 // Build-verified: removing fails net6.0/net8.0/net10.0.
        segment.StartsWith("[", StringComparison.Ordinal);
#pragma warning restore CA1865

    /// <summary>
    /// Determines whether <paramref name="rule"/> addresses a prefix of <paramref name="path"/>. Indices reached by the
    /// comparison but not named by the rule (e.g. <c>Orders[0]</c> against a <c>Orders.Status</c> rule) are ignored, while
    /// an index named by the rule must match the reached index exactly.
    /// </summary>
    /// <param name="rule">The path segments declared by a rule.</param>
    /// <param name="path">The path segments reached by the comparison.</param>
    /// <returns><see langword="true"/> when the rule covers the beginning of the path.</returns>
    internal static bool IsPrefix(string[] rule, IReadOnlyList<string> path) {
        var ruleIndex = 0;
        var pathIndex = 0;
        while(ruleIndex < rule.Length && pathIndex < path.Count) {
            if(IsIndex(path[pathIndex]) && !IsIndex(rule[ruleIndex])) {
                pathIndex++;
                continue;
            }

            if(!string.Equals(rule[ruleIndex], path[pathIndex], StringComparison.Ordinal)) {
                return false;
            }

            ruleIndex++;
            pathIndex++;
        }

        return ruleIndex == rule.Length;
    }

    /// <summary>
    /// Determines whether <paramref name="rule"/> lies at or below <paramref name="path"/>, ignoring indices entirely so a
    /// rule written for a collection's elements still protects a path that only names the collection.
    /// </summary>
    /// <param name="rule">The path segments declared by a rule.</param>
    /// <param name="path">The path segments reached by the comparison.</param>
    /// <returns><see langword="true"/> when the rule starts with the path.</returns>
    internal static bool IsUnder(string[] rule, IReadOnlyList<string> path) {
        var ruleIndex = 0;
        var pathIndex = 0;
        while(ruleIndex < rule.Length && pathIndex < path.Count) {
            if(IsIndex(rule[ruleIndex])) {
                ruleIndex++;
                continue;
            }

            if(IsIndex(path[pathIndex])) {
                pathIndex++;
                continue;
            }

            if(!string.Equals(rule[ruleIndex], path[pathIndex], StringComparison.Ordinal)) {
                return false;
            }

            ruleIndex++;
            pathIndex++;
        }

        return true;
    }

    /// <summary>Formats an index or key value for path rendering.</summary>
    /// <param name="value">The index or key value.</param>
    /// <returns>Quoted strings as <c>["key"]</c>, nulls as <c>[&lt;null&gt;]</c>, others invariantly bracketed.</returns>
    private static string FormatIndex(object? value) => value switch {
        string text => $"[\"{text}\"]",
        null => "[<null>]",
        _ => $"[{AssertionScope.FormatInvariant(value)}]",
    };
}

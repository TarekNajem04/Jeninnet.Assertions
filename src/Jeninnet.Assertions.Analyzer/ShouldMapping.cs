//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Analyzer;

/// <summary>Decides whether a framework <c>Assert</c> call converts to a fluent <c>Should()</c> call.</summary>
/// <remarks>Shared by the analyzer and the code fix so they can never disagree. Supported sources are
/// MSTest and NUnit classic <c>Assert.AreEqual</c>/<c>IsTrue</c>-family calls, xUnit <c>Assert.Equal</c>/
/// <c>True</c>-family calls, and simple NUnit <c>Assert.That(actual, Is.X)</c> constraints. Conversions are
/// only offered when the generated code is guaranteed to compile: the target method must exist for the
/// statically known subject type, and tolerance-based or comparer-based overloads are never mapped.</remarks>
internal static class ShouldMapping {
    /// <summary>The namespace of the MSTest assertion type.</summary>
    internal const string MSTEST_ASSERT_NAMESPACE = "Microsoft.VisualStudio.TestTools.UnitTesting";

    /// <summary>The namespace of the xUnit assertion type.</summary>
    internal const string XUNIT_ASSERT_NAMESPACE = "Xunit";

    /// <summary>The namespace of the NUnit assertion type.</summary>
    internal const string NUNIT_ASSERT_NAMESPACE = "NUnit.Framework";

    /// <summary>A planned fluent replacement for one assertion call.</summary>
    internal sealed class Replacement {
        /// <summary>Creates a planned replacement.</summary>
        /// <param name="targetMethod">The fluent method name (<c>Be</c>, <c>NotBe</c>, <c>BeTrue</c>, <c>BeFalse</c>, <c>BeNull</c>, <c>NotBeNull</c>, <c>Contain</c>).</param>
        /// <param name="actual">The expression becoming the <c>Should()</c> receiver.</param>
        /// <param name="expected">The value argument (<see langword="null"/> when the target takes none).</param>
        /// <param name="because">The reason argument (<see langword="null"/> when absent).</param>
        internal Replacement(string targetMethod, ExpressionSyntax actual, ArgumentSyntax? expected, ArgumentSyntax? because) {
            TargetMethod = targetMethod;
            Actual = actual;
            Expected = expected;
            Because = because;
        }

        /// <summary>Gets the fluent method name.</summary>
        internal string TargetMethod { get; }

        /// <summary>Gets the expression becoming the <c>Should()</c> receiver.</summary>
        internal ExpressionSyntax Actual { get; }

        /// <summary>Gets the value argument (<see langword="null"/> when the target takes none).</summary>
        internal ArgumentSyntax? Expected { get; }

        /// <summary>Gets the reason argument (<see langword="null"/> when absent).</summary>
        internal ArgumentSyntax? Because { get; }
    }

    /// <summary>Determines whether <paramref name="type"/> is a supported assertion entry point.</summary>
    /// <param name="type">The containing type of a candidate method.</param>
    /// <returns><see langword="true"/> for MSTest, xUnit, or NUnit <c>Assert</c>.</returns>
    internal static bool IsSupportedAssert(ITypeSymbol? type) {
        if(type is not { Name: "Assert" }) {
            return false;
        }

        var namespaceName = type.ContainingNamespace?.ToDisplayString();
        return namespaceName is MSTEST_ASSERT_NAMESPACE or XUNIT_ASSERT_NAMESPACE or NUNIT_ASSERT_NAMESPACE;
    }

    /// <summary>Determines whether a method name is an equality assertion on any supported framework.</summary>
    /// <param name="methodName">The simple method name.</param>
    /// <returns><see langword="true"/> for <c>AreEqual</c>/<c>AreNotEqual</c>/<c>Equal</c>/<c>NotEqual</c>.</returns>
    internal static bool IsEqualityMethod(string methodName) =>
        methodName is "AreEqual" or "AreNotEqual" or "Equal" or "NotEqual";

    /// <summary>Determines whether a method name is a condition assertion on any supported framework.</summary>
    /// <param name="methodName">The simple method name.</param>
    /// <returns><see langword="true"/> for the <c>IsTrue</c>/<c>True</c> family.</returns>
    internal static bool IsConditionMethod(string methodName) =>
        methodName is "IsTrue" or "IsFalse" or "IsNull" or "IsNotNull" or "True" or "False" or "Null" or "NotNull";

    /// <summary>Maps a classic equality invocation (<c>Assert.AreEqual</c> family) to its fluent replacement.</summary>
    /// <param name="semanticModel">The semantic model for the invocation.</param>
    /// <param name="invocation">The assertion call.</param>
    /// <param name="method">The bound assertion method.</param>
    /// <param name="cancellationToken">Cancels the analysis.</param>
    /// <returns>The replacement, or <see langword="null"/> when no safe conversion exists.</returns>
    internal static Replacement? TryMapEquality(SemanticModel semanticModel, InvocationExpressionSyntax invocation, IMethodSymbol method, CancellationToken cancellationToken) {
        var arguments = invocation.ArgumentList.Arguments;
        if(arguments.Count is not (2 or 3) || HasNonEquivalenceParameter(method)) {
            return null;
        }

        var expected = NamedArgument(arguments, "expected") ?? NamedArgument(arguments, "notExpected") ?? arguments[0];
        var actual = NamedArgument(arguments, "actual") ?? arguments[1];
        var because = NamedArgument(arguments, "message") ?? (arguments.Count == 3 ? arguments[2] : null);
        if(IsDelegateReason(because)) {
            return null;
        }

        var isNegated = method.Name is "AreNotEqual" or "NotEqual";
        var target = isNegated ? "NotBe" : "Be";

        var actualType = semanticModel.GetTypeInfo(actual.Expression, cancellationToken).Type;
        if(actual.Expression is LiteralExpressionSyntax || actualType is null) {
            return null;
        }

        if(TryMapBooleanLiteral(expected, actual, because, !isNegated, out var boolean)) {
            return boolean;
        }

        if(!IsBeCapable(actualType)) {
            return null;
        }

        if(IsProvenUncompilable(semanticModel, invocation, actual.Expression, target, expected.Expression)) {
            return null;
        }

        return new Replacement(target, actual.Expression, expected, because);
    }

    /// <summary>Maps a classic condition invocation (<c>Assert.IsTrue</c> family) to its fluent replacement.</summary>
    /// <param name="semanticModel">The semantic model for the invocation.</param>
    /// <param name="invocation">The assertion call.</param>
    /// <param name="method">The bound assertion method.</param>
    /// <param name="cancellationToken">Cancels the analysis.</param>
    /// <returns>The replacement, or <see langword="null"/> when no safe conversion exists.</returns>
    internal static Replacement? TryMapCondition(SemanticModel semanticModel, InvocationExpressionSyntax invocation, IMethodSymbol method, CancellationToken cancellationToken) {
        var arguments = invocation.ArgumentList.Arguments;
        if(arguments.Count is not (1 or 2)) {
            return null;
        }

        var value = NamedArgument(arguments, "condition") ?? NamedArgument(arguments, "value") ?? arguments[0];
        var because = NamedArgument(arguments, "message") ?? (arguments.Count == 2 ? arguments[1] : null);
        if(IsDelegateReason(because)) {
            return null;
        }

        var target = method.Name switch {
            "IsTrue" or "True" => "BeTrue",
            "IsFalse" or "False" => "BeFalse",
            "IsNull" or "Null" => "BeNull",
            _ => "NotBeNull",
        };

        var valueType = semanticModel.GetTypeInfo(value.Expression, cancellationToken).Type;
        if(valueType is null || !IsConditionCapable(valueType, method.Name)) {
            return null;
        }

        if(IsProvenUncompilable(semanticModel, invocation, value.Expression, target, null)) {
            return null;
        }

        return new Replacement(target, value.Expression, null, because);
    }

    /// <summary>Maps an xUnit <c>Assert.Contains</c> invocation to its fluent <c>Contain</c> replacement.</summary>
    /// <param name="semanticModel">The semantic model for the invocation.</param>
    /// <param name="invocation">The assertion call.</param>
    /// <param name="method">The bound assertion method.</param>
    /// <param name="cancellationToken">Cancels the analysis.</param>
    /// <returns>The replacement, or <see langword="null"/> when no safe conversion exists.</returns>
    internal static Replacement? TryMapContains(SemanticModel semanticModel, InvocationExpressionSyntax invocation, IMethodSymbol method, CancellationToken cancellationToken) {
        _ = method;
        var arguments = invocation.ArgumentList.Arguments;
        if(arguments.Count is not (2 or 3)) {
            return null;
        }

        var expected = NamedArgument(arguments, "expected") ?? NamedArgument(arguments, "expectedSubstring") ?? arguments[0];
        var actual = NamedArgument(arguments, "actualString") ?? NamedArgument(arguments, "collection") ?? arguments[1];
        var because = NamedArgument(arguments, "message") ?? (arguments.Count == 3 ? arguments[2] : null);
        if(IsDelegateReason(because)) {
            return null;
        }

        var actualType = semanticModel.GetTypeInfo(actual.Expression, cancellationToken).Type;
        if(actual.Expression is LiteralExpressionSyntax || actualType is null) {
            return null;
        }

        if(IsProvenUncompilable(semanticModel, invocation, actual.Expression, "Contain", expected.Expression)) {
            return null;
        }

        return new Replacement("Contain", actual.Expression, expected, because);
    }

    /// <summary>Maps a simple NUnit <c>Assert.That(actual, Is.X)</c> constraint to its fluent replacement.</summary>
    /// <param name="semanticModel">The semantic model for the invocation.</param>
    /// <param name="invocation">The assertion call.</param>
    /// <param name="method">The bound assertion method.</param>
    /// <param name="cancellationToken">Cancels the analysis.</param>
    /// <returns>The replacement, or <see langword="null"/> when the constraint is not a supported simple form.</returns>
    internal static Replacement? TryMapConstraint(SemanticModel semanticModel, InvocationExpressionSyntax invocation, IMethodSymbol method, CancellationToken cancellationToken) {
        _ = method;
        var arguments = invocation.ArgumentList.Arguments;
        if(arguments.Count is not (2 or 3)) {
            return null;
        }

        var actual = NamedArgument(arguments, "actual") ?? arguments[0];
        var constraint = NamedArgument(arguments, "expression") ?? NamedArgument(arguments, "constraint") ?? arguments[1];
        var because = NamedArgument(arguments, "message") ?? (arguments.Count == 3 ? arguments[2] : null);
        if(IsDelegateReason(because)) {
            return null;
        }

        if(!TryDescribeConstraint(constraint.Expression, out var negated, out var kind, out var expected)) {
            return null;
        }

        var actualType = semanticModel.GetTypeInfo(actual.Expression, cancellationToken).Type;
        if(actual.Expression is LiteralExpressionSyntax || actualType is null) {
            return null;
        }

        if(kind == "EqualTo") {
            if(expected is null) {
                return null;
            }

            if(TryMapBooleanLiteral(expected, actual, because, !negated, out var boolean)) {
                return boolean;
            }

            if(!IsBeCapable(actualType)) {
                return null;
            }

            var equalityTarget = negated ? "NotBe" : "Be";
            if(IsProvenUncompilable(semanticModel, invocation, actual.Expression, equalityTarget, expected.Expression)) {
                return null;
            }

            return new Replacement(equalityTarget, actual.Expression, expected, because);
        }

        if(negated && kind != "Null") {
            return null;
        }

        var target = kind switch {
            "True" => "BeTrue",
            "False" => "BeFalse",
            "Null" => negated ? "NotBeNull" : "BeNull",
            _ => null,
        };
        if(target is null || !IsConditionCapable(actualType, kind is "True" or "False" ? "IsTrue" : "IsNull")) {
            return null;
        }

        if(IsProvenUncompilable(semanticModel, invocation, actual.Expression, target, null)) {
            return null;
        }

        return new Replacement(target, actual.Expression, null, because);
    }

    /// <summary>Describes a simple NUnit constraint expression (<c>Is.EqualTo(x)</c>, <c>Is.Not.Null</c>, ...).</summary>
    /// <param name="expression">The constraint expression to inspect.</param>
    /// <param name="negated">When supported, whether the constraint is negated (<c>Is.Not.*</c>).</param>
    /// <param name="kind">When supported, the canonical kind: <c>EqualTo</c>, <c>True</c>, <c>False</c>, or <c>Null</c>.</param>
    /// <param name="expected">When supported and present, the constraint's value argument (for <c>Is.EqualTo</c>).</param>
    /// <returns><see langword="true"/> when the constraint is one of the supported simple forms.</returns>
    private static bool TryDescribeConstraint(ExpressionSyntax expression, out bool negated, out string kind, out ArgumentSyntax? expected) {
        negated = false;
        kind = "";
        expected = null;

        var node = expression;
        if(node is InvocationExpressionSyntax invocation) {
            expected = invocation.ArgumentList.Arguments.Count >= 1 ? invocation.ArgumentList.Arguments[0] : null;
            node = invocation.Expression;
        }

        switch(DottedName(node)) {
            case "Is.EqualTo":
                kind = "EqualTo";
                return true;
            case "Is.Not.EqualTo":
                kind = "EqualTo";
                negated = true;
                return true;
            case "Is.True":
                kind = "True";
                return true;
            case "Is.False":
                kind = "False";
                return true;
            case "Is.Null":
                kind = "Null";
                return true;
            case "Is.Not.Null":
                kind = "Null";
                negated = true;
                return true;
            default:
                return false;
        }
    }

    /// <summary>Renders a member-access chain (e.g. <c>Is.Not.Null</c>) as a dotted name.</summary>
    /// <param name="expression">The expression to render.</param>
    /// <returns>The dotted name, or an empty string when the expression is not a plain identifier chain.</returns>
    private static string DottedName(ExpressionSyntax? expression) {
        var parts = new Stack<string>();
        while(expression is MemberAccessExpressionSyntax member) {
            parts.Push(member.Name.Identifier.Text);
            expression = member.Expression;
        }

        if(expression is IdentifierNameSyntax identifier) {
            parts.Push(identifier.Identifier.Text);
        } else if(expression is not null) {
            return "";
        }

        return string.Join(".", parts);
    }

    /// <summary>Finds an argument passed by name, tolerating calls that use positional arguments.</summary>
    /// <param name="arguments">The invocation's argument list.</param>
    /// <param name="name">The parameter name to look for.</param>
    /// <returns>The matching argument, or <see langword="null"/> when it is positional or absent.</returns>
    private static ArgumentSyntax? NamedArgument(SeparatedSyntaxList<ArgumentSyntax> arguments, string name) {
        foreach(var argument in arguments) {
            if(argument.NameColon?.Name.Identifier.Text == name) {
                return argument;
            }
        }

        return null;
    }

    /// <summary>Determines whether a reason argument is a delegate, which the fluent API cannot accept as <c>because</c>.</summary>
    /// <param name="reason">The candidate reason argument.</param>
    /// <returns><see langword="true"/> for lambdas and anonymous methods.</returns>
    private static bool IsDelegateReason(ArgumentSyntax? reason) =>
        reason?.Expression is LambdaExpressionSyntax or AnonymousMethodExpressionSyntax;

    /// <summary>Determines whether the assertion method takes a parameter the fluent API cannot express
    /// (tolerance types, <c>ignoreCase</c>, comparers, or a culture).</summary>
    /// <param name="method">The bound assertion method.</param>
    /// <returns><see langword="true"/> when the call must be left manual.</returns>
    private static bool HasNonEquivalenceParameter(IMethodSymbol method) {
        foreach(var parameter in method.Parameters) {
            if(parameter.Type.SpecialType is SpecialType.System_Double or SpecialType.System_Single or SpecialType.System_Decimal) {
                return true;
            }

            if(parameter.Type.SpecialType == SpecialType.System_Boolean && parameter.Name == "ignoreCase") {
                return true;
            }

            if(parameter.Type is INamedTypeSymbol { Name: "IEqualityComparer" }) {
                return true;
            }

            if(parameter.Type.ToDisplayString() == "System.Globalization.CultureInfo") {
                return true;
            }
        }

        return false;
    }

    /// <summary>Maps <c>AreEqual(true/false, actual)</c> and <c>Is.EqualTo(true/false)</c> to <c>BeTrue</c>/<c>BeFalse</c>.</summary>
    /// <param name="expected">The expected-value argument.</param>
    /// <param name="actual">The actual-value argument becoming the subject.</param>
    /// <param name="because">The reason argument, carried over unchanged.</param>
    /// <param name="isEqual">Whether the original assertion is an equality (not a negation).</param>
    /// <param name="replacement">The planned replacement when the literal is a boolean.</param>
    /// <returns><see langword="true"/> when <paramref name="expected"/> is a boolean literal.</returns>
    private static bool TryMapBooleanLiteral(ArgumentSyntax expected, ArgumentSyntax actual, ArgumentSyntax? because, bool isEqual, out Replacement? replacement) {
        replacement = null;
        if(expected.Expression is not LiteralExpressionSyntax literal
            || literal.Token.ValueText is not ("true" or "false")) {
            return false;
        }

#pragma warning disable IDE0047 // Chained == without parentheses misreads as one comparison.
        var target = (literal.Token.ValueText == "true") == isEqual ? "BeTrue" : "BeFalse";
#pragma warning restore IDE0047
        replacement = new Replacement(target, actual.Expression, null, because);
        return true;
    }

    /// <summary>Determines whether the subject type has a fluent <c>Be</c> overload for a value comparison.</summary>
    /// <param name="type">The statically known subject type.</param>
    /// <returns><see langword="true"/> when <c>Should().Be(...)</c> is guaranteed to exist.</returns>
    private static bool IsBeCapable(ITypeSymbol type) {
        UnwrapNullable(type, out var underlying, out var isNullable);
        if(underlying.SpecialType == SpecialType.System_Boolean) {
            return isNullable;
        }

        if(underlying.SpecialType is SpecialType.System_String or SpecialType.System_Object) {
            return true;
        }

        if(IsNumeric(underlying) || underlying.TypeKind == TypeKind.Enum) {
            return isNullable;
        }

        return underlying.ToDisplayString() is "System.Guid" or "System.DateTime" or "System.DateOnly" or "System.TimeOnly" or "System.DateTimeOffset" or "System.TimeSpan";
    }

    /// <summary>Determines whether the subject type supports the planned condition verb.</summary>
    /// <param name="type">The statically known subject type.</param>
    /// <param name="methodName">The original assertion method name.</param>
    /// <returns><see langword="true"/> when the fluent condition call is guaranteed to exist.</returns>
    private static bool IsConditionCapable(ITypeSymbol type, string methodName) {
        UnwrapNullable(type, out var underlying, out var isNullable);
        if(methodName is "IsTrue" or "IsFalse" or "True" or "False") {
            return underlying.SpecialType == SpecialType.System_Boolean;
        }

        return !underlying.IsValueType || isNullable;
    }

    /// <summary>Unwraps a <see cref="Nullable{T}"/> subject type to its underlying type.</summary>
    /// <param name="type">The type to inspect.</param>
    /// <param name="underlying">The underlying type, or <paramref name="type"/> when it is not nullable.</param>
    /// <param name="isNullable">Whether <paramref name="type"/> was a <see cref="Nullable{T}"/>.</param>
    private static void UnwrapNullable(ITypeSymbol type, out ITypeSymbol underlying, out bool isNullable) {
        underlying = type;
        isNullable = false;
        if(type is INamedTypeSymbol { IsGenericType: true } named && named.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T) {
            underlying = named.TypeArguments[0];
            isNullable = true;
        }
    }

    /// <summary>Determines whether a type is one of the built-in numeric types.</summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns><see langword="true"/> for the C# numeric primitive types.</returns>
    private static bool IsNumeric(ITypeSymbol type) => type.SpecialType is
        SpecialType.System_SByte or SpecialType.System_Byte or SpecialType.System_Int16 or SpecialType.System_UInt16 or
        SpecialType.System_Int32 or SpecialType.System_UInt32 or SpecialType.System_Int64 or SpecialType.System_UInt64 or
        SpecialType.System_Single or SpecialType.System_Double or SpecialType.System_Decimal;

    /// <summary>Determines whether the planned fluent call demonstrably fails to compile.</summary>
    /// <param name="semanticModel">The semantic model for the invocation.</param>
    /// <param name="invocation">The assertion call (provides the speculative binding position).</param>
    /// <param name="actual">The expression becoming the <c>Should()</c> receiver.</param>
    /// <param name="target">The planned fluent method name.</param>
    /// <param name="expected">The value argument (<see langword="null"/> when the target takes none).</param>
    /// <returns><see langword="true"/> only when <c>Should()</c> binds but the target method does not;
    /// an unresolvable <c>Should()</c> (e.g. missing using) is inconclusive and trusts the type rules.</returns>
    private static bool IsProvenUncompilable(SemanticModel semanticModel, InvocationExpressionSyntax invocation, ExpressionSyntax actual, string target, ExpressionSyntax? expected) {
        var shouldCall = SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, actual, SyntaxFactory.IdentifierName("Should")));
        if(semanticModel.GetSpeculativeSymbolInfo(invocation.SpanStart, shouldCall, SpeculativeBindingOption.BindAsExpression).Symbol is not IMethodSymbol) {
            return false;
        }

        var arguments = expected is null
            ? SyntaxFactory.ArgumentList()
            : SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(expected)));
        var fluentCall = SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, shouldCall, SyntaxFactory.IdentifierName(target)),
            arguments);
        return semanticModel.GetSpeculativeSymbolInfo(invocation.SpanStart, fluentCall, SpeculativeBindingOption.BindAsExpression).Symbol is not IMethodSymbol;
    }
}

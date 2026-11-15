//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Analyzer;

/// <summary>Maps simple Shouldly and NFluent assertions to their Jeninnet equivalents.</summary>
/// <remarks>
/// Only patterns with an unambiguous, compile-verified fluent replacement are mapped. Tolerance, comparer,
/// culture, custom-message overloads, chained results, approval snapshots, and dynamic assertions stay manual.
/// </remarks>
internal static class PeerAssertionMapping {
    /// <summary>The namespace of the Shouldly assertion extensions.</summary>
    internal const string SHOULDLY_NAMESPACE = "Shouldly";

    /// <summary>The namespace of the NFluent assertion API.</summary>
    internal const string NFLUENT_NAMESPACE = "NFluent";

    /// <summary>Determines whether <paramref name="type"/> belongs to a supported peer assertion library.</summary>
    /// <param name="type">The containing type of a candidate method.</param>
    /// <returns><see langword="true"/> for the Shouldly or NFluent assertion namespaces.</returns>
    internal static bool IsSupportedPeer(ITypeSymbol? type) {
        var namespaceName = type?.ContainingNamespace?.ToDisplayString();
        return namespaceName is SHOULDLY_NAMESPACE or NFLUENT_NAMESPACE;
    }

    /// <summary>Maps a supported peer assertion invocation to its fluent replacement.</summary>
    /// <param name="semanticModel">The semantic model for the invocation.</param>
    /// <param name="invocation">The assertion call.</param>
    /// <param name="method">The bound assertion method.</param>
    /// <param name="cancellationToken">Cancels the analysis.</param>
    /// <returns>The replacement, or <see langword="null"/> when no safe conversion exists.</returns>
    internal static ShouldMapping.Replacement? TryMap(
        SemanticModel semanticModel,
        InvocationExpressionSyntax invocation,
        IMethodSymbol method,
        CancellationToken cancellationToken) {
        var namespaceName = method.ContainingType.ContainingNamespace?.ToDisplayString();
        return namespaceName switch {
            SHOULDLY_NAMESPACE => TryMapShouldly(semanticModel, invocation, method),
            NFLUENT_NAMESPACE => TryMapNfluent(semanticModel, invocation, method, cancellationToken),
            _ => null,
        };
    }

    /// <summary>Maps a simple Shouldly extension call.</summary>
    /// <param name="semanticModel">The semantic model for the invocation.</param>
    /// <param name="invocation">The assertion call.</param>
    /// <param name="method">The bound Shouldly method.</param>
    /// <returns>The replacement, or <see langword="null"/> when the shape is not safely convertible.</returns>
    private static ShouldMapping.Replacement? TryMapShouldly(SemanticModel semanticModel, InvocationExpressionSyntax invocation, IMethodSymbol method) {
        if(invocation.Expression is not MemberAccessExpressionSyntax receiver) {
            return null;
        }

        var actual = SyntaxFactory.Argument(receiver.Expression);
        var arguments = invocation.ArgumentList.Arguments;
        var name = method.Name;
        if(name is "ShouldBe" or "ShouldNotBe") {
            if(arguments.Count != 1 || actual.Expression is LiteralExpressionSyntax) {
                return null;
            }

            var expected = arguments[0].WithNameColon(null);
            var target = name == "ShouldBe" ? "Be" : "NotBe";
            return TryCreate(semanticModel, invocation, actual, expected, target, UseShouldAnalyzer.SHOULDLY_ID);
        }

        if(name is "ShouldBeNull" or "ShouldNotBeNull" or "ShouldBeTrue" or "ShouldBeFalse") {
            if(arguments.Count != 0) {
                return null;
            }

            var target = name switch {
                "ShouldBeNull" => "BeNull",
                "ShouldNotBeNull" => "NotBeNull",
                "ShouldBeTrue" => "BeTrue",
                _ => "BeFalse",
            };
            return TryCreate(semanticModel, invocation, actual, null, target, UseShouldAnalyzer.SHOULDLY_ID);
        }

        if(name is "ShouldContain" or "ShouldNotContain") {
            if(arguments.Count != 1 || actual.Expression is LiteralExpressionSyntax) {
                return null;
            }

            var target = name == "ShouldContain" ? "Contain" : "NotContain";
            return TryCreate(semanticModel, invocation, actual, arguments[0].WithNameColon(null), target, UseShouldAnalyzer.SHOULDLY_ID);
        }

        if(name is "ShouldBeEmpty" or "ShouldNotBeEmpty") {
            if(arguments.Count != 0) {
                return null;
            }

            var target = name == "ShouldBeEmpty" ? "BeEmpty" : "NotBeEmpty";
            return TryCreate(semanticModel, invocation, actual, null, target, UseShouldAnalyzer.SHOULDLY_ID);
        }

        if(name == "ShouldThrow") {
            if(arguments.Count != 0 || method.TypeArguments.Length != 1 || !ShouldMapping.IsResultDiscarded(invocation)) {
                return null;
            }

            var typeArgument = TypeSyntaxFor(semanticModel, invocation, method.TypeArguments[0]);
            return TryCreate(semanticModel, invocation, actual, null, "Throw", UseShouldAnalyzer.SHOULDLY_ID, typeArgument);
        }

        if(name == "ShouldNotThrow") {
            if(arguments.Count != 0 || !ShouldMapping.IsResultDiscarded(invocation)) {
                return null;
            }

            return TryCreate(semanticModel, invocation, actual, null, "NotThrow", UseShouldAnalyzer.SHOULDLY_ID);
        }

        return null;
    }

    /// <summary>Maps a simple NFluent <c>Check.That</c> chain.</summary>
    /// <param name="semanticModel">The semantic model for the invocation.</param>
    /// <param name="invocation">The assertion call.</param>
    /// <param name="method">The bound NFluent method.</param>
    /// <param name="cancellationToken">Cancels the analysis.</param>
    /// <returns>The replacement, or <see langword="null"/> when the chain is not a supported statement.</returns>
    private static ShouldMapping.Replacement? TryMapNfluent(
        SemanticModel semanticModel,
        InvocationExpressionSyntax invocation,
        IMethodSymbol method,
        CancellationToken cancellationToken) {
        if(!ShouldMapping.IsResultDiscarded(invocation)
            || !TryGetCheckSubject(semanticModel, invocation, cancellationToken, out var subject, out var isCode)) {
            return null;
        }

        var arguments = invocation.ArgumentList.Arguments;
        var name = method.Name;
        if(name is "IsEqualTo" or "IsNotEqualTo") {
            if(arguments.Count != 1) {
                return null;
            }

            var target = name == "IsEqualTo" ? "Be" : "NotBe";
            return TryCreate(semanticModel, invocation, subject, arguments[0].WithNameColon(null), target, UseShouldAnalyzer.NFLUENT_ID);
        }

        if(name is "IsNull" or "IsNotNull" or "IsTrue" or "IsFalse") {
            if(arguments.Count != 0) {
                return null;
            }

            var target = name switch {
                "IsNull" => "BeNull",
                "IsNotNull" => "NotBeNull",
                "IsTrue" => "BeTrue",
                _ => "BeFalse",
            };
            return TryCreate(semanticModel, invocation, subject, null, target, UseShouldAnalyzer.NFLUENT_ID);
        }

        if(name is "Contains" or "IsEmpty" or "IsNotEmpty") {
            if(name != "Contains" && arguments.Count != 0) {
                return null;
            }

            if(name == "Contains" && arguments.Count != 1) {
                return null;
            }

            var target = name switch {
                "Contains" => "Contain",
                "IsEmpty" => "BeEmpty",
                _ => "NotBeEmpty",
            };
            var expected = name == "Contains" ? arguments[0].WithNameColon(null) : null;
            return TryCreate(semanticModel, invocation, subject, expected, target, UseShouldAnalyzer.NFLUENT_ID);
        }

        if(name == "HasSize") {
            if(arguments.Count != 1) {
                return null;
            }

            return TryCreate(semanticModel, invocation, subject, arguments[0].WithNameColon(null), "HaveCount", UseShouldAnalyzer.NFLUENT_ID);
        }

        if(name == "Throws") {
            if(!isCode || arguments.Count != 0 || method.TypeArguments.Length != 1) {
                return null;
            }

            var typeArgument = TypeSyntaxFor(semanticModel, invocation, method.TypeArguments[0]);
            return TryCreate(semanticModel, invocation, subject, null, "Throw", UseShouldAnalyzer.NFLUENT_ID, typeArgument);
        }

        return null;
    }

    /// <summary>Extracts the subject expression from a <c>Check.That(x)</c>/<c>Check.ThatCode(code)</c> receiver.</summary>
    /// <param name="semanticModel">The semantic model for the invocation.</param>
    /// <param name="invocation">The assertion call.</param>
    /// <param name="cancellationToken">Cancels the analysis.</param>
    /// <param name="subject">The subject argument.</param>
    /// <param name="isCode"><see langword="true"/> for <c>Check.ThatCode</c>.</param>
    /// <returns><see langword="true"/> when the receiver is a supported NFluent entry point.</returns>
    private static bool TryGetCheckSubject(
        SemanticModel semanticModel,
        InvocationExpressionSyntax invocation,
        CancellationToken cancellationToken,
        out ArgumentSyntax subject,
        out bool isCode) {
        subject = null!;
        isCode = false;
        if(invocation.Expression is not MemberAccessExpressionSyntax { Expression: InvocationExpressionSyntax checkCall }
            || checkCall.Expression is not MemberAccessExpressionSyntax checkAccess
            || checkAccess.Name.Identifier.Text is not ("That" or "ThatCode")
            || checkCall.ArgumentList.Arguments.Count != 1
            || semanticModel.GetSymbolInfo(checkCall, cancellationToken).Symbol is not IMethodSymbol checkMethod
            || checkMethod.ContainingType.ToDisplayString() != "NFluent.Check") {
            return false;
        }

        subject = checkCall.ArgumentList.Arguments[0];
        isCode = checkAccess.Name.Identifier.Text == "ThatCode";
        return true;
    }

    /// <summary>Builds and verifies a replacement only when the speculative fluent call binds.</summary>
    /// <param name="semanticModel">The semantic model for the invocation.</param>
    /// <param name="invocation">The original assertion call.</param>
    /// <param name="actual">The subject argument.</param>
    /// <param name="expected">The expected value argument, when any.</param>
    /// <param name="target">The fluent method name.</param>
    /// <param name="diagnosticId">The peer diagnostic ID.</param>
    /// <param name="typeArgument">The generic argument for exception assertions.</param>
    /// <returns>The replacement, or <see langword="null"/> when the fluent call does not bind.</returns>
    private static ShouldMapping.Replacement? TryCreate(
        SemanticModel semanticModel,
        InvocationExpressionSyntax invocation,
        ArgumentSyntax actual,
        ArgumentSyntax? expected,
        string target,
        string diagnosticId,
        TypeSyntax? typeArgument = null) {
        if(ShouldMapping.IsProvenUncompilable(semanticModel, invocation, actual.Expression, target, expected?.Expression, typeArgument)) {
            return null;
        }

        return new ShouldMapping.Replacement(target, actual.Expression, expected, null, diagnosticId, typeArgument);
    }

    /// <summary>Converts a bound type symbol into the minimal syntax usable at the assertion site.</summary>
    /// <param name="semanticModel">The semantic model for the invocation.</param>
    /// <param name="invocation">The assertion call.</param>
    /// <param name="type">The type to render.</param>
    /// <returns>The minimal type syntax.</returns>
    private static TypeSyntax TypeSyntaxFor(SemanticModel semanticModel, InvocationExpressionSyntax invocation, ITypeSymbol type) =>
        SyntaxFactory.ParseTypeName(type.ToMinimalDisplayString(semanticModel, invocation.SpanStart));
}

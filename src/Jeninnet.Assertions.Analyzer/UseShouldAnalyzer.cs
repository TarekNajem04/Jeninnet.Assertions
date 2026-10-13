//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Analyzer;

/// <summary>Suggests fluent <c>Should()</c> assertions over convertible framework <c>Assert</c> calls.</summary>
/// <remarks>Recognizes MSTest and NUnit classic equality/condition calls, xUnit <c>Assert.Equal</c>-family
/// calls, xUnit <c>Assert.Contains</c>, and simple NUnit <c>Assert.That(actual, Is.X)</c> constraints.</remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseShouldAnalyzer : DiagnosticAnalyzer {
    /// <summary>Diagnostic ID for equality conversions (<c>Assert.AreEqual</c>, xUnit <c>Assert.Equal</c>, <c>Is.EqualTo</c>).</summary>
    /// <remarks>The <c>JNA</c> prefix stands for Jeninnet Assertions.</remarks>
    public const string ARE_EQUAL_ID = "JNA1001";

    /// <summary>Diagnostic ID for condition conversions (<c>Assert.IsTrue</c>, xUnit <c>Assert.True</c>, <c>Is.Null</c>).</summary>
    /// <remarks>The <c>JNA</c> prefix stands for Jeninnet Assertions.</remarks>
    public const string IS_CONDITION_ID = "JNA1002";

    private static readonly DiagnosticDescriptor _areEqualRule = new(
        ARE_EQUAL_ID,
        "Use Should().Be() instead of classic equality assertions",
        "Use '{0}' instead of 'Assert.{1}'",
        "Testing",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Fluent Should() assertions produce richer failure messages than framework equality assertions.");

    private static readonly DiagnosticDescriptor _isConditionRule = new(
        IS_CONDITION_ID,
        "Use Should() assertion instead of classic condition assertions",
        "Use '{0}' instead of 'Assert.{1}'",
        "Testing",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Fluent Should() assertions produce richer failure messages than framework condition assertions.");

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [_areEqualRule, _isConditionRule];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context) {
        if(context is null) {
            throw new ArgumentNullException(nameof(context));
        }

        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    /// <summary>Reports a diagnostic when an invocation is a supported assertion with a safe fluent replacement.</summary>
    /// <param name="context">The syntax-node analysis context for the invocation.</param>
    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context) {
        var invocation = (InvocationExpressionSyntax)context.Node;
        var methodName = invocation.Expression switch {
            MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.Text,
            IdentifierNameSyntax identifier => identifier.Identifier.Text,
            _ => null,
        };
        if(methodName is null) {
            return;
        }

        if(context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is not IMethodSymbol method
            || !ShouldMapping.IsSupportedAssert(method.ContainingType)) {
            return;
        }

        var replacement = Map(context.SemanticModel, invocation, method, methodName, context.CancellationToken, out var rule);
        if(replacement is not null) {
            context.ReportDiagnostic(Diagnostic.Create(rule, invocation.GetLocation(), replacement.TargetMethod, methodName));
        }
    }

    /// <summary>Routes a supported assertion invocation to the matching mapping and diagnostic rule.</summary>
    /// <param name="semanticModel">The semantic model for the invocation.</param>
    /// <param name="invocation">The assertion call.</param>
    /// <param name="method">The bound assertion method.</param>
    /// <param name="methodName">The simple method name used to pick the mapping.</param>
    /// <param name="cancellationToken">Cancels the analysis.</param>
    /// <param name="rule">The diagnostic rule to report when a replacement is produced.</param>
    /// <returns>The planned replacement, or <see langword="null"/> when no safe conversion exists.</returns>
    private static ShouldMapping.Replacement? Map(SemanticModel semanticModel, InvocationExpressionSyntax invocation, IMethodSymbol method, string methodName, CancellationToken cancellationToken, out DiagnosticDescriptor rule) {
        if(ShouldMapping.IsEqualityMethod(methodName)) {
            rule = _areEqualRule;
            return ShouldMapping.TryMapEquality(semanticModel, invocation, method, cancellationToken);
        }

        if(ShouldMapping.IsConditionMethod(methodName)) {
            rule = _isConditionRule;
            return ShouldMapping.TryMapCondition(semanticModel, invocation, method, cancellationToken);
        }

        if(methodName == "Contains") {
            rule = _areEqualRule;
            return ShouldMapping.TryMapContains(semanticModel, invocation, method, cancellationToken);
        }

        if(methodName == "That") {
            var replacement = ShouldMapping.TryMapConstraint(semanticModel, invocation, method, cancellationToken);
            rule = replacement?.TargetMethod is "Be" or "NotBe" or "Contain" ? _areEqualRule : _isConditionRule;
            return replacement;
        }

        rule = _areEqualRule;
        return null;
    }
}

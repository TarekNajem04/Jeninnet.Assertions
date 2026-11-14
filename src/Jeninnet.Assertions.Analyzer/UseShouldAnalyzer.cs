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

    /// <summary>Diagnostic ID for string conversions (<c>StringAssert.Contains</c>, xUnit <c>Assert.StartsWith</c>).</summary>
    /// <remarks>The <c>JNA</c> prefix stands for Jeninnet Assertions.</remarks>
    public const string STRING_ASSERTION_ID = "JNA1003";

    /// <summary>Diagnostic ID for collection conversions (<c>Assert.Empty</c>, <c>CollectionAssert.Contains</c>).</summary>
    /// <remarks>The <c>JNA</c> prefix stands for Jeninnet Assertions.</remarks>
    public const string COLLECTION_ASSERTION_ID = "JNA1004";

    /// <summary>Diagnostic ID for exception conversions (<c>Assert.ThrowsException</c>, <c>Assert.Throws</c>).</summary>
    /// <remarks>The <c>JNA</c> prefix stands for Jeninnet Assertions.</remarks>
    public const string EXCEPTION_ASSERTION_ID = "JNA1005";

    /// <summary>Diagnostic ID for type conversions (<c>Assert.IsInstanceOfType</c>, <c>Assert.IsAssignableFrom</c>).</summary>
    /// <remarks>The <c>JNA</c> prefix stands for Jeninnet Assertions.</remarks>
    public const string TYPE_ASSERTION_ID = "JNA1006";

    /// <summary>Diagnostic ID for task-state conversions (<c>Assert.True(task.IsCompleted)</c>).</summary>
    /// <remarks>The <c>JNA</c> prefix stands for Jeninnet Assertions.</remarks>
    public const string TASK_STATE_ID = "JNA1007";

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

    private static readonly DiagnosticDescriptor _stringRule = new(
        STRING_ASSERTION_ID,
        "Use Should() string assertions",
        "Use '{0}' instead of 'Assert.{1}'",
        "Testing",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Fluent Should() string assertions produce richer failure messages than framework string assertions.");

    private static readonly DiagnosticDescriptor _collectionRule = new(
        COLLECTION_ASSERTION_ID,
        "Use Should() collection assertions",
        "Use '{0}' instead of 'Assert.{1}'",
        "Testing",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Fluent Should() collection assertions produce richer failure messages than framework collection assertions.");

    private static readonly DiagnosticDescriptor _exceptionRule = new(
        EXCEPTION_ASSERTION_ID,
        "Use Should().Throw() instead of classic exception assertions",
        "Use '{0}' instead of 'Assert.{1}'",
        "Testing",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Fluent Should().Throw() assertions expose the caught exception and richer failure messages.");

    private static readonly DiagnosticDescriptor _typeRule = new(
        TYPE_ASSERTION_ID,
        "Use Should() type assertions",
        "Use '{0}' instead of 'Assert.{1}'",
        "Testing",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Fluent Should() type assertions produce richer failure messages than framework type assertions.");

    private static readonly DiagnosticDescriptor _taskRule = new(
        TASK_STATE_ID,
        "Use Should() task-state assertions",
        "Use '{0}' instead of 'Assert.{1}'",
        "Testing",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Fluent task-state assertions name the expected state directly and produce richer failure messages.");

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [_areEqualRule, _isConditionRule, _stringRule, _collectionRule, _exceptionRule, _typeRule, _taskRule];

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

        var replacement = Map(context.SemanticModel, invocation, method, context.CancellationToken, out var rule);
        if(replacement is not null) {
            context.ReportDiagnostic(Diagnostic.Create(rule, invocation.GetLocation(), replacement.TargetMethod, methodName));
        }
    }

    /// <summary>Routes a supported assertion invocation to the matching mapping and diagnostic rule.</summary>
    /// <param name="semanticModel">The semantic model for the invocation.</param>
    /// <param name="invocation">The assertion call.</param>
    /// <param name="method">The bound assertion method.</param>
    /// <param name="cancellationToken">Cancels the analysis.</param>
    /// <param name="rule">The diagnostic rule to report when a replacement is produced.</param>
    /// <returns>The planned replacement, or <see langword="null"/> when no safe conversion exists.</returns>
    private static ShouldMapping.Replacement? Map(SemanticModel semanticModel, InvocationExpressionSyntax invocation, IMethodSymbol method, CancellationToken cancellationToken, out DiagnosticDescriptor rule) {
        var replacement = ShouldMapping.TryMap(semanticModel, invocation, method, cancellationToken);
        rule = RuleFor(replacement?.DiagnosticId ?? ARE_EQUAL_ID);
        return replacement;
    }

    /// <summary>Resolves the diagnostic descriptor for a replacement rule ID.</summary>
    /// <param name="diagnosticId">One of the <c>JNA10xx</c> IDs.</param>
    /// <returns>The matching descriptor, defaulting to the equality rule for unknown IDs.</returns>
    private static DiagnosticDescriptor RuleFor(string diagnosticId) =>
        diagnosticId switch {
            STRING_ASSERTION_ID => _stringRule,
            COLLECTION_ASSERTION_ID => _collectionRule,
            EXCEPTION_ASSERTION_ID => _exceptionRule,
            TYPE_ASSERTION_ID => _typeRule,
            TASK_STATE_ID => _taskRule,
            IS_CONDITION_ID => _isConditionRule,
            _ => _areEqualRule,
        };
}

//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Analyzer;

/// <summary>Rewrites convertible framework <c>Assert</c> calls (MSTest, xUnit, NUnit) to fluent <c>Should()</c> calls.</summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseShouldCodeFixProvider)), Shared]
public sealed class UseShouldCodeFixProvider : CodeFixProvider {
    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds => [
        UseShouldAnalyzer.ARE_EQUAL_ID,
        UseShouldAnalyzer.IS_CONDITION_ID,
        UseShouldAnalyzer.STRING_ASSERTION_ID,
        UseShouldAnalyzer.COLLECTION_ASSERTION_ID,
        UseShouldAnalyzer.EXCEPTION_ASSERTION_ID,
        UseShouldAnalyzer.TYPE_ASSERTION_ID,
        UseShouldAnalyzer.TASK_STATE_ID,
    ];

    /// <inheritdoc />
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc />
    public override async Task RegisterCodeFixesAsync(CodeFixContext context) {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
        if(root?.FindNode(context.Span).FirstAncestorOrSelf<InvocationExpressionSyntax>() is not InvocationExpressionSyntax invocation) {
            return;
        }

        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);
        if(semanticModel?.GetSymbolInfo(invocation, context.CancellationToken).Symbol is not IMethodSymbol method
            || !ShouldMapping.IsSupportedAssert(method.ContainingType)) {
            return;
        }

        var replacement = ShouldMapping.TryMap(semanticModel, invocation, method, context.CancellationToken);
        if(replacement is null) {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                $"Convert to '{replacement.TargetMethod}'",
                cancellationToken => ConvertAsync(context.Document, root, invocation, replacement, cancellationToken),
                nameof(UseShouldCodeFixProvider)),
            context.Diagnostics);
    }

    /// <summary>Builds the fluent <c>Should()</c> call, renames carried arguments, and ensures the using directive.</summary>
    /// <param name="document">The document containing the assertion.</param>
    /// <param name="root">The current syntax root of the document.</param>
    /// <param name="invocation">The assertion call being replaced.</param>
    /// <param name="replacement">The planned fluent replacement.</param>
    /// <param name="cancellationToken">Accepted for the code-fix contract; the rewrite is synchronous.</param>
    /// <returns>A task producing the document with the converted assertion.</returns>
    private static Task<Document> ConvertAsync(
        Document document,
        SyntaxNode root,
        InvocationExpressionSyntax invocation,
        ShouldMapping.Replacement replacement,
        CancellationToken cancellationToken) {
        _ = cancellationToken;
        var receiver = AsReceiver(replacement.Actual, replacement.RequiresAwait);
        var receiverAccess = SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            receiver,
            SyntaxFactory.IdentifierName("Should"));
        var shouldCall = SyntaxFactory.InvocationExpression(receiverAccess);
        var arguments = new List<ArgumentSyntax>();
        if(replacement.Expected is not null) {
            arguments.Add(RenameArgument(replacement.Expected, "notExpected", "unexpected"));
        }

        if(replacement.Because is not null) {
            arguments.Add(RenameArgument(replacement.Because, "message", "because"));
        }

        var fluentCall = SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, shouldCall, MethodName(replacement)),
            SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));
        ExpressionSyntax converted = replacement.RequiresAwait ? SyntaxFactory.AwaitExpression(fluentCall) : fluentCall;
        var replacedNode = replacement.RequiresAwait && invocation.Parent is AwaitExpressionSyntax awaited
            ? (SyntaxNode)awaited
            : invocation;
        var replaced = root.ReplaceNode(
            replacedNode,
            converted.WithLeadingTrivia(replacedNode.GetLeadingTrivia()).WithTrailingTrivia(replacedNode.GetTrailingTrivia()));
        return Task.FromResult(document.WithSyntaxRoot(EnsureUsing(replaced, "Jeninnet.Assertions")));
    }

    /// <summary>Builds the <c>Should()</c> receiver, wrapping a bare lambda in an explicit delegate creation.</summary>
    /// <param name="actual">The expression becoming the subject.</param>
    /// <param name="requiresAwait">Whether the target is an async assertion (uses <c>Func&lt;Task&gt;</c>).</param>
    /// <returns>The receiver expression.</returns>
    private static ExpressionSyntax AsReceiver(ExpressionSyntax actual, bool requiresAwait) {
        var expression = actual.WithoutTrivia();
        if(expression is not (LambdaExpressionSyntax or AnonymousMethodExpressionSyntax)) {
            return expression;
        }

        var delegateType = requiresAwait ? "System.Func<System.Threading.Tasks.Task>" : "System.Action";
        return SyntaxFactory.ObjectCreationExpression(SyntaxFactory.ParseTypeName(delegateType))
            .WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(expression))));
    }

    /// <summary>Builds the target method name, applying a generic type argument when the mapping requires one.</summary>
    /// <param name="replacement">The planned fluent replacement.</param>
    /// <returns>A plain or generic method name.</returns>
    private static SimpleNameSyntax MethodName(ShouldMapping.Replacement replacement) =>
        replacement.TypeArgument is null
            ? SyntaxFactory.IdentifierName(replacement.TargetMethod)
            : SyntaxFactory.GenericName(SyntaxFactory.Identifier(replacement.TargetMethod))
                .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(replacement.TypeArgument)));

    /// <summary>Renames a carried-over named argument (e.g. <c>message:</c> to <c>because:</c>).</summary>
    /// <param name="argument">The argument to inspect.</param>
    /// <param name="from">The original parameter name.</param>
    /// <param name="to">The fluent parameter name.</param>
    /// <returns>The argument with the new name, or unchanged when it does not use <paramref name="from"/>.</returns>
    private static ArgumentSyntax RenameArgument(ArgumentSyntax argument, string from, string to) {
        if(argument.NameColon?.Name.Identifier.Text != from) {
            return argument;
        }

        return argument.WithNameColon(argument.NameColon.WithName(SyntaxFactory.IdentifierName(to)));
    }

    /// <summary>Ensures the compilation unit imports <paramref name="namespaceName"/>, preserving its line endings.</summary>
    /// <param name="root">The rewritten syntax root.</param>
    /// <param name="namespaceName">The namespace the converted call requires.</param>
    /// <returns>The root with the using directive present, or the original root when it is not a compilation unit.</returns>
    private static SyntaxNode EnsureUsing(SyntaxNode root, string namespaceName) {
        if(root is not CompilationUnitSyntax compilationUnit) {
            return root;
        }

        foreach(var directive in compilationUnit.Usings) {
            if(directive.Name?.ToString() == namespaceName) {
                return root;
            }
        }

        var endOfLine = root.ToFullString().Contains("\r\n") ? "\r\n" : "\n";
        if(compilationUnit.Usings.Count == 0) {
            var firstToken = compilationUnit.GetFirstToken();
            var directive = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(namespaceName))
                .WithLeadingTrivia(firstToken.LeadingTrivia.Add(SyntaxFactory.EndOfLine(endOfLine)))
                .WithTrailingTrivia(SyntaxFactory.EndOfLine(endOfLine), SyntaxFactory.EndOfLine(endOfLine));
            var stripped = compilationUnit.ReplaceToken(firstToken, firstToken.WithLeadingTrivia());
            return stripped is CompilationUnitSyntax withoutLeading
                ? withoutLeading.WithUsings(SyntaxFactory.SingletonList(directive))
                : stripped;
        }

        var appended = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(namespaceName))
            .WithTrailingTrivia(SyntaxFactory.EndOfLine(endOfLine));
        return compilationUnit.WithUsings(compilationUnit.Usings.Add(appended));
    }
}

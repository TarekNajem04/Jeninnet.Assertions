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
        /// <param name="targetMethod">The fluent method name (<c>Be</c>, <c>NotBe</c>, <c>BeTrue</c>, <c>BeFalse</c>,
        /// <c>BeNull</c>, <c>NotBeNull</c>, <c>Contain</c>, <c>StartWith</c>, <c>EndWith</c>, <c>BeEmpty</c>,
        /// <c>NotBeEmpty</c>, <c>ContainSingle</c>, <c>Throw</c>, <c>ThrowAsync</c>, <c>BeOfType</c>,
        /// <c>BeAssignableTo</c>, <c>BeCompleted</c>, or <c>NotBeCompleted</c>).</param>
        /// <param name="actual">The expression becoming the <c>Should()</c> receiver.</param>
        /// <param name="expected">The value argument (<see langword="null"/> when the target takes none).</param>
        /// <param name="because">The reason argument (<see langword="null"/> when absent).</param>
        /// <param name="diagnosticId">The diagnostic rule that justifies the replacement.</param>
        /// <param name="typeArgument">The generic type argument for verbs such as <c>BeOfType&lt;T&gt;</c>.</param>
        /// <param name="requiresAwait">Whether the replacement must be awaited (async exception assertions).</param>
        internal Replacement(
            string targetMethod,
            ExpressionSyntax actual,
            ArgumentSyntax? expected,
            ArgumentSyntax? because,
            string diagnosticId = UseShouldAnalyzer.ARE_EQUAL_ID,
            TypeSyntax? typeArgument = null,
            bool requiresAwait = false) {
            TargetMethod = targetMethod;
            Actual = actual;
            Expected = expected;
            Because = because;
            DiagnosticId = diagnosticId;
            TypeArgument = typeArgument;
            RequiresAwait = requiresAwait;
        }

        /// <summary>Gets the fluent method name.</summary>
        internal string TargetMethod { get; }

        /// <summary>Gets the expression becoming the <c>Should()</c> receiver.</summary>
        internal ExpressionSyntax Actual { get; }

        /// <summary>Gets the value argument (<see langword="null"/> when the target takes none).</summary>
        internal ArgumentSyntax? Expected { get; }

        /// <summary>Gets the reason argument (<see langword="null"/> when absent).</summary>
        internal ArgumentSyntax? Because { get; }

        /// <summary>Gets the diagnostic rule that justifies the replacement.</summary>
        internal string DiagnosticId { get; }

        /// <summary>Gets the generic type argument for verbs such as <c>BeOfType&lt;T&gt;</c>.</summary>
        internal TypeSyntax? TypeArgument { get; }

        /// <summary>Gets whether the replacement must be awaited.</summary>
        internal bool RequiresAwait { get; }
    }

    /// <summary>Determines whether <paramref name="type"/> is a supported assertion entry point.</summary>
    /// <param name="type">The containing type of a candidate method.</param>
    /// <returns><see langword="true"/> for MSTest, xUnit, or NUnit <c>Assert</c>/<c>StringAssert</c>/<c>CollectionAssert</c>.</returns>
    internal static bool IsSupportedAssert(ITypeSymbol? type) {
        if(type is not { } named) {
            return false;
        }

        var namespaceName = named.ContainingNamespace?.ToDisplayString();
        if(namespaceName is not (MSTEST_ASSERT_NAMESPACE or XUNIT_ASSERT_NAMESPACE or NUNIT_ASSERT_NAMESPACE)) {
            return false;
        }

        return named.Name is "Assert" or "StringAssert" or "CollectionAssert";
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

    /// <summary>Maps a supported assertion invocation to its fluent replacement.</summary>
    /// <remarks>This is the single dispatch point shared by the analyzer and the code fix, so they can never disagree.</remarks>
    /// <param name="semanticModel">The semantic model for the invocation.</param>
    /// <param name="invocation">The assertion call.</param>
    /// <param name="method">The bound assertion method.</param>
    /// <param name="cancellationToken">Cancels the analysis.</param>
    /// <returns>The replacement, or <see langword="null"/> when no safe conversion exists.</returns>
    internal static Replacement? TryMap(SemanticModel semanticModel, InvocationExpressionSyntax invocation, IMethodSymbol method, CancellationToken cancellationToken) {
        var name = method.Name;
        if(IsEqualityMethod(name)) {
            return TryMapEquality(semanticModel, invocation, method, cancellationToken);
        }

        if(IsConditionMethod(name)) {
            return TryMapCondition(semanticModel, invocation, method, cancellationToken);
        }

        if(name is "StartsWith" or "EndsWith") {
            return TryMapString(semanticModel, invocation, method, cancellationToken);
        }

        if(name == "Contains") {
            return TryMapContains(semanticModel, invocation, method, cancellationToken);
        }

        if(name is "Empty" or "NotEmpty" or "Single" or "IsEmpty" or "IsNotEmpty" or "DoesNotContain") {
            return TryMapCollection(semanticModel, invocation, method, cancellationToken);
        }

        if(name is "Throws" or "ThrowsAsync" or "ThrowsException" or "ThrowsExceptionAsync") {
            return TryMapException(semanticModel, invocation, method, cancellationToken);
        }

        if(name is "IsInstanceOfType" or "IsNotInstanceOfType" or "IsExactInstanceOfType" or "IsNotExactInstanceOfType"
            or "IsType" or "IsNotType" or "IsAssignableFrom" or "IsInstanceOf" or "IsNotInstanceOf") {
            return TryMapType(semanticModel, invocation, method, cancellationToken);
        }

        if(name == "That") {
            return TryMapConstraint(semanticModel, invocation, method, cancellationToken);
        }

        return null;
    }

    /// <summary>Maps a string <c>StringAssert</c>/xUnit prefix or suffix assertion to its fluent replacement.</summary>
    /// <param name="semanticModel">The semantic model for the invocation.</param>
    /// <param name="invocation">The assertion call.</param>
    /// <param name="method">The bound assertion method.</param>
    /// <param name="cancellationToken">Cancels the analysis.</param>
    /// <returns>The replacement, or <see langword="null"/> when either operand is not a string or an argument is unsafe to drop.</returns>
    internal static Replacement? TryMapString(SemanticModel semanticModel, InvocationExpressionSyntax invocation, IMethodSymbol method, CancellationToken cancellationToken) {
        var arguments = invocation.ArgumentList.Arguments;
        if(arguments.Count is not (2 or 3)) {
            return null;
        }

        var isStringAssert = method.ContainingType.Name == "StringAssert";
        var first = arguments[0];
        var second = arguments[1];
        var actual = isStringAssert ? first : second;
        var expected = (isStringAssert ? second : first).WithNameColon(null);
        var because = NamedArgument(arguments, "message") ?? (arguments.Count == 3 ? arguments[2] : null);
        if(IsDelegateReason(because) || !IsStringArgument(because, semanticModel, cancellationToken)) {
            return null;
        }

        var actualType = semanticModel.GetTypeInfo(actual.Expression, cancellationToken).Type;
        var expectedType = semanticModel.GetTypeInfo(expected.Expression, cancellationToken).Type;
        if(actualType?.SpecialType != SpecialType.System_String || expectedType?.SpecialType != SpecialType.System_String) {
            return null;
        }

        var target = method.Name == "StartsWith" ? "StartWith" : "EndWith";
        if(IsProvenUncompilable(semanticModel, invocation, actual.Expression, target, expected.Expression)) {
            return null;
        }

        return new Replacement(target, actual.Expression, expected, because, UseShouldAnalyzer.STRING_ASSERTION_ID);
    }

    /// <summary>Maps a collection emptiness/single/containment assertion to its fluent replacement.</summary>
    /// <param name="semanticModel">The semantic model for the invocation.</param>
    /// <param name="invocation">The assertion call.</param>
    /// <param name="method">The bound assertion method.</param>
    /// <param name="cancellationToken">Cancels the analysis.</param>
    /// <returns>The replacement, or <see langword="null"/> when the subject is not a generic collection or the result would change.</returns>
    internal static Replacement? TryMapCollection(SemanticModel semanticModel, InvocationExpressionSyntax invocation, IMethodSymbol method, CancellationToken cancellationToken) {
        var arguments = invocation.ArgumentList.Arguments;
        if(arguments.Count is < 1 or > 3) {
            return null;
        }

        var isCollectionAssert = method.ContainingType.Name == "CollectionAssert";
        var isContainment = method.Name is "Contains" or "DoesNotContain";
        if((!isContainment && arguments.Count > 2) || (method.Name == "Single" && arguments.Count != 1)) {
            return null;
        }

        var actual = arguments[0];
        ArgumentSyntax? expected = null;
        if(isContainment) {
            actual = isCollectionAssert ? arguments[0] : arguments[1];
            expected = (isCollectionAssert ? arguments[1] : arguments[0]).WithNameColon(null);
        }

        var becauseIndex = isContainment ? 2 : 1;
        var because = NamedArgument(arguments, "message")
            ?? (arguments.Count > becauseIndex ? arguments[arguments.Count - 1] : null);
        if(IsDelegateReason(because) || !IsStringArgument(because, semanticModel, cancellationToken)) {
            return null;
        }

        var actualType = semanticModel.GetTypeInfo(actual.Expression, cancellationToken).Type;
        if(actual.Expression is LiteralExpressionSyntax || actualType is null || !IsGenericCollection(actualType)) {
            return null;
        }

        var target = method.Name switch {
            "Empty" or "IsEmpty" => "BeEmpty",
            "NotEmpty" or "IsNotEmpty" => "NotBeEmpty",
            "Single" => "ContainSingle",
            "DoesNotContain" => "NotContain",
            _ => "Contain",
        };

        if(!method.ReturnsVoid && !IsResultDiscarded(invocation)) {
            return null;
        }

        var expectedExpression = expected?.Expression;
        if(IsProvenUncompilable(semanticModel, invocation, actual.Expression, target, expectedExpression)) {
            return null;
        }

        return new Replacement(target, actual.Expression, expected, because, UseShouldAnalyzer.COLLECTION_ASSERTION_ID);
    }

    /// <summary>Maps a framework exception assertion to <c>Should().Throw&lt;T&gt;()</c> or <c>ThrowAsync&lt;T&gt;()</c>.</summary>
    /// <remarks>Only statement-position calls are converted: framework assertions return the caught exception, while the
    /// fluent form returns an assertion object, so using the result would change meaning.</remarks>
    /// <param name="semanticModel">The semantic model for the invocation.</param>
    /// <param name="invocation">The assertion call.</param>
    /// <param name="method">The bound assertion method.</param>
    /// <param name="cancellationToken">Cancels the analysis.</param>
    /// <returns>The replacement, or <see langword="null"/> when the call is not statement-position or the exception type is unknown.</returns>
    internal static Replacement? TryMapException(SemanticModel semanticModel, InvocationExpressionSyntax invocation, IMethodSymbol method, CancellationToken cancellationToken) {
        _ = cancellationToken;
        var arguments = invocation.ArgumentList.Arguments;
        if(arguments.Count is < 1 or > 2 || method.TypeArguments.Length == 0) {
            return null;
        }

        var exceptionType = method.TypeArguments[0];
        if(exceptionType.ToDisplayString() == "System.Exception") {
            return null;
        }

        var isAsync = method.Name is "ThrowsAsync" or "ThrowsExceptionAsync";
        if(isAsync) {
            if(invocation.Parent is not AwaitExpressionSyntax { Parent: ExpressionStatementSyntax }) {
                return null;
            }
        } else if(!IsResultDiscarded(invocation)) {
            return null;
        }

        var because = NamedArgument(arguments, "message") ?? (arguments.Count == 2 ? arguments[1] : null);
        if(IsDelegateReason(because) || !IsStringArgument(because, semanticModel, cancellationToken)) {
            return null;
        }

        var isMstest = method.ContainingType.ContainingNamespace?.ToDisplayString() == MSTEST_ASSERT_NAMESPACE;
        var exact = method.Name is "ThrowsException" or "ThrowsExceptionAsync" || !isMstest;
        string target;
        if(exact) {
            target = isAsync ? "ThrowExactlyAsync" : "ThrowExactly";
        } else {
            target = isAsync ? "ThrowAsync" : "Throw";
        }
        var typeSyntax = SyntaxFactory.ParseTypeName(exceptionType.ToMinimalDisplayString(semanticModel, invocation.SpanStart));
        if(IsProvenUncompilable(semanticModel, invocation, arguments[0].Expression, target, null, typeSyntax, isAsync)) {
            return null;
        }

        return new Replacement(target, arguments[0].Expression, null, because, UseShouldAnalyzer.EXCEPTION_ASSERTION_ID, typeSyntax, isAsync);
    }

    /// <summary>Maps a framework type assertion to <c>BeOfType&lt;T&gt;</c> or <c>BeAssignableTo&lt;T&gt;</c>.</summary>
    /// <param name="semanticModel">The semantic model for the invocation.</param>
    /// <param name="invocation">The assertion call.</param>
    /// <param name="method">The bound assertion method.</param>
    /// <param name="cancellationToken">Cancels the analysis.</param>
    /// <returns>The replacement, or <see langword="null"/> when the expected type is not a compile-time type or the result is used.</returns>
    internal static Replacement? TryMapType(SemanticModel semanticModel, InvocationExpressionSyntax invocation, IMethodSymbol method, CancellationToken cancellationToken) {
        var arguments = invocation.ArgumentList.Arguments;
        if(arguments.Count is < 1 or > 3 || (method.TypeArguments.Length == 0 && arguments.Count < 2)) {
            return null;
        }

        ITypeSymbol? expectedType = method.TypeArguments.Length > 0 ? method.TypeArguments[0] : null;
        if(expectedType is null) {
            if(arguments.Count < 2 || arguments[1].Expression is not TypeOfExpressionSyntax typeOf) {
                return null;
            }

            expectedType = semanticModel.GetTypeInfo(typeOf.Type, cancellationToken).Type;
        }

        if(expectedType is null || !IsResultDiscarded(invocation)) {
            return null;
        }

        var isNegated = method.Name is "IsNotInstanceOfType" or "IsNotType" or "IsNotExactInstanceOfType" or "IsNotInstanceOf";
        var isExact = method.Name is "IsType" or "IsNotType" or "IsExactInstanceOfType" or "IsNotExactInstanceOfType";
        string target;
        if(isExact) {
            target = isNegated ? "NotBeOfType" : "BeOfType";
        } else {
            target = isNegated ? "NotBeAssignableTo" : "BeAssignableTo";
        }
        var typeSyntax = SyntaxFactory.ParseTypeName(expectedType.ToMinimalDisplayString(semanticModel, invocation.SpanStart));
        var messageIndex = method.TypeArguments.Length > 0 ? 1 : 2;
        var because = NamedArgument(arguments, "message")
            ?? (arguments.Count > messageIndex ? arguments[arguments.Count - 1] : null);
        if(IsDelegateReason(because) || !IsStringArgument(because, semanticModel, cancellationToken)) {
            return null;
        }

        if(IsProvenUncompilable(semanticModel, invocation, arguments[0].Expression, target, null, typeSyntax)) {
            return null;
        }

        return new Replacement(target, arguments[0].Expression, null, because, UseShouldAnalyzer.TYPE_ASSERTION_ID, typeSyntax);
    }

    /// <summary>Maps <c>Assert.True(task.IsCompleted)</c> to <c>task.Should().BeCompleted()</c>.</summary>
    /// <param name="semanticModel">The semantic model for the invocation.</param>
    /// <param name="invocation">The assertion call.</param>
    /// <param name="method">The bound condition method.</param>
    /// <param name="value">The condition argument.</param>
    /// <param name="because">The carried reason argument.</param>
    /// <param name="cancellationToken">Cancels the analysis.</param>
    /// <returns>The replacement, or <see langword="null"/> when the condition is not a supported task state.</returns>
    private static Replacement? TryMapTaskState(
        SemanticModel semanticModel,
        InvocationExpressionSyntax invocation,
        IMethodSymbol method,
        ArgumentSyntax value,
        ArgumentSyntax? because,
        CancellationToken cancellationToken) {
        if(method.Name is not ("IsTrue" or "True" or "IsFalse" or "False")
            || value.Expression is not MemberAccessExpressionSyntax { Name.Identifier.Text: "IsCompleted" } member) {
            return null;
        }

        var receiverType = semanticModel.GetTypeInfo(member.Expression, cancellationToken).Type;
        if(!IsTaskType(receiverType)) {
            return null;
        }

        var target = method.Name is "IsFalse" or "False" ? "NotBeCompleted" : "BeCompleted";
        if(IsProvenUncompilable(semanticModel, invocation, member.Expression, target, null)) {
            return null;
        }

        return new Replacement(target, member.Expression, null, because, UseShouldAnalyzer.TASK_STATE_ID);
    }

    /// <summary>Determines whether a type is <see cref="Task"/> or <c>Task&lt;T&gt;</c>.</summary>
    /// <param name="type">The type to inspect (may be <see langword="null"/>).</param>
    /// <returns><see langword="true"/> for task types.</returns>
    private static bool IsTaskType(ITypeSymbol? type) {
        for(var current = type; current is not null; current = current.BaseType) {
            if(current.OriginalDefinition.ToDisplayString() is "System.Threading.Tasks.Task" or "System.Threading.Tasks.Task<TResult>") {
                return true;
            }
        }

        return false;
    }

    /// <summary>Determines whether a type is an array or implements <c>IEnumerable&lt;T&gt;</c>, excluding strings.</summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns><see langword="true"/> for generic collections.</returns>
    private static bool IsGenericCollection(ITypeSymbol type) {
        if(type.SpecialType == SpecialType.System_String) {
            return false;
        }

        if(type is IArrayTypeSymbol) {
            return true;
        }

        if(type.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T) {
            return true;
        }

        return type.AllInterfaces.Any(static contract => contract.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T);
    }

    /// <summary>Determines whether an assertion result is discarded by an expression statement (or an awaited statement).</summary>
    /// <param name="invocation">The assertion invocation.</param>
    /// <returns><see langword="true"/> when the caller does not consume the assertion result.</returns>
    internal static bool IsResultDiscarded(InvocationExpressionSyntax invocation) =>
        invocation.Parent is ExpressionStatementSyntax
        || invocation.Parent is AwaitExpressionSyntax { Parent: ExpressionStatementSyntax };

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

        if(TryMapBooleanLiteral(expected, actual, because, !isNegated, UseShouldAnalyzer.ARE_EQUAL_ID, out var boolean)) {
            return boolean;
        }

        if(!IsBeCapable(actualType)) {
            return null;
        }

        if(IsProvenUncompilable(semanticModel, invocation, actual.Expression, target, expected.Expression)) {
            return null;
        }

        return new Replacement(target, actual.Expression, expected, because, UseShouldAnalyzer.ARE_EQUAL_ID);
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

        if(TryMapTaskState(semanticModel, invocation, method, value, because, cancellationToken) is { } taskState) {
            return taskState;
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

        return new Replacement(target, value.Expression, null, because, UseShouldAnalyzer.IS_CONDITION_ID);
    }

    /// <summary>Maps a string or collection <c>Contains</c> assertion to its fluent <c>Contain</c> replacement.</summary>
    /// <param name="semanticModel">The semantic model for the invocation.</param>
    /// <param name="invocation">The assertion call.</param>
    /// <param name="method">The bound assertion method.</param>
    /// <param name="cancellationToken">Cancels the analysis.</param>
    /// <returns>The replacement, or <see langword="null"/> when no safe conversion exists.</returns>
    internal static Replacement? TryMapContains(SemanticModel semanticModel, InvocationExpressionSyntax invocation, IMethodSymbol method, CancellationToken cancellationToken) {
        var arguments = invocation.ArgumentList.Arguments;
        if(arguments.Count is not (2 or 3)) {
            return null;
        }

        var actualFirst = method.ContainingType.Name is "StringAssert" or "CollectionAssert";
        var actual = actualFirst ? arguments[0] : arguments[1];
        var expected = actualFirst ? arguments[1] : arguments[0];
        var because = NamedArgument(arguments, "message") ?? (arguments.Count == 3 ? arguments[2] : null);
        if(IsDelegateReason(because) || !IsStringArgument(because, semanticModel, cancellationToken)) {
            return null;
        }

        var actualType = semanticModel.GetTypeInfo(actual.Expression, cancellationToken).Type;
        var expectedType = semanticModel.GetTypeInfo(expected.Expression, cancellationToken).Type;
        if(actual.Expression is LiteralExpressionSyntax || actualType is null) {
            return null;
        }

        var isString = actualType.SpecialType == SpecialType.System_String;
        if(isString && expectedType?.SpecialType != SpecialType.System_String) {
            return null;
        }

        if(!isString && !IsGenericCollection(actualType)) {
            return null;
        }

        if(IsProvenUncompilable(semanticModel, invocation, actual.Expression, "Contain", expected.Expression)) {
            return null;
        }

        var diagnosticId = isString ? UseShouldAnalyzer.STRING_ASSERTION_ID : UseShouldAnalyzer.COLLECTION_ASSERTION_ID;
        return new Replacement("Contain", actual.Expression, expected, because, diagnosticId);
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

            if(TryMapBooleanLiteral(expected, actual, because, !negated, UseShouldAnalyzer.ARE_EQUAL_ID, out var boolean)) {
                return boolean;
            }

            if(!IsBeCapable(actualType)) {
                return null;
            }

            var equalityTarget = negated ? "NotBe" : "Be";
            if(IsProvenUncompilable(semanticModel, invocation, actual.Expression, equalityTarget, expected.Expression)) {
                return null;
            }

            return new Replacement(equalityTarget, actual.Expression, expected, because, UseShouldAnalyzer.ARE_EQUAL_ID);
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

        return new Replacement(target, actual.Expression, null, because, UseShouldAnalyzer.IS_CONDITION_ID);
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

    /// <summary>Determines whether an optional trailing argument is a string message that can become <c>because</c>.</summary>
    /// <param name="argument">The candidate argument (may be <see langword="null"/>).</param>
    /// <param name="semanticModel">The semantic model for the invocation.</param>
    /// <param name="cancellationToken">Cancels the analysis.</param>
    /// <returns><see langword="true"/> when the argument is absent or typed as a string.</returns>
    private static bool IsStringArgument(ArgumentSyntax? argument, SemanticModel semanticModel, CancellationToken cancellationToken) =>
        argument is null || semanticModel.GetTypeInfo(argument.Expression, cancellationToken).Type?.SpecialType == SpecialType.System_String;

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
    /// <param name="diagnosticId">The diagnostic rule that justifies the replacement.</param>
    /// <param name="replacement">The planned replacement when the literal is a boolean.</param>
    /// <returns><see langword="true"/> when <paramref name="expected"/> is a boolean literal.</returns>
    private static bool TryMapBooleanLiteral(ArgumentSyntax expected, ArgumentSyntax actual, ArgumentSyntax? because, bool isEqual, string diagnosticId, out Replacement? replacement) {
        replacement = null;
        if(expected.Expression is not LiteralExpressionSyntax literal
            || literal.Token.ValueText is not ("true" or "false")) {
            return false;
        }

#pragma warning disable IDE0047 // Chained == without parentheses misreads as one comparison.
        var target = (literal.Token.ValueText == "true") == isEqual ? "BeTrue" : "BeFalse";
#pragma warning restore IDE0047
        replacement = new Replacement(target, actual.Expression, null, because, diagnosticId);
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

    /// <summary>Determines whether the planned fluent call demonstrably fails to compile.</summary>
    /// <param name="semanticModel">The semantic model for the invocation.</param>
    /// <param name="invocation">The assertion call (provides the speculative binding position).</param>
    /// <param name="actual">The expression becoming the <c>Should()</c> receiver.</param>
    /// <param name="target">The planned fluent method name.</param>
    /// <param name="expected">The value argument (<see langword="null"/> when the target takes none).</param>
    /// <param name="typeArgument">The generic type argument for verbs such as <c>BeOfType&lt;T&gt;</c>.</param>
    /// <param name="requiresAwait">Whether the replacement targets an async verb and needs a <c>Func&lt;Task&gt;</c> receiver.</param>
    /// <returns><see langword="true"/> only when <c>Should()</c> binds but the target method does not;
    /// an unresolvable <c>Should()</c> (e.g. missing using) is inconclusive and trusts the type rules.</returns>
    internal static bool IsProvenUncompilable(SemanticModel semanticModel, InvocationExpressionSyntax invocation, ExpressionSyntax actual, string target, ExpressionSyntax? expected, TypeSyntax? typeArgument = null, bool requiresAwait = false) {
        var shouldCall = SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, AsReceiver(actual, requiresAwait), SyntaxFactory.IdentifierName("Should")));
        if(semanticModel.GetSpeculativeSymbolInfo(invocation.SpanStart, shouldCall, SpeculativeBindingOption.BindAsExpression).Symbol is not IMethodSymbol) {
            return false;
        }

        var methodName = typeArgument is null
            ? (SimpleNameSyntax)SyntaxFactory.IdentifierName(target)
            : SyntaxFactory.GenericName(SyntaxFactory.Identifier(target))
                .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(typeArgument)));
        var arguments = expected is null
            ? SyntaxFactory.ArgumentList()
            : SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(expected)));
        var fluentCall = SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, shouldCall, methodName),
            arguments);
        return semanticModel.GetSpeculativeSymbolInfo(invocation.SpanStart, fluentCall, SpeculativeBindingOption.BindAsExpression).Symbol is not IMethodSymbol;
    }
}

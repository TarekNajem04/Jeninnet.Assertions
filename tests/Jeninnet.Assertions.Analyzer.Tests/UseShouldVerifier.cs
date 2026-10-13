//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Analyzer.Tests;

/// <summary>Shared harness wiring the analyzer and code fix with framework references.</summary>
internal static class UseShouldVerifier {
    /// <summary>Verifies that <paramref name="source"/> produces no diagnostics.</summary>
    /// <param name="source">The code under test.</param>
    /// <returns>A task that represents the asynchronous verification operation.</returns>
    internal static async Task VerifyNoDiagnosticAsync(string source) {
        var test = new ShouldAnalyzerTest { TestCode = source };
        await test.RunAsync();
    }

    /// <summary>Verifies that <paramref name="source"/> produces no diagnostics alongside an extra compilation unit.</summary>
    /// <param name="source">The code under test.</param>
    /// <param name="extraSource">An additional compiled source file (e.g. a framework stub).</param>
    /// <returns>A task that represents the asynchronous verification operation.</returns>
    internal static async Task VerifyNoDiagnosticWithReferencesAsync(string source, string extraSource) {
        var test = new ShouldAnalyzerTest { TestCode = source };
        test.TestState.Sources.Add(extraSource);
        await test.RunAsync();
    }

    /// <summary>Verifies the diagnostic and the exact fixed output.</summary>
    /// <param name="source">The code under test, with the triggering call wrapped in <c>{|#0:|}</c> markup.</param>
    /// <param name="fixedSource">The expected code after the fix.</param>
    /// <param name="diagnosticId">The expected diagnostic ID.</param>
    /// <param name="messageArgs">The expected diagnostic message arguments.</param>
    /// <returns>A task that represents the asynchronous verification operation.</returns>
    internal static async Task VerifyCodeFixAsync(string source, string fixedSource, string diagnosticId, params string[] messageArgs) {
        var test = new ShouldCodeFixTest { TestCode = source, FixedCode = fixedSource };
        test.ExpectedDiagnostics.Add(new DiagnosticResult(diagnosticId, DiagnosticSeverity.Info).WithLocation(0).WithArguments(messageArgs));
        await test.RunAsync();
    }

    /// <summary>Verifies the diagnostic and the exact fixed output alongside an extra compilation unit.</summary>
    /// <param name="source">The code under test, with the triggering call wrapped in <c>{|#0:|}</c> markup.</param>
    /// <param name="fixedSource">The expected code after the fix.</param>
    /// <param name="diagnosticId">The expected diagnostic ID.</param>
    /// <param name="extraSource">An additional compiled source file (e.g. a framework stub).</param>
    /// <param name="messageArgs">The expected diagnostic message arguments.</param>
    /// <returns>A task that represents the asynchronous verification operation.</returns>
    internal static async Task VerifyCodeFixWithReferencesAsync(string source, string fixedSource, string diagnosticId, string extraSource, params string[] messageArgs) {
        var test = new ShouldCodeFixTest { TestCode = source, FixedCode = fixedSource };
        test.TestState.Sources.Add(extraSource);
        test.FixedState.Sources.Add(extraSource);
        test.ExpectedDiagnostics.Add(new DiagnosticResult(diagnosticId, DiagnosticSeverity.Info).WithLocation(0).WithArguments(messageArgs));
        await test.RunAsync();
    }

    private sealed class ShouldAnalyzerTest : CSharpAnalyzerTest<UseShouldAnalyzer, DefaultVerifier> {
        public ShouldAnalyzerTest() {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90;
            TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(typeof(Assert).Assembly.Location));
            TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(typeof(BoolAssertions).Assembly.Location));
        }
    }

    private sealed class ShouldCodeFixTest : CSharpCodeFixTest<UseShouldAnalyzer, UseShouldCodeFixProvider, DefaultVerifier> {
        public ShouldCodeFixTest() {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90;
            TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(typeof(Assert).Assembly.Location));
            TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(typeof(BoolAssertions).Assembly.Location));
        }
    }
}

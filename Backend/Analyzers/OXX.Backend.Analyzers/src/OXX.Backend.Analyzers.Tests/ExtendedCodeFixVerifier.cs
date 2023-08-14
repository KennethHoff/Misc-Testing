using Microsoft.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace OXX.Backend.Analyzers.Tests;

public sealed class ExtendedCodeFixVerifier<TAnalyzer, TCodeFix> : ExtendedCodeFixVerifier<TAnalyzer, TCodeFix,
    CSharpCodeFixTest<TAnalyzer, TCodeFix, MSTestVerifier>, MSTestVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
{
}

public class ExtendedCodeFixVerifier<TAnalyzer, TCodeFix, TTest, TVerifier>
    where TTest : CodeFixTest<TVerifier>, new()
    where TVerifier : IVerifier, new()
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
{
    public static DiagnosticResult Diagnostic()
    {
        var analyzer = new TAnalyzer();
        try
        {
            return Diagnostic(analyzer.SupportedDiagnostics.Single());
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException(
                $"'{nameof(Diagnostic)}()' can only be used when the analyzer has a single supported diagnostic. Use the '{nameof(Diagnostic)}(DiagnosticDescriptor)' overload to specify the descriptor from which to create the expected result.",
                ex);
        }
    }

    public static DiagnosticResult Diagnostic(string diagnosticId)
    {
        var analyzer = new TAnalyzer();
        try
        {
            return Diagnostic(analyzer.SupportedDiagnostics.Single(i => i.Id == diagnosticId));
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException(
                $"'{nameof(Diagnostic)}(string)' can only be used when the analyzer has a single supported diagnostic with the specified ID. Use the '{nameof(Diagnostic)}(DiagnosticDescriptor)' overload to specify the descriptor from which to create the expected result.",
                ex);
        }
    }

    public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor) => new(descriptor);

    public static Task VerifyAnalyzerAsync(string source, Action<CodeFixTest<TVerifier>>? configure = default,
        params DiagnosticResult[] expected)
    {
        var test = new TTest
        {
            TestCode = source,
        };

        configure?.Invoke(test);

        test.ExpectedDiagnostics.AddRange(expected);
        return test.RunAsync(CancellationToken.None);
    }

    public static Task VerifyCodeFixAsync(string source, string fixedSource,
        Action<CodeFixTest<TVerifier>>? configure = default,
        params DiagnosticResult[] expected)
    {
        var test = new TTest
        {
            TestCode = source,
            FixedCode = fixedSource,
        };

        configure?.Invoke(test);

        test.ExpectedDiagnostics.AddRange(expected);
        return test.RunAsync(CancellationToken.None);
    }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace OXX.Backend.Analyzers.Tests;

public class ExtendedSuppressorVerifier<TSuppressor> : ExtendedSuppressorVerifier<EmptyDiagnosticAnalyzer, TSuppressor>
    where TSuppressor : DiagnosticSuppressor, new()
{ }

public class ExtendedSuppressorVerifier<TAnalyzer, TSuppressor> : CSharpAnalyzerTest<TAnalyzer, MSTestVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TSuppressor : DiagnosticSuppressor, new()
{
    protected override IEnumerable<DiagnosticAnalyzer> GetDiagnosticAnalyzers()
        => base.GetDiagnosticAnalyzers().Concat(new[] { new TSuppressor() });

    public static Task VerifyAnalyzerAsync(CompilerDiagnostics compilerDiagnostics, string source, Action<CSharpAnalyzerTest<TAnalyzer, MSTestVerifier>>? configure = default,
        params DiagnosticResult[] expected)
    {
        var verifier = new ExtendedSuppressorVerifier<TAnalyzer, TSuppressor>
        {
            CompilerDiagnostics = compilerDiagnostics,
            TestCode = source,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net60,
        };

        verifier.TestState.AdditionalReferences.Add(
            MetadataReference.CreateFromFile(typeof(OneOf.OneOf<>).Assembly.Location));

        verifier.ExpectedDiagnostics.AddRange(expected);
        configure?.Invoke(verifier);

        return verifier.RunAsync(CancellationToken.None);
    }
}

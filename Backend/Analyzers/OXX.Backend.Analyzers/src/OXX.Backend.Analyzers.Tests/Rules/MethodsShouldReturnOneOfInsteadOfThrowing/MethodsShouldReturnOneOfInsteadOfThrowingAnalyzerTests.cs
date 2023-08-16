using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneOf;
using AnalyzerUnderTest = OXX.Backend.Analyzers.Rules.MethodsShouldReturnOneOfInsteadOfThrowing.MethodsShouldReturnOneOfInsteadOfThrowingAnalyzer;

namespace OXX.Backend.Analyzers.Tests.Rules.MethodsShouldReturnOneOfInsteadOfThrowing;
using Verifier = ExtendedAnalyzerVerifier<AnalyzerUnderTest>;

[TestClass]
public class MethodsShouldReturnOneOfInsteadOfThrowingAnalyzerTests
{
    [TestMethod]
    public async Task WhenMethodHasThrowStatements_ReportDiagnostic()
    {
        var text = CodeHelper.AddUsingsAndWrapInsideClass(
            """
            public static void MethodThatThrows()
            {
                throw new Exception();
            }
            """);

        var expected = Verifier.Diagnostic(AnalyzerUnderTest.Rule)
            .WithSpan(10, 5, 10, 27)
            .WithArguments("Exception");

        await Verifier.VerifyAnalyzerAsync(text, Configure, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task WhenMethodHasThrowExpressions_ReportDiagnostic()
    {
        var text = CodeHelper.AddUsingsAndWrapInsideClass(
            """
            public static void MethodThatThrows()
            {
                OneOf<string, bool>? hmm = "hmm";
                var huh = hmm ?? throw new Exception();
            }
            """);

        var expected = Verifier.Diagnostic(AnalyzerUnderTest.Rule)
            .WithSpan(11, 22, 11, 43)
            .WithArguments("Exception");

        await Verifier.VerifyAnalyzerAsync(text, Configure, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task WhenMethodHasNoThrowStatements_DoNotReportDiagnostic()
    {
        var text = CodeHelper.AddUsingsAndWrapInsideClass(
            """
            public static void MethodThatDoesNotThrow()
            {
                var x = 1;
            }
            """);

        await Verifier.VerifyAnalyzerAsync(text, Configure).ConfigureAwait(false);
    }

    private static void Configure(CSharpAnalyzerTest<AnalyzerUnderTest, MSTestVerifier> configuration)
    {
        configuration.ReferenceAssemblies = ReferenceAssemblies.Net.Net60;
        configuration.TestState.AdditionalReferences.Add(
            MetadataReference.CreateFromFile(typeof(OneOf<>).GetTypeInfo().Assembly.Location));
    }
}

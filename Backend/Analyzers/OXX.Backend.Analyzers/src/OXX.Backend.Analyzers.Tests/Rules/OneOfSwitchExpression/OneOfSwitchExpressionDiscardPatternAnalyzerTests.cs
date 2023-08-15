using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneOf;
using AnalyzerUnderTest = OXX.Backend.Analyzers.Rules.OneOfSwitchExpression.OneOfSwitchExpressionDiscardPatternAnalyzer;

namespace OXX.Backend.Analyzers.Tests.Rules.OneOfSwitchExpression;
using Verifier = ExtendedAnalyzerVerifier<AnalyzerUnderTest>;

[TestClass]
public class OneOfSwitchExpressionDiscardPatternAnalyzerTests
{
    [TestMethod]
    public async Task WhenExpressionChecksForDiscardPattern_ReportDiagnostic()
    {
        var text = CodeHelper.AddUsingsAndWrapInsideClass(
            """
            public static void DoThing()
            {
                OneOf<string, bool> twoOf = "hmm";
                string message = twoOf.Value switch
                {
                    _ => "one",
                };
            }
            """);

        var expected = Verifier.Diagnostic(AnalyzerUnderTest.Rule)
            .WithSpan(13, 9, 13, 19);

        await Verifier.VerifyAnalyzerAsync(text, Configure, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task WhenExpressionDoesNotCheckForDiscardPattern_ReportNoDiagnostic()
    {
        var text = CodeHelper.AddUsingsAndWrapInsideClass(
            """
            public static void DoThing()
            {
                OneOf<string, bool> twoOf = "hmm";
                string message = twoOf.Value switch { };
            }
            """);

        await Verifier.VerifyAnalyzerAsync(text, Configure).ConfigureAwait(false);
    }

    private static void Configure(
        CSharpAnalyzerTest<AnalyzerUnderTest, MSTestVerifier> configuration)
    {
        configuration.ReferenceAssemblies = ReferenceAssemblies.Net.Net60;
        configuration.TestState.AdditionalReferences.Add(
            MetadataReference.CreateFromFile(typeof(OneOf<>).Assembly.Location));
    }
}

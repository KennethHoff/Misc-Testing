using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneOf;
using OXX.Backend.Analyzers.Rules.OneOfSwitchExpression;
using OXX.Backend.Analyzers.Utilities;
using AnalyzerUnderTest = OXX.Backend.Analyzers.Rules.OneOfSwitchExpression.OneOfSwitchExpressionImpossibleCasesAnalyzer;

namespace OXX.Backend.Analyzers.Tests.Rules.OneOfSwitchExpression;
using Verifier = ExtendedAnalyzerVerifier<AnalyzerUnderTest>;

[TestClass]
public class OneOfSwitchExpressionImpossibleCasesAnalyzerTests
{
    [TestMethod]
    public async Task WhenExpressionChecksForNonContainedType_ReportDiagnostic()
    {
        var text = CodeHelper.AddUsingsAndWrapInsideClass(
            """
            public static void DoThing()
            {
                OneOf<string, bool> twoOf = "hmm";
                string message = twoOf.Value switch
                {
                    int i => i.ToString(),
                };
            }
            """);

        var expected = Verifier.Diagnostic(AnalyzerUnderTest.Rule)
            .WithSpan(12, 9, 12, 30)
            .WithArguments("int", DiagnosticUtilities.FixHtmlFormatting("OneOf<string, bool>"));

        await Verifier.VerifyAnalyzerAsync(text, Configure, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task WhenExpressionChecksForLiteral_ReportDiagnostic()
    {
        var text = CodeHelper.AddUsingsAndWrapInsideClass(
            """
            public static void DoThing()
            {
                OneOf<string, bool> twoOf = "hmm";
                string message = twoOf.Value switch
                {
                    1 => "one",
                };
            }
            """);

        var expected = Verifier.Diagnostic(AnalyzerUnderTest.RuleLiteralPattern)
            .WithSpan(12, 9, 12, 19);

        await Verifier.VerifyAnalyzerAsync(text, Configure, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task WhenNeitherLiteralNorImpossibleType_ReportNoDiagnostic()
    {
        var text = CodeHelper.AddUsingsAndWrapInsideClass(
            """
            public static void DoThing()
            {
                OneOf<string, bool> twoOf = "hmm";
                string message = twoOf.Value switch
                {
                    string s => s,
                    bool b => b.ToString(),
                };
            }
            """);

        await Verifier.VerifyAnalyzerAsync(text, Configure).ConfigureAwait(false);
    }

    private static void Configure(
        CSharpAnalyzerTest<OneOfSwitchExpressionImpossibleCasesAnalyzer, MSTestVerifier> configuration)
    {
        configuration.ReferenceAssemblies = ReferenceAssemblies.Net.Net60;
        configuration.TestState.AdditionalReferences.Add(
            MetadataReference.CreateFromFile(typeof(OneOf<>).GetTypeInfo().Assembly.Location));
    }
}

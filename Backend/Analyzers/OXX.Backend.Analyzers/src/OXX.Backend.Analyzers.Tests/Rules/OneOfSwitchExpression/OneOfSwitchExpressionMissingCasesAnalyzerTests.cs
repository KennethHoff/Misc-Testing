using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneOf;
using OXX.Backend.Analyzers.Constants;
using OXX.Backend.Analyzers.Rules.OneOfSwitchExpression;
using Verifier = OXX.Backend.Analyzers.Tests.ExtendedAnalyzerVerifier<
    OXX.Backend.Analyzers.Rules.OneOfSwitchExpression.OneOfSwitchExpressionMissingCasesAnalyzer>;

namespace OXX.Backend.Analyzers.Tests.Rules.OneOfSwitchExpression;

[TestClass]
public class OneOfSwitchExpressionMissingCasesAnalyzerTests
{
    [TestMethod]
    public async Task WhenExpressionIsEmpty_ReportDiagnostic()
    {
        var text = CodeHelper.AddUsingsAndWrapInsideClass(
            """
            public static void DoThing()
            {
                OneOf<string, bool> twoOf = "hmm";
                string message = twoOf.Value switch { };
            }
            """);

        var expected = Verifier.Diagnostic(AnalyzerId.OneOf.SwitchExpressionMissingCases)
            .WithSpan(10, 22, 10, 44)
            .WithArguments("string, bool");

        await Verifier.VerifyAnalyzerAsync(text, Configure, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task WhenExpressionHasMissingCases_ReportDiagnostic()
    {
        var text = CodeHelper.AddUsingsAndWrapInsideClass(
            """
            public static void DoThing()
            {
                OneOf<string, bool> twoOf = "hmm";
                string message = twoOf.Value switch
                {
                    string s => s,
                };
            }
            """);

        var expected = Verifier.Diagnostic(AnalyzerId.OneOf.SwitchExpressionMissingCases)
            .WithSpan(10, 22, 13, 6)
            .WithArguments("bool");

        await Verifier.VerifyAnalyzerAsync(text, Configure, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task WhenExpressionHasAllCases_ReportNoDiagnostic()
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
        CSharpAnalyzerTest<OneOfSwitchExpressionMissingCasesAnalyzer, MSTestVerifier> configuration)
    {
        configuration.ReferenceAssemblies = ReferenceAssemblies.Net.Net60;
        configuration.TestState.AdditionalReferences.Add(
            MetadataReference.CreateFromFile(typeof(OneOf<>).GetTypeInfo().Assembly.Location));
    }
}

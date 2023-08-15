using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OXX.Backend.Analyzers.Constants;
using SuppressorUnderTest = OXX.Backend.Analyzers.Rules.OneOfSwitchExpression.OneOfSwitchExpressionDiagnosticSuppressor;

namespace OXX.Backend.Analyzers.Tests.Rules.OneOfSwitchExpression;

using Verifier = ExtendedSuppressorVerifier<SuppressorUnderTest>;

[TestClass]
public class OneOfSwitchExpressionDiagnosticSuppressorAnalyzer
{
    [TestMethod]
    public async Task WhenSwitchExpressionIsOnOneOfValue_ThenSuppressDiagnostic()
    {
        var text = CodeHelper.AddUsingsAndWrapInsideClass(
            """
            public static void DoThing()
            {
                OneOf<string, bool> twoOf = "hmm";
                string message = twoOf.Value switch { };
            }
            """);

        await Verifier.VerifyAnalyzerAsync(CompilerDiagnostics.Warnings, text, Configure).ConfigureAwait(false);
    }
    
    [TestMethod]
    public async Task WhenSwitchExpressionIsOnSomeOtherMemberOnOneOf_ThenDoNotSuppressDiagnostic()
    {
        var text = CodeHelper.AddUsingsAndWrapInsideClass(
            """
            public static void DoThing()
            {
                OneOf<string, bool> twoOf = "hmm";
                string message = twoOf.Index switch { };
            }
            """);

        var expected = DiagnosticResult.CompilerWarning(AnalyzerId.BuiltIn.NonExhaustiveSwitchExpression)
            .WithSpan(11, 34, 11, 40)
            .WithArguments("_");

        await Verifier.VerifyAnalyzerAsync(CompilerDiagnostics.Warnings, text, Configure, expected).ConfigureAwait(false);
    }

    [TestMethod]
    public async Task WhenSwitchExpressionIsNotOnOneOfValue_ThenDoNotSuppressDiagnostic()
    {
        var text = CodeHelper.AddUsingsAndWrapInsideClass(
            """
            public static void DoThing()
            {
                string message = true switch { };
            }
            """);

        var expected = DiagnosticResult.CompilerWarning(AnalyzerId.BuiltIn.NonExhaustiveSwitchExpression)
            .WithSpan(10, 27, 10, 33)
            .WithArguments("_");

        await Verifier.VerifyAnalyzerAsync(CompilerDiagnostics.Warnings, text, Configure, expected).ConfigureAwait(false);
    }

    private void Configure(CSharpAnalyzerTest<EmptyDiagnosticAnalyzer, MSTestVerifier> verifier)
    {
        verifier.TestState.AdditionalReferences.Add(
            MetadataReference.CreateFromFile(typeof(OneOf.OneOf<>).Assembly.Location));
    }
}

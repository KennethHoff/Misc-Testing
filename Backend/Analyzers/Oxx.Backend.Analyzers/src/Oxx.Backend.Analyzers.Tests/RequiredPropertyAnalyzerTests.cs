using Xunit;
using Verifier =
	Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<Oxx.Backend.Analyzers.Rules.RequiredProperty.RequiredPropertyAnalyzer>;

namespace Oxx.Backend.Analyzers.Tests;

public class RequiredPropertyAnalyzerTests
{

	[Fact]
	public async Task NoCode_NoDiagnostics()
	{
		const string text = "";

		await Verifier.VerifyAnalyzerAsync(text);
	}

	[Fact]
	public async Task NonRequiredProperty_AlertDiagnostic()
	{
		const string text = """

                            public class Spaceship
                            {
                                public int Speed { get; set; }
                            }

                            """;

		var expected = Verifier.Diagnostic()
			.WithLocation(4, 16)
			.WithArguments("Speed");

		await Verifier.VerifyAnalyzerAsync(text, expected);
	}
}

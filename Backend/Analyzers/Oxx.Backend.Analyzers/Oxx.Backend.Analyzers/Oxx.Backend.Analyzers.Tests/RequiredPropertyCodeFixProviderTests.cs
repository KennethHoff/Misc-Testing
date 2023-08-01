using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<Oxx.Backend.Analyzers.RequiredPropertyAnalyzer,
        Oxx.Backend.Analyzers.RequiredPropertyCodeFixProvider>;

namespace Oxx.Backend.Analyzers.Tests;

public class RequiredPropertyCodeFixProviderTests
{
    [Fact]
    public async Task PropertyWithoutRequiredKeyword_AddRequiredKeyword()
    {
        const string text = """

                            public class Person
                            {
                                public string FirstName { get; set; }
                            }

                            """;

        const string newText = """

                               public class Person
                               {
                                   public required string FirstName { get; set; }
                               }

                               """;

        var expected = Verifier.Diagnostic()
            .WithArguments("FirstName");

        await Verifier.VerifyCodeFixAsync(text, expected, newText).ConfigureAwait(false);
    }
}
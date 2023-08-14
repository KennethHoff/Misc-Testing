using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace OXX.Backend.Analyzers.Tests;

public static class CompilationHelper
{
    public static Compilation CreateCompilation(string code)
    {
        var syntaxTrees = new[] { CSharpSyntaxTree.ParseText(code) };

        return CSharpCompilation.Create(
            Guid.NewGuid().ToString("N"),
            syntaxTrees,
            references: new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(OneOf.OneOf<bool>).Assembly.Location),
            },
            options: new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                reportSuppressedDiagnostics: true,
                nullableContextOptions: NullableContextOptions.Enable)
            );
    }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OXX.Backend.Analyzers.Utilities;

public static class MethodDeclarationUtilities
{
    public static bool Throws(MethodDeclarationSyntax methodDeclarationSyntax, out HashSet<SyntaxNode> syntaxesThatThrow)
    {
        syntaxesThatThrow = new HashSet<SyntaxNode>(methodDeclarationSyntax.Body?.Statements
            .Where(x => x.IsKind(SyntaxKind.ThrowStatement)) ?? Enumerable.Empty<SyntaxNode>());
        return syntaxesThatThrow.Any();
    }
}

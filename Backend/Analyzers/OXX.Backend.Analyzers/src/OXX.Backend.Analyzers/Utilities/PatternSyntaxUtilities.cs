using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OXX.Backend.Analyzers.Utilities;

public static class PatternSyntaxUtilities
{
    public static bool IsLiteral(this PatternSyntax pattern)
    {
        return pattern is ConstantPatternSyntax { Expression: LiteralExpressionSyntax };
    }

    public static bool IsDiscard(this PatternSyntax pattern)
    {
        return pattern is DiscardPatternSyntax;
    }
}

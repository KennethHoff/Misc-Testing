using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace OXX.Backend.Analyzers.Utilities;

public static class OneOfUtilities
{
    public static bool IsSwitchExpressionOnOneOfValue(SyntaxNodeAnalysisContext context,
        [NotNullWhen(true)] out SwitchExpressionSyntax? switchExpressionSyntax,
        [NotNullWhen(true)] out INamedTypeSymbol? oneOfTypeSymbol,
        [NotNullWhen(true)] out MemberAccessExpressionSyntax? memberAccessExpressionSyntax)
    {
        // If it's not a SwitchExpression on a MemberAccessExpression, we're not interested.
        if (!SwitchExpressionUtilities.HasSyntaxNodesForMemberAccess(context,
                out switchExpressionSyntax, out memberAccessExpressionSyntax))
        {
            oneOfTypeSymbol = null;
            return false;
        }

        return IsOneOfTypeSymbol(context, memberAccessExpressionSyntax, out oneOfTypeSymbol);
    }

    public static bool IsSwitchExpressionOnOneOfValue(SuppressionAnalysisContext context, Diagnostic diagnostic,
        [NotNullWhen(true)] out SwitchExpressionSyntax? switchExpressionSyntax,
        [NotNullWhen(true)] out INamedTypeSymbol? oneOfTypeSymbol,
        [NotNullWhen(true)] out MemberAccessExpressionSyntax? memberAccessExpressionSyntax)
    {
        // If it's not a SwitchExpression on a MemberAccessExpression, we're not interested.
        if (!SwitchExpressionUtilities.HasSyntaxNodesForMemberAccess(context, diagnostic,
            out switchExpressionSyntax, out memberAccessExpressionSyntax))
        {
            oneOfTypeSymbol = null;
            return false;
        }

        return IsOneOfTypeSymbol(context, diagnostic, memberAccessExpressionSyntax, out oneOfTypeSymbol);
    }

    public static bool IsOneOfTypeSymbol(SyntaxNodeAnalysisContext context,
        MemberAccessExpressionSyntax memberAccessExpressionSyntax,
        [NotNullWhen(true)] out INamedTypeSymbol? namedTypeSymbol)
    {
        var typeInfo = context.SemanticModel.GetTypeInfo(memberAccessExpressionSyntax.Expression);
        namedTypeSymbol = typeInfo.Type as INamedTypeSymbol;
        
        return IsOnValue(namedTypeSymbol, memberAccessExpressionSyntax);
    }

    public static bool IsOneOfTypeSymbol(SuppressionAnalysisContext context, Diagnostic diagnostic,
        MemberAccessExpressionSyntax memberAccessExpressionSyntax,
        [NotNullWhen(true)] out INamedTypeSymbol? namedTypeSymbol)
    {
        if (diagnostic.Location.SourceTree is null)
        {
            namedTypeSymbol = null;
            return false;
        }

        var semanticModel = context.GetSemanticModel(diagnostic.Location.SourceTree);

        var typeInfo = semanticModel.GetTypeInfo(memberAccessExpressionSyntax.Expression);
        namedTypeSymbol = typeInfo.Type as INamedTypeSymbol;
        
        return IsOnValue(namedTypeSymbol, memberAccessExpressionSyntax);
    }
    
    private static bool IsOnValue(INamedTypeSymbol? namedTypeSymbol, MemberAccessExpressionSyntax memberAccessExpressionSyntax)
    {
        // If the type is not `OneOf`, we're not interested.
        if (namedTypeSymbol is not { Name: "OneOf" })
        {
            return false;
        }

        // If the member is not `Value`, we're not interested.
        if (memberAccessExpressionSyntax.Name.Identifier.Text != "Value")
        {
            return false;
        }

        return true;
    }
}

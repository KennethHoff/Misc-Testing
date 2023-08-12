using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Oxx.Backend.Analyzers.Utilities;

public static class SwitchExpressionUtilities
{
	public static HashSet<ITypeSymbol> GetComparableTypeSymbolsForArms(SemanticModel semanticModel, SwitchExpressionSyntax switchExpressionSyntax)
	{
		return new HashSet<ITypeSymbol>(switchExpressionSyntax.Arms.Select(arm => GetTypeForArm(semanticModel, arm)).OfType<ITypeSymbol>(), SymbolEqualityComparer.Default);
	}

	public static ITypeSymbol? GetTypeForArm(SemanticModel semanticModel, SwitchExpressionArmSyntax armSyntax)
	{
		return semanticModel.GetTypeInfo(armSyntax.Pattern).ConvertedType;
	}

	public static (SwitchExpressionSyntax SwitchExpressionSyntax, MemberAccessExpressionSyntax MemberAccessExpressionSyntax)? GetSyntaxNodesForMemberAccess(SyntaxNodeAnalysisContext context)
	{
		// If it's not a SwitchExpression, we're not interested.
		if (context.Node is not SwitchExpressionSyntax switchExpressionSyntax)
		{
			return null;
		}

		// If the switch expression is not on a MemberAccessExpression, we're not interested
		// A `MemberAccessExpressionSyntax` is the `.Value` in `oneOfVariable.Value`
		if (switchExpressionSyntax.GoverningExpression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
		{
			return null;
		}

		return (switchExpressionSyntax, memberAccessExpressionSyntax);
	}
}

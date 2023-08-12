using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace OXX.Backend.Analyzers.Utilities;

public static class SwitchExpressionUtilities
{
	public static HashSet<ITypeSymbol> GetTypeSymbolsForArms(SemanticModel semanticModel,
		SwitchExpressionSyntax switchExpressionSyntax)
	{
		return new HashSet<ITypeSymbol>(
			switchExpressionSyntax.Arms.Select(arm => GetTypeForArm(semanticModel, arm)).OfType<ITypeSymbol>(),
			SymbolEqualityComparer.Default);
	}

	public static ITypeSymbol? GetTypeForArm(SemanticModel semanticModel, SwitchExpressionArmSyntax armSyntax)
	{
		return semanticModel.GetTypeInfo(armSyntax.Pattern).ConvertedType;
	}

	public static bool HasSyntaxNodesForMemberAccess(SyntaxNodeAnalysisContext context,
		[NotNullWhen(true)] out SwitchExpressionSyntax? switchExpressionSyntax,
		[NotNullWhen(true)] out MemberAccessExpressionSyntax? memberAccessExpressionSyntax)
	{
		// If it's not a SwitchExpression, we're not interested.
		if (context.Node is not SwitchExpressionSyntax switchExpression)
		{
			switchExpressionSyntax = null;
			memberAccessExpressionSyntax = null;
			return false;
		}

		// If the switch expression is not on a MemberAccessExpression, we're not interested
		// A `MemberAccessExpressionSyntax` is the `.Value` in `oneOfVariable.Value`
		if (switchExpression.GoverningExpression is not MemberAccessExpressionSyntax memberAccess)
		{
			switchExpressionSyntax = null;
			memberAccessExpressionSyntax = null;
			return false;
		}

		switchExpressionSyntax = switchExpression;
		memberAccessExpressionSyntax = memberAccess;
		return true;
	}

	public static bool HasSyntaxNodesForMemberAccess(SuppressionAnalysisContext context, Diagnostic diagnostic,
		[NotNullWhen(true)] out SwitchExpressionSyntax? switchExpressionSyntax,
		[NotNullWhen(true)] out MemberAccessExpressionSyntax? memberAccessExpressionSyntax)
	{
		switchExpressionSyntax = diagnostic.Location.SourceTree?.GetRoot()
			.FindNode(diagnostic.Location.SourceSpan) as SwitchExpressionSyntax;

		memberAccessExpressionSyntax = switchExpressionSyntax?.GoverningExpression as MemberAccessExpressionSyntax;
		return switchExpressionSyntax is not null && memberAccessExpressionSyntax is not null;
	}
}

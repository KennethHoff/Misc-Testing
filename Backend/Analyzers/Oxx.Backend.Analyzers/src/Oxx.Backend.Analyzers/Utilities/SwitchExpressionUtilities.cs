using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Oxx.Backend.Analyzers.Utilities;

public static class SwitchExpressionUtilities
{
	public static IEnumerable<ITypeSymbol> GetTypeForAllArms(SemanticModel semanticModel, SwitchExpressionSyntax switchExpressionSyntax)
	{
		return switchExpressionSyntax.Arms.Select(arm => GetTypeForArm(semanticModel, arm)).OfType<ITypeSymbol>();
	}

	public static ITypeSymbol? GetTypeForArm(SemanticModel semanticModel, SwitchExpressionArmSyntax armSyntax)
	{
		return semanticModel.GetTypeInfo(armSyntax.Pattern).ConvertedType;
	}
}

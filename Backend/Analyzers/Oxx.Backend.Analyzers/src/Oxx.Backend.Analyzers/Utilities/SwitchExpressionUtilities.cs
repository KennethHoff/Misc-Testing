using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Oxx.Backend.Analyzers.Utilities;

public static class SwitchExpressionUtilities
{
	public static IEnumerable<ITypeSymbol> GetTypeForAllArms(SemanticModel semanticModel, SwitchExpressionSyntax switchExpressionSyntax)
	{
		return switchExpressionSyntax.Arms.Select(arm => semanticModel.GetTypeInfo(arm.Pattern).ConvertedType).OfType<ITypeSymbol>();
	}
}

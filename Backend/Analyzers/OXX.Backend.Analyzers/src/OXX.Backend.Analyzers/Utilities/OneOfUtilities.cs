using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace OXX.Backend.Analyzers.Utilities;

public static class OneOfUtilities
{
	public static bool IsOneOfTypeSymbol(SyntaxNodeAnalysisContext context,
			MemberAccessExpressionSyntax memberAccessExpressionSyntax,
			[NotNullWhen(true)] out INamedTypeSymbol? oneOfTypeSymbol)
	{
		var typeInfo = context.SemanticModel.GetTypeInfo(memberAccessExpressionSyntax.Expression);
		oneOfTypeSymbol = typeInfo.Type as INamedTypeSymbol;

		return oneOfTypeSymbol?.Name == "OneOf";
	}

	public static bool IsOneOfTypeSymbol(SuppressionAnalysisContext context, Diagnostic diagnostic,
		MemberAccessExpressionSyntax memberAccessExpressionSyntax,
		[NotNullWhen(true)] out INamedTypeSymbol? oneOfTypeSymbol)
	{
		if (diagnostic.Location.SourceTree is null)
		{
			oneOfTypeSymbol = null;
			return false;
		}

		var semanticModel = context.GetSemanticModel(diagnostic.Location.SourceTree);

		var typeInfo = semanticModel.GetTypeInfo(memberAccessExpressionSyntax.Expression);
		oneOfTypeSymbol = typeInfo.Type as INamedTypeSymbol;

		return oneOfTypeSymbol?.Name == "OneOf";
	}
}

using System.Text;
using Microsoft.CodeAnalysis;

namespace Oxx.Backend.Analyzers.Utilities;

public static class DiagnosticUtilities
{
	public static string CreateMessageArguments(ReadOnlySpan<ITypeSymbol> typeSymbols)
	{
		var builder = new StringBuilder();

		for (var i = 0; i < typeSymbols.Length; i++)
		{
			var typeSymbol = typeSymbols[i];

			builder.Append(typeSymbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat));

			if (i < typeSymbols.Length - 1)
			{
				builder.Append(", ");
			}
		}

		return builder.ToString();
	}
}

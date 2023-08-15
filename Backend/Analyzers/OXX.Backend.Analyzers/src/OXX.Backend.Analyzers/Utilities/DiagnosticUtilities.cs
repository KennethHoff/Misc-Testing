using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using OXX.Backend.Analyzers.Constants;

namespace OXX.Backend.Analyzers.Utilities;

public static class DiagnosticUtilities
{
    private static readonly LocalizableString UnreachableTitle = new LocalizableResourceString(
        nameof(Resources.UnreachableTitle), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString UnreachableMessageFormat = new LocalizableResourceString(
        nameof(Resources.UnreachableMessageFormat), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString UnreachableDescription = new LocalizableResourceString(
        nameof(Resources.UnreachableDescription), Resources.ResourceManager, typeof(Resources));

    public static readonly DiagnosticDescriptor UnreachableRule = new(AnalyzerId.Unreachable, UnreachableTitle,
        UnreachableMessageFormat,
        DiagnosticCategory.Design, DiagnosticSeverity.Info, isEnabledByDefault: true, description: UnreachableDescription);

    public static SourceText CreateDebuggingSourceText(string text)
    {
        const string prefixText = """
		                          If you're seeing this, it's because the analyzer has failed to generate a proper CodeFix.
		                          Undo the CodeFix and please report this to Kenneth Hoff (kenneth.hoff@oxx.no) with the following information:

		                          - The code you were trying to analyze.
		                          - The analyzer ID.
		                          - Version of the analyzer (If you're not using the latest version, I probably will not help you).
		                          """;

        var sourceText = $"""
		                  /*
		                  {prefixText}

		                  Problem reported:
		                  {text}
		                  */
		                  """;

        return SourceText.From(sourceText);
    }

    public static DiagnosticDescriptor CreateRule(string id, string titleResourceName, string messageFormatResourceName, string descriptionResourceName, string category, DiagnosticSeverity diagnosticSeverity)
    {
        return new DiagnosticDescriptor(
            id,
            new LocalizableResourceString(titleResourceName, Resources.ResourceManager, typeof(Resources)),
            new LocalizableResourceString(messageFormatResourceName, Resources.ResourceManager, typeof(Resources)),
            category,
            diagnosticSeverity,
            true,
            new LocalizableResourceString(descriptionResourceName, Resources.ResourceManager, typeof(Resources))
        );
    }

    public static string CreateMessageArguments(scoped ReadOnlySpan<ITypeSymbol> typeSymbols)
    {
        var builder = new StringBuilder();

        for (var i = 0; i < typeSymbols.Length; i++)
        {
            var displayString = typeSymbols[i].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

            builder.Append(FixHtmlFormatting(displayString));

            if (i < typeSymbols.Length - 1)
            {
                builder.Append(", ");
            }
        }

        return builder.ToString();
    }

    public static string CreateMessageArgument(ITypeSymbol typeSymbol)
    {
        var displayString = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        return FixHtmlFormatting(displayString);
    }

    public static string FixHtmlFormatting(string message)
    {
        // Replace `<` and `>` with `&lt;` and `&gt;` to avoid HTML issues.
        message = message.Replace("<", "&lt;").Replace(">", "&gt;");

        return message;
    }
}

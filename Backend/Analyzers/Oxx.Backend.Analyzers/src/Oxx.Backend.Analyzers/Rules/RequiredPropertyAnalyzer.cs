using System.Collections.Immutable;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Oxx.Backend.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp), PublicAPI("Roslyn Analyzer")]
public sealed class RequiredPropertyAnalyzer : DiagnosticAnalyzer
{
	private static readonly LocalizableString Title = new LocalizableResourceString(
		nameof(Resources.OXX0001Title), Resources.ResourceManager, typeof(Resources));

	private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
		nameof(Resources.OXX0001MessageFormat), Resources.ResourceManager, typeof(Resources));

	private static readonly LocalizableString Description = new LocalizableResourceString(
		nameof(Resources.OXX0001Description), Resources.ResourceManager, typeof(Resources));

	private static readonly DiagnosticDescriptor Rule = new(AnalyzerIds.RequiredProperty, Title, MessageFormat,
		Categories.Design, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);

	public override void Initialize(AnalysisContext context)
	{
		// TODO: Should this be `None` instead?
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
											   GeneratedCodeAnalysisFlags.ReportDiagnostics);
		context.EnableConcurrentExecution();

		context.RegisterSymbolAction(AnalyzeOperation, SymbolKind.Property);
	}

	private void AnalyzeOperation(SymbolAnalysisContext context)
	{
		// If it's not a Property, we're not interested.
		if (context.Symbol is not IPropertySymbol propertySymbol)
		{
			return;
		}

		// If it's already required or nullable, we're not interested.
		if (propertySymbol.IsRequired || propertySymbol.NullableAnnotation is NullableAnnotation.Annotated)
		{
			return;
		}

		// Otherwise, report a diagnostic.
		context.ReportDiagnostic(Diagnostic.Create(Rule,
			// The highlighted area in the analyzed source code. Keep it as specific as possible.
			propertySymbol.Locations[0],
			// The value is passed to the 'MessageFormat' argument of your rule.
			propertySymbol.Name));
	}
}

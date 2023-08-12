// OXX0001 | Design   | Warning  | All non-nullable properties must be required.

// using System.Collections.Immutable;
// using JetBrains.Annotations;
// using Microsoft.CodeAnalysis;
// using Microsoft.CodeAnalysis.CSharp.Syntax;
// using Microsoft.CodeAnalysis.Diagnostics;
// using Oxx.Backend.Analyzers.Constants;
//
// namespace Oxx.Backend.Analyzers.Rules.RequiredProperty;
//
// [DiagnosticAnalyzer(LanguageNames.CSharp)]
// [PublicAPI("Roslyn Analyzer")]
// public sealed class RequiredPropertyAnalyzer : DiagnosticAnalyzer
// {
// 	private static readonly LocalizableString Title = new LocalizableResourceString(
// 		nameof(Resources.OXX0001Title), Resources.ResourceManager, typeof(Resources));
//
// 	private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
// 		nameof(Resources.OXX0001MessageFormat), Resources.ResourceManager, typeof(Resources));
//
// 	private static readonly LocalizableString Description = new LocalizableResourceString(
// 		nameof(Resources.OXX0001Description), Resources.ResourceManager, typeof(Resources));
//
// 	private static readonly DiagnosticDescriptor Rule = new(AnalyzerId.RequiredProperty, Title, MessageFormat,
// 		DiagnosticCategory.Design, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
//
// 	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);
//
// 	public override void Initialize(AnalysisContext context)
// 	{
// 		// This might need to be `GeneratedCodeAnalysisFlags.None` instead, but I'm not sure.
// 		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
// 		                                       GeneratedCodeAnalysisFlags.ReportDiagnostics);
// 		context.EnableConcurrentExecution();
//
// 		context.RegisterSymbolAction(AnalyzeOperation, SymbolKind.Property);
// 	}
//
// 	private void AnalyzeOperation(SymbolAnalysisContext context)
// 	{
// 		// If it's not a Property, we're not interested.
// 		if (context.Symbol is not IPropertySymbol propertySymbol)
// 		{
// 			return;
// 		}
//
// 		// If it's readonly, we're not interested.
// 		if (propertySymbol.IsReadOnly)
// 		{
// 			return;
// 		}
//
// 		// If it's already required or nullable, we're not interested.
// 		if (propertySymbol.IsRequired || propertySymbol.NullableAnnotation is NullableAnnotation.Annotated)
// 		{
// 			return;
// 		}
//
// 		// If it's initialized, we're not interested.
// 		if (IsSetInInitializer(propertySymbol) || IsSetInConstructor(propertySymbol))
// 		{
// 			return;
// 		}
//
// 		// If the property is static, only report nullable.
// 		if (propertySymbol.IsStatic)
// 		{
// 			ReportNullable(context, propertySymbol);
// 			return;
// 		}
//
// 		ReportRequiredOrNullable(context, propertySymbol);
// 	}
//
// 	private static void ReportRequiredOrNullable(SymbolAnalysisContext context, IPropertySymbol propertySymbol)
// 	{
// 		context.ReportDiagnostic(Diagnostic.Create(Rule, propertySymbol.Locations[0], propertySymbol.Name));
// 	}
//
// 	private static void ReportNullable(SymbolAnalysisContext context, IPropertySymbol propertySymbol)
// 	{
// 		context.ReportDiagnostic(Diagnostic.Create(Rule, propertySymbol.Locations[0], propertySymbol.Name));
// 	}
//
// 	// W.I.P - This is not yet working.
// 	// Should maybe be merged with IsSetInInitializer as they both iterate over the same nodes.
// 	private static bool IsSetInInitializer(IPropertySymbol propertySymbol)
// 	{
// 		return propertySymbol.DeclaringSyntaxReferences
// 			.SelectMany(x => x.GetSyntax().DescendantNodes())
// 			.OfType<AssignmentExpressionSyntax>()
// 			.Any(x => x.Left is IdentifierNameSyntax identifierNameSyntax &&
// 			          identifierNameSyntax.Identifier.Text == propertySymbol.Name);
// 	}
//
// 	// W.I.P - This is not yet working.
// 	private static bool IsSetInConstructor(IPropertySymbol propertySymbol)
// 	{
// 		return propertySymbol.DeclaringSyntaxReferences
// 			.SelectMany(x => x.GetSyntax().DescendantNodes())
// 			.OfType<ConstructorDeclarationSyntax>()
// 			.SelectMany(x => x.Body?.DescendantNodes() ?? Enumerable.Empty<SyntaxNode>())
// 			.OfType<AssignmentExpressionSyntax>()
// 			.Any(x => x.Left is IdentifierNameSyntax identifierNameSyntax &&
// 			          identifierNameSyntax.Identifier.Text == propertySymbol.Name);
// 	}
// }

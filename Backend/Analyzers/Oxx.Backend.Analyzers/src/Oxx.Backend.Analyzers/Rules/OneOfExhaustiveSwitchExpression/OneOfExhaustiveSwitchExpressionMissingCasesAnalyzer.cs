using System.Collections.Immutable;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Oxx.Backend.Analyzers.Constants;
using Oxx.Backend.Analyzers.Utilities;

namespace Oxx.Backend.Analyzers.Rules.OneOfExhaustiveSwitchExpression;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
[PublicAPI("Roslyn Analyzer")]
public sealed class OneOfExhaustiveSwitchExpressionMissingCasesAnalyzer : DiagnosticAnalyzer
{
	private static readonly LocalizableString Title = new LocalizableResourceString(
		nameof(Resources.OXX9001Title), Resources.ResourceManager, typeof(Resources));

	private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
		nameof(Resources.OXX9001MessageFormat), Resources.ResourceManager, typeof(Resources));

	private static readonly LocalizableString Description = new LocalizableResourceString(
		nameof(Resources.OXX9001Description), Resources.ResourceManager, typeof(Resources));

	private static readonly DiagnosticDescriptor Rule = new(AnalyzerId.OneOfSwitchExpressionMissingCases, Title,
		MessageFormat,
		DiagnosticCategory.Design, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule, DiagnosticUtilities.UnreachableRule);

	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
		                                       GeneratedCodeAnalysisFlags.ReportDiagnostics);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(AnalyzeOperation, SyntaxKind.SwitchExpression);
	}

	private static void AnalyzeOperation(SyntaxNodeAnalysisContext context)
	{
		// If it's not a SwitchExpression on a MemberAccessExpression, we're not interested.
		if (SwitchExpressionUtilities.GetSyntaxNodesForMemberAccess(context) is not
		    {
			    SwitchExpressionSyntax: var switchExpressionSyntax,
			    MemberAccessExpressionSyntax: var memberAccessExpressionSyntax
		    })
		{
			return;
		}

		// If it's not a OneOf, we're not interested.
		var typeInfo = context.SemanticModel.GetTypeInfo(memberAccessExpressionSyntax.Expression);
		if (typeInfo.Type is not INamedTypeSymbol { Name: "OneOf" } oneOfTypeSymbol)
		{
			return;
		}

		// If the switch expression has the same exact types as the OneOf, we're not interested.
		HashSet<ITypeSymbol> requiredTypes = new(oneOfTypeSymbol.TypeArguments, SymbolEqualityComparer.Default);
		HashSet<ITypeSymbol> currentTypes = SwitchExpressionUtilities.GetComparableTypeSymbolsForArms(context.SemanticModel, switchExpressionSyntax);

		var missingTypes = requiredTypes.Except(currentTypes).ToArray();

		if (missingTypes.Length is 0)
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(Rule, switchExpressionSyntax.GetLocation(),
			DiagnosticUtilities.CreateMessageArguments(missingTypes)));
	}
}

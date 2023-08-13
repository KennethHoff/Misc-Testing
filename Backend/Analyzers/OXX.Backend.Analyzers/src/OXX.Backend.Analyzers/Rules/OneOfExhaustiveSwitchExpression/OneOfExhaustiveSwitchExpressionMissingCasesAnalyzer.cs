using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using OXX.Backend.Analyzers.Constants;
using OXX.Backend.Analyzers.Utilities;

namespace OXX.Backend.Analyzers.Rules.OneOfExhaustiveSwitchExpression;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
[PublicAPI("Roslyn Analyzer")]
public sealed class OneOfExhaustiveSwitchExpressionMissingCasesAnalyzer : DiagnosticAnalyzer
{
	private static readonly DiagnosticDescriptor Rule = DiagnosticUtilities.CreateRule(
		AnalyzerId.OneOfSwitchExpressionMissingCases,
		nameof(Resources.OXX9001Title),
		nameof(Resources.OXX9001MessageFormat),
		nameof(Resources.OXX9001Description));

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
		= ImmutableArray.Create(Rule, DiagnosticUtilities.UnreachableRule);

	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
		                                       GeneratedCodeAnalysisFlags.ReportDiagnostics);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(AnalyzeSwitchExpressionForMissingCases, SyntaxKind.SwitchExpression);
	}

	private static void AnalyzeSwitchExpressionForMissingCases(SyntaxNodeAnalysisContext context)
	{
		// If it's not a SwitchExpression on a MemberAccessExpression, we're not interested.
		if (!SwitchExpressionUtilities.HasSyntaxNodesForMemberAccess(context,
			    out var switchExpressionSyntax, out var memberAccessExpressionSyntax))
		{
			return;
		}

		if (!OneOfUtilities.IsOneOfTypeSymbol(context, memberAccessExpressionSyntax, out var oneOfTypeSymbol))
		{
			return;
		}

		if (!IsSwitchExpressionMissingCases(context, switchExpressionSyntax, oneOfTypeSymbol, out var missingTypes))
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(Rule, switchExpressionSyntax.GetLocation(),
			DiagnosticUtilities.CreateMessageArguments(missingTypes)));
	}

	private static bool IsSwitchExpressionMissingCases(SyntaxNodeAnalysisContext context,
		SwitchExpressionSyntax switchExpressionSyntax, INamedTypeSymbol oneOfTypeSymbol,
		[NotNullWhen(true)] out ITypeSymbol[]? missingTypes)
	{
		var requiredTypes = oneOfTypeSymbol.TypeArguments;
		var currentTypes = SwitchExpressionUtilities.GetTypeSymbolsForArms(context.SemanticModel,
			switchExpressionSyntax);

		missingTypes = requiredTypes.Except(currentTypes).ToArray();

		return missingTypes.Length is not 0;
	}
}

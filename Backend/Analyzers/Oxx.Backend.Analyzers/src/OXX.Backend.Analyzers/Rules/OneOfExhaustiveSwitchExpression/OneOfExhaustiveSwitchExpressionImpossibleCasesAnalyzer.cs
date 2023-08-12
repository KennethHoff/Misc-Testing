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
public sealed class OneOfExhaustiveSwitchExpressionImpossibleCasesAnalyzer : DiagnosticAnalyzer
{
	private static readonly DiagnosticDescriptor Rule = DiagnosticUtilities.CreateRule(
		AnalyzerId.OneOfSwitchExpressionImpossibleCases,
		nameof(Resources.OXX9002Title),
		nameof(Resources.OXX9002MessageFormat),
		nameof(Resources.OXX9002Description));

	private static readonly DiagnosticDescriptor RuleDiscardPattern = DiagnosticUtilities.CreateRule(
		AnalyzerId.OneOfSwitchExpressionImpossibleCases,
		nameof(Resources.OXX9002TitleDiscardPattern),
		nameof(Resources.OXX9002MessageFormatDiscardPattern),
		nameof(Resources.OXX9002DescriptionDiscardPattern));


	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create(Rule, RuleDiscardPattern, DiagnosticUtilities.UnreachableRule);

	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
		                                       GeneratedCodeAnalysisFlags.ReportDiagnostics);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(AnalyzeSwitchExpressionForImpossibleCases, SyntaxKind.SwitchExpression);
	}

	private static void AnalyzeSwitchExpressionForImpossibleCases(SyntaxNodeAnalysisContext context)
	{
		// If it's not a SwitchExpression on a MemberAccessExpression, we're not interested.
		if (!SwitchExpressionUtilities.HasSyntaxNodesForMemberAccess(context,
			    out var switchExpressionSyntax, out var memberAccessExpressionSyntax))
		{
			return;
		}

		if (!OneOfUtilities.IsOneOfTypeSymbol(context, memberAccessExpressionSyntax,
			    out var oneOfTypeSymbol))
		{
			return;
		}

		if (!HasSwitchExpressionImpossibleTypes(context, switchExpressionSyntax, oneOfTypeSymbol,
			    out var impossibleTypes))
		{
			return;
		}

		ReportDiagnosticsForImpossibleCases();
		return;

		void ReportDiagnosticsForImpossibleCases()
		{
			var impossibleIndices = switchExpressionSyntax.Arms
				.Select((arm, index)
					=> (index, type: SwitchExpressionUtilities.GetTypeForArm(context.SemanticModel, arm)))
				.Where(t => impossibleTypes.Contains(t.type, SymbolEqualityComparer.Default))
				.Select(t => t.index)
				.ToArray();

			foreach (var index in impossibleIndices)
			{
				if (SwitchExpressionUtilities.GetTypeForArm(context.SemanticModel, switchExpressionSyntax.Arms[index])
				    is not { } impossibleType)
				{
					continue;
				}

				// If it's the DiscardPattern or `object`, report a different diagnostic.
				if (impossibleType.Name.Equals("Object", StringComparison.OrdinalIgnoreCase))
				{
					context.ReportDiagnostic(Diagnostic.Create(RuleDiscardPattern,
						switchExpressionSyntax.Arms[index].GetLocation()));
					continue;
				}

				context.ReportDiagnostic(Diagnostic.Create(Rule, switchExpressionSyntax.Arms[index].GetLocation(),
					DiagnosticUtilities.CreateMessageArgument(impossibleType),
					DiagnosticUtilities.CreateMessageArgument(oneOfTypeSymbol)));
			}
		}
	}

	private static bool HasSwitchExpressionImpossibleTypes(SyntaxNodeAnalysisContext context,
		SwitchExpressionSyntax switchExpressionSyntax, INamedTypeSymbol oneOfTypeSymbol,
		[NotNullWhen(true)] out ITypeSymbol[]? impossibleTypes)
	{
		var requiredTypes = oneOfTypeSymbol.TypeArguments;
		var currentTypes =
			SwitchExpressionUtilities.GetTypeSymbolsForArms(context.SemanticModel, switchExpressionSyntax);

		impossibleTypes = currentTypes.Except(requiredTypes).ToArray();

		return impossibleTypes.Length is not 0;
	}
}

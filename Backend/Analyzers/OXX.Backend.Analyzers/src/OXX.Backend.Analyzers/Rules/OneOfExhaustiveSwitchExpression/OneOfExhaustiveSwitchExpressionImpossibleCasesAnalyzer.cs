using System.Collections.Immutable;
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

	private static readonly DiagnosticDescriptor RuleLiteralPattern = DiagnosticUtilities.CreateRule(
		AnalyzerId.OneOfSwitchExpressionImpossibleCases,
		nameof(Resources.OXX9002TitleLiteralPattern),
		nameof(Resources.OXX9002MessageFormatLiteralPattern),
		nameof(Resources.OXX9002DescriptionLiteralPattern));


	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create(Rule, RuleDiscardPattern, RuleLiteralPattern, DiagnosticUtilities.UnreachableRule);

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

		if (!HasSwitchExpressionImpossibleArms(context, switchExpressionSyntax, oneOfTypeSymbol))
		{
			return;
		}

		ReportDiagnosticsForImpossibleCases();
		return;

		void ReportDiagnosticsForImpossibleCases()
		{
			var impossibleIndices = switchExpressionSyntax.Arms
				.Select((arm, index) => (arm, index))
				.Where(tuple =>
				{
					if (PatternSyntaxUtilities.IsLiteral(tuple.arm.Pattern) ||
					    PatternSyntaxUtilities.IsDiscard(tuple.arm.Pattern))
					{
						return true;
					}

					if (SwitchExpressionUtilities.GetTypeForArm(context.SemanticModel, tuple.arm) is not { } armType)
					{
						return true;
					}

					return !oneOfTypeSymbol.TypeArguments.Contains(armType);
				})
				.Select(tuple => tuple.index)
				.ToArray();

			foreach (var index in impossibleIndices)
			{
				var arm = switchExpressionSyntax.Arms[index];

				if (PatternSyntaxUtilities.IsLiteral(arm.Pattern))
				{
					context.ReportDiagnostic(Diagnostic.Create(RuleLiteralPattern, arm.GetLocation()));
					continue;
				}

				if (PatternSyntaxUtilities.IsDiscard(arm.Pattern))
				{
					context.ReportDiagnostic(Diagnostic.Create(RuleDiscardPattern, arm.GetLocation()));
					continue;
				}

				var armType = SwitchExpressionUtilities.GetTypeForArm(context.SemanticModel, arm);

				if (armType is null)
				{
					context.ReportDiagnostic(Diagnostic.Create(DiagnosticUtilities.UnreachableRule, arm.GetLocation()));
					continue;
				}

				context.ReportDiagnostic(Diagnostic.Create(Rule, arm.GetLocation(),
					DiagnosticUtilities.CreateMessageArgument(armType),
					DiagnosticUtilities.CreateMessageArgument(oneOfTypeSymbol)));
			}
		}
	}

	private static bool HasSwitchExpressionImpossibleArms(SyntaxNodeAnalysisContext context,
		SwitchExpressionSyntax switchExpressionSyntax, INamedTypeSymbol oneOfTypeSymbol)
	{
		var requiredTypes = oneOfTypeSymbol.TypeArguments;
		var typeSymbols = switchExpressionSyntax.Arms.Select(x => x.Pattern).ToArray();

		return typeSymbols
			.Any(pattern =>
			{
				if (PatternSyntaxUtilities.IsLiteral(pattern) || PatternSyntaxUtilities.IsDiscard(pattern))
				{
					return true;
				}

				var typeInfo = context.SemanticModel.GetTypeInfo(pattern);
				return typeInfo.ConvertedType is null || !requiredTypes.Contains(typeInfo.ConvertedType);
			});
	}
}

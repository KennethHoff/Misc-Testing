using System.Collections.Immutable;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using OXX.Backend.Analyzers.Constants;
using OXX.Backend.Analyzers.Utilities;

namespace OXX.Backend.Analyzers.Rules.OneOfSwitchExpression;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
[PublicAPI("Roslyn Analyzer")]
public sealed class OneOfSwitchExpressionImpossibleCasesAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor Rule = DiagnosticUtilities.CreateRule(
		AnalyzerId.OneOf.SwitchExpressionImpossibleCases,
		nameof(Resources.OXX9002Title),
		nameof(Resources.OXX9002MessageFormat),
		nameof(Resources.OXX9002Description));

	public static readonly DiagnosticDescriptor RuleLiteralPattern = DiagnosticUtilities.CreateRule(
		AnalyzerId.OneOf.SwitchExpressionImpossibleCases,
		nameof(Resources.OXX9002TitleLiteralPattern),
		nameof(Resources.OXX9002MessageFormatLiteralPattern),
		nameof(Resources.OXX9002DescriptionLiteralPattern));

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create(Rule, RuleLiteralPattern, DiagnosticUtilities.UnreachableRule);

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

		if (!HasImpossibleArms(context, switchExpressionSyntax, oneOfTypeSymbol))
		{
			return;
		}

		ReportDiagnosticsForImpossibleCases(context, switchExpressionSyntax, oneOfTypeSymbol);
	}

	private static void ReportDiagnosticsForImpossibleCases(SyntaxNodeAnalysisContext context,
		SwitchExpressionSyntax switchExpressionSyntax, INamedTypeSymbol oneOfTypeSymbol)
	{
		var impossibleIndices = switchExpressionSyntax.Arms
			.Select((arm, index) => (arm, index))
			.Where(tuple =>
			{
				if (tuple.arm.Pattern.IsDiscard())
				{
					// Discard patterns are handled by OneOfSwitchExpressionDiscardPatternAnalyzer
					return false;
				}

				if (tuple.arm.Pattern.IsLiteral())
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

			if (arm.Pattern.IsLiteral())
			{
				context.ReportDiagnostic(Diagnostic.Create(RuleLiteralPattern, arm.GetLocation()));
				continue;
			}

			if (SwitchExpressionUtilities.GetTypeForArm(context.SemanticModel, arm) is not {} armType)
			{
				context.ReportDiagnostic(Diagnostic.Create(DiagnosticUtilities.UnreachableRule, arm.GetLocation()));
				continue;
			}

			context.ReportDiagnostic(Diagnostic.Create(Rule, arm.GetLocation(),
				DiagnosticUtilities.CreateMessageArgument(armType),
				DiagnosticUtilities.CreateMessageArgument(oneOfTypeSymbol)));
		}
	}

	private static bool HasImpossibleArms(SyntaxNodeAnalysisContext context,
		SwitchExpressionSyntax switchExpressionSyntax, INamedTypeSymbol oneOfTypeSymbol)
	{
		var requiredTypes = oneOfTypeSymbol.TypeArguments;
		var typeSymbols = switchExpressionSyntax.Arms.Select(x => x.Pattern).ToArray();

		return typeSymbols.Any(pattern =>
		{
			if (pattern.IsDiscard())
			{
				// Discard patterns are handled by OneOfSwitchExpressionDiscardPatternAnalyzer
				return false;
			}

			if (pattern.IsLiteral())
			{
				return true;
			}

			var typeInfo = context.SemanticModel.GetTypeInfo(pattern);
			return typeInfo.ConvertedType is null || !requiredTypes.Contains(typeInfo.ConvertedType);
		});
	}
}

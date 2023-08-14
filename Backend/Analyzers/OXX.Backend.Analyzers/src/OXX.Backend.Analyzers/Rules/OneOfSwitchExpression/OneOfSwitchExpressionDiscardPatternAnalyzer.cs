using System.Collections.Immutable;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using OXX.Backend.Analyzers.Constants;
using OXX.Backend.Analyzers.Utilities;

namespace OXX.Backend.Analyzers.Rules.OneOfSwitchExpression;

[DiagnosticAnalyzer(LanguageNames.CSharp), PublicAPI("Roslyn Analyzer")]
public sealed class OneOfSwitchExpressionDiscardPatternAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor Rule = DiagnosticUtilities.CreateRule(
		AnalyzerId.OneOf.SwitchExpressionDiscardPattern,
		nameof(Resources.OXX9003Title),
		nameof(Resources.OXX9003MessageFormat),
		nameof(Resources.OXX9003Description));

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
		= ImmutableArray.Create(Rule, DiagnosticUtilities.UnreachableRule);

	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze
		                                       | GeneratedCodeAnalysisFlags.ReportDiagnostics);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(AnalyzeSwitchExpressionForDiscardPattern, SyntaxKind.SwitchExpression);
	}

	private static void AnalyzeSwitchExpressionForDiscardPattern(SyntaxNodeAnalysisContext context)
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

		if (!HasDiscardPattern(switchExpressionSyntax))
		{
			return;
		}

		ReportDiagnosticsForDiscardPattern(context, switchExpressionSyntax, oneOfTypeSymbol);
	}

	private static bool HasDiscardPattern(SwitchExpressionSyntax switchExpressionSyntax)
	{
		return switchExpressionSyntax.Arms.Any(x => x.Pattern.IsDiscard());
	}

	private static void ReportDiagnosticsForDiscardPattern(SyntaxNodeAnalysisContext context,
		SwitchExpressionSyntax switchExpressionSyntax, INamedTypeSymbol oneOfTypeSymbol)
	{
		// Find the discard pattern syntax node
		var discardPatternSyntax = switchExpressionSyntax.Arms.First(x => x.Pattern.IsDiscard());

		context.ReportDiagnostic(Diagnostic.Create(Rule, discardPatternSyntax.GetLocation()));
	}
}

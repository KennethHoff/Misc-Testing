using System.Collections.Immutable;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Oxx.Backend.Analyzers.Constants;
using Oxx.Backend.Analyzers.Utilities;

namespace Oxx.Backend.Analyzers.Rules.OneOfExhaustiveSwitchExpression;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
[PublicAPI("Roslyn Analyzer")]
public sealed class OneOfExhaustiveSwitchExpressionImpossibleCasesAnalyzer : DiagnosticAnalyzer
{
	private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.OXX9002Title),
		Resources.ResourceManager, typeof(Resources));

	private static readonly LocalizableString MessageFormat =
		new LocalizableResourceString(nameof(Resources.OXX9002MessageFormat), Resources.ResourceManager,
			typeof(Resources));

	private static readonly LocalizableString Description =
		new LocalizableResourceString(nameof(Resources.OXX9002Description), Resources.ResourceManager,
			typeof(Resources));

	private static readonly DiagnosticDescriptor Rule = new(AnalyzerId.OneOfSwitchExpressionImpossibleCases, Title,
		MessageFormat, DiagnosticCategory.Design, DiagnosticSeverity.Warning, isEnabledByDefault: true,
		description: Description);


	private static readonly LocalizableString DescriptionDiscardPattern =
		new LocalizableResourceString(nameof(Resources.OXX9002DescriptionDiscardPattern), Resources.ResourceManager,
			typeof(Resources));

	private static readonly LocalizableString MessageFormatDiscardPattern =
		new LocalizableResourceString(nameof(Resources.OXX9002MessageFormatDiscardPattern), Resources.ResourceManager,
			typeof(Resources));

	private static readonly LocalizableString TitleDiscardPattern =
		new LocalizableResourceString(nameof(Resources.OXX9002TitleDiscardPattern), Resources.ResourceManager,
			typeof(Resources));

	private static readonly DiagnosticDescriptor RuleDiscardPattern =
		new(AnalyzerId.OneOfSwitchExpressionImpossibleCases, TitleDiscardPattern, MessageFormatDiscardPattern,
			DiagnosticCategory.Design, DiagnosticSeverity.Warning, isEnabledByDefault: true,
			description: DescriptionDiscardPattern);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create(Rule, RuleDiscardPattern, DiagnosticUtilities.UnreachableRule);

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
		HashSet<ITypeSymbol> currentTypes =
			SwitchExpressionUtilities.GetComparableTypeSymbolsForArms(context.SemanticModel, switchExpressionSyntax);

		var impossibleTypes = currentTypes.Except(requiredTypes).ToArray();

		if (impossibleTypes.Length is 0)
		{
			return;
		}

		ReportDiagnosticsForImpossibleCases();

		return;

		void ReportDiagnosticsForImpossibleCases()
		{
			var indices = new List<int>();
			for (var i = 0; i < switchExpressionSyntax.Arms.Count; i++)
			{
				var arm = switchExpressionSyntax.Arms[i];
				var type = SwitchExpressionUtilities.GetTypeForArm(context.SemanticModel, arm);
				if (type is null)
				{
					continue;
				}

				if (impossibleTypes.Contains(type, SymbolEqualityComparer.Default))
				{
					indices.Add(i);
				}
			}

			foreach (var index in indices)
			{
				if (SwitchExpressionUtilities.GetTypeForArm(context.SemanticModel, switchExpressionSyntax.Arms[index])
				    is not
				    { } impossibleType)
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
}

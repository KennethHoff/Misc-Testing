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
public sealed class OneOfExhaustiveSwitchExpressionAnalyzer : DiagnosticAnalyzer
{
	private static readonly LocalizableString Title = new LocalizableResourceString(
		nameof(Resources.OXX9001Title_NotExhaustive), Resources.ResourceManager, typeof(Resources));


	private static readonly LocalizableString MessageFormatNotExhaustive = new LocalizableResourceString(
		nameof(Resources.OXX9001MessageFormat_NotExhaustive), Resources.ResourceManager, typeof(Resources));

	private static readonly LocalizableString MessageFormatTooExhaustive = new LocalizableResourceString(
		nameof(Resources.OXX9001MessageFormat_TooExhaustive), Resources.ResourceManager, typeof(Resources));

	private static readonly LocalizableString MessageFormatBoth = new LocalizableResourceString(
		nameof(Resources.OXX9001MessageFormat_Both), Resources.ResourceManager, typeof(Resources));


	private static readonly LocalizableString Description = new LocalizableResourceString(
		nameof(Resources.OXX9001Description), Resources.ResourceManager, typeof(Resources));

	private static readonly DiagnosticDescriptor RuleNotExhaustive = new(AnalyzerId.OneOfExhaustiveSwitch, Title,
		MessageFormatNotExhaustive,
		DiagnosticCategory.Design, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

	private static readonly DiagnosticDescriptor RuleTooExhaustive = new(AnalyzerId.OneOfExhaustiveSwitch, Title,
		MessageFormatTooExhaustive,
		DiagnosticCategory.Design, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

	private static readonly DiagnosticDescriptor RuleBoth = new(AnalyzerId.OneOfExhaustiveSwitch, Title,
		MessageFormatBoth,
		DiagnosticCategory.Design, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.CreateRange(
		new[]
		{
			RuleNotExhaustive,
			RuleTooExhaustive,
			RuleBoth
		});

	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
		                                       GeneratedCodeAnalysisFlags.ReportDiagnostics);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(AnalyzeOperation, SyntaxKind.SwitchExpression);
	}

	private static void AnalyzeOperation(SyntaxNodeAnalysisContext context)
	{
		// If it's not a SwitchExpression, we're not interested.
		if (context.Node is not SwitchExpressionSyntax switchExpressionSyntax)
		{
			return;
		}

		// If it's not a MemberAccessExpression, we're not interested.
		if (switchExpressionSyntax.GoverningExpression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
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
		HashSet<ITypeSymbol> currentTypes = new(SwitchExpressionUtilities.GetTypeForAllArms(context.SemanticModel, switchExpressionSyntax), SymbolEqualityComparer.Default);

		var missingTypes = requiredTypes.Except(currentTypes).ToArray();
		var redundantTypes = currentTypes.Except(requiredTypes).ToArray();

		if (missingTypes.Length == 0 && redundantTypes.Length == 0)
		{
			return;
		}

		// If there are more types in the OneOf than in the switch expression, report a "Missing" diagnostic.
		if (requiredTypes.IsSupersetOf(currentTypes))
		{
			context.ReportDiagnostic(Diagnostic.Create(RuleNotExhaustive, switchExpressionSyntax.GetLocation(),
				DiagnosticUtilities.CreateMessageArguments(missingTypes)));
			return;
		}

		// If there are more types in the switch expression than in the OneOf, report a "Unnecessary" diagnostic.
		if (currentTypes.IsSupersetOf(requiredTypes))
		{
			context.ReportDiagnostic(Diagnostic.Create(RuleTooExhaustive, switchExpressionSyntax.GetLocation(),
				DiagnosticUtilities.CreateMessageArguments(redundantTypes)));
			return;
		}

		// Otherwise, report a "Both" diagnostic.
		context.ReportDiagnostic(Diagnostic.Create(RuleBoth, switchExpressionSyntax.GetLocation(),
			DiagnosticUtilities.CreateMessageArguments(missingTypes),
			DiagnosticUtilities.CreateMessageArguments(redundantTypes)));
	}
}

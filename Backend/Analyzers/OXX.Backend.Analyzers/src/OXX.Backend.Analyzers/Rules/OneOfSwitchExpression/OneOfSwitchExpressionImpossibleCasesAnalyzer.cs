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
        nameof(Resources.OXX9002Description), DiagnosticCategory.Design, DiagnosticSeverity.Warning);

    public static readonly DiagnosticDescriptor RuleLiteralPattern = DiagnosticUtilities.CreateRule(
        AnalyzerId.OneOf.SwitchExpressionImpossibleCases,
        nameof(Resources.OXX9002TitleLiteralPattern),
        nameof(Resources.OXX9002MessageFormatLiteralPattern),
        nameof(Resources.OXX9002DescriptionLiteralPattern), DiagnosticCategory.Design, DiagnosticSeverity.Warning);

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
        // If it's not a SwitchExpression on a OneOf<T>.Value, we're not interested.
        if (!OneOfUtilities.IsSwitchExpressionOnOneOfValue(context,
                out var switchExpressionSyntax, out var oneOfTypeSymbol, out _))
        {
            return;
        }

        // If there are no impossible cases, we're not interested.
        if (!HasImpossibleArms(context, switchExpressionSyntax, oneOfTypeSymbol))
        {
            return;
        }

        // Otherwise, report the impossible cases
        ReportDiagnosticsForImpossibleCases(context, switchExpressionSyntax, oneOfTypeSymbol);
    }

    private static void ReportDiagnosticsForImpossibleCases(SyntaxNodeAnalysisContext context,
        SwitchExpressionSyntax switchExpressionSyntax, INamedTypeSymbol oneOfTypeSymbol)
    {
        var impossibleIndices = switchExpressionSyntax.Arms
            .Select((arm, index) => (arm, index))
            .Where(tuple =>
            {
                // If it's a discard pattern, we don't care as it's handled by OneOfSwitchExpressionDiscardPatternAnalyzer
                if (tuple.arm.Pattern.IsDiscard())
                {
                    return false;
                }

                // If it's a literal pattern, it's obviously not a Type, so it's impossible
                if (tuple.arm.Pattern.IsLiteral())
                {
                    return true;
                }

                // If we can't get the type for the arm, we can't check if it's impossible so we assume it is.
                // (I don't really understand what this would be, but it's here for safety)
                if (SwitchExpressionUtilities.GetTypeForArm(context.SemanticModel, tuple.arm) is not { } armType)
                {
                    return true;
                }

                // Otherwise, check if the type is required
                return !oneOfTypeSymbol.TypeArguments.Contains(armType);
            })
            .Select(tuple => tuple.index)
            .ToArray();

        foreach (var index in impossibleIndices)
        {
            var arm = switchExpressionSyntax.Arms[index];

            // If it's a literal pattern, it's obviously not a Type, so it's impossible
            if (arm.Pattern.IsLiteral())
            {
                context.ReportDiagnostic(Diagnostic.Create(RuleLiteralPattern, arm.GetLocation()));
                continue;
            }
            
            // If we can't get the type for the arm, we can't check if it's impossible so we assume it is.
            // We also don't know what the type is, so we just say it's UNKNOWN
            if (SwitchExpressionUtilities.GetTypeForArm(context.SemanticModel, arm) is not { } armType)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, arm.GetLocation(),
                    "UNKNOWN",
                    DiagnosticUtilities.CreateMessageArgument(oneOfTypeSymbol)));
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
        var currentArms = switchExpressionSyntax.Arms.Select(x => x.Pattern).ToArray();

        return currentArms.Any(pattern =>
        {
            // If it's a discard pattern, we don't care as it's handled by OneOfSwitchExpressionDiscardPatternAnalyzer
            if (pattern.IsDiscard())
            {
                return false;
            }

            // If it's a literal pattern, it's obviously not a Type, so it's impossible
            if (pattern.IsLiteral())
            {
                return true;
            }

            // Otherwise, check if the type is required
            var typeInfo = context.SemanticModel.GetTypeInfo(pattern);
            return typeInfo.ConvertedType is null || !requiredTypes.Contains(typeInfo.ConvertedType);
        });
    }
}

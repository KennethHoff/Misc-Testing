using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
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
public sealed class OneOfSwitchExpressionMissingCasesAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor Rule = DiagnosticUtilities.CreateRule(
        AnalyzerId.OneOf.SwitchExpressionMissingCases,
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
        // If it's not a SwitchExpression on a OneOf<T>.Value, we're not interested.
        if (!OneOfUtilities.IsSwitchExpressionOnOneOfValue(context,
                out var switchExpressionSyntax, out var oneOfTypeSymbol, out _))
        {
            return;
        }

        // If you have a discard pattern, you don't need to have all cases
        if (SwitchExpressionUtilities.HasDiscardPattern(switchExpressionSyntax))
        {
            return;
        }

        // If you don't miss any cases, you're good to go
        if (!HasMissingCases(context, switchExpressionSyntax, oneOfTypeSymbol, out var missingTypes))
        {
            return;
        }

        // Otherwise, report the missing cases
        context.ReportDiagnostic(Diagnostic.Create(Rule, switchExpressionSyntax.GetLocation(),
            DiagnosticUtilities.CreateMessageArguments(missingTypes)));
    }

    private static bool HasMissingCases(SyntaxNodeAnalysisContext context,
        SwitchExpressionSyntax switchExpressionSyntax, INamedTypeSymbol oneOfTypeSymbol,
        [NotNullWhen(true)] out ITypeSymbol[]? missingTypes)
    {
        var requiredTypes = oneOfTypeSymbol.TypeArguments;

        var literals = new HashSet<ITypeSymbol>(switchExpressionSyntax.Arms
            .Select(x => x.Pattern)
            .Where(x => x.IsLiteral())
            .Select(x => context.SemanticModel.GetTypeInfo(x).ConvertedType)
            .OfType<ITypeSymbol>(), EqualityComparer<ITypeSymbol>.Default);

        var nonLiterals = new HashSet<ITypeSymbol>(switchExpressionSyntax.Arms
            .Select(x => x.Pattern)
            .Where(x => !x.IsLiteral())
            .Select(x => context.SemanticModel.GetTypeInfo(x).ConvertedType)
            .OfType<ITypeSymbol>(), EqualityComparer<ITypeSymbol>.Default);

        var problematicLiterals = literals
            .Where(x => !nonLiterals.Contains(x));

        missingTypes = requiredTypes.Except(nonLiterals.Except(problematicLiterals)).ToArray();

        return missingTypes.Length is not 0;
    }
}

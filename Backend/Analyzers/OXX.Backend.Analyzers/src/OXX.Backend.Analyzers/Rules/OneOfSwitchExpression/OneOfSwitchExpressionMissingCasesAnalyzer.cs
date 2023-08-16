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
        nameof(Resources.OXX9001Description), DiagnosticCategory.Design, DiagnosticSeverity.Warning);

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

        // If it has a discard pattern, we're not interested as a discard pattern is always exhaustive
        if (SwitchExpressionUtilities.HasDiscardPattern(switchExpressionSyntax))
        {
            return;
        }

        // If there are no missing cases, we're not interested.
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
        // Get the required types from the OneOf<T>
        var requiredTypes = oneOfTypeSymbol.TypeArguments;

        // Literals are always bad, so only non-literals count towards fulfilling the OneOf.
        // However, their ConvertedType is the type of the literal, so we need to get them all in order to filter them out.
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

        // Get the types that are only present in the literals. These are problematic as they are not actually types.
        var problematicTypes = literals.Where(x => !nonLiterals.Contains(x));

        // Get the types that are present in the non-literals. These are unproblematic as they are actually types.
        var unproblematicTypes = nonLiterals.Except(problematicTypes);

        // Get the types that are required but not present in the unproblematic types. These are missing.
        missingTypes = requiredTypes.Except(unproblematicTypes).ToArray();

        // If there are no missing types, we're not interested.
        return missingTypes.Length is not 0;
    }
}

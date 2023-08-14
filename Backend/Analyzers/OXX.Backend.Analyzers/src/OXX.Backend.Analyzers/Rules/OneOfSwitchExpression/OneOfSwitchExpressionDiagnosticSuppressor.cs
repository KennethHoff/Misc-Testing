using System.Collections.Immutable;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using OXX.Backend.Analyzers.Constants;
using OXX.Backend.Analyzers.Utilities;

namespace OXX.Backend.Analyzers.Rules.OneOfSwitchExpression;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
[PublicAPI("Roslyn Analyzer")]
public sealed class OneOfSwitchExpressionDiagnosticSuppressor : DiagnosticSuppressor
{
    private static readonly LocalizableString Justification = new LocalizableResourceString(
        nameof(Resources.OXX9001SuppressorJustification), Resources.ResourceManager, typeof(Resources));

    private static readonly string[] SuppressedDiagnosticIds = { AnalyzerId.BuiltIn.NonExhaustiveSwitchExpression };

    private static readonly IReadOnlyDictionary<string, SuppressionDescriptor> SuppressionDescriptors =
        SuppressedDiagnosticIds.ToDictionary(id => id,
            id => new SuppressionDescriptor("OXX9001_SPR", id, Justification));

    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions { get; } =
        ImmutableArray.CreateRange(SuppressionDescriptors.Values);

    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
        foreach (var diagnostic in context.ReportedDiagnostics)
        {
            if (!SuppressionDescriptors.TryGetValue(diagnostic.Id, out var descriptor))
            {
                continue;
            }

            if (!OneOfUtilities.IsSwitchExpressionOnOneOfValue(context, diagnostic, out _, out _, out _))
            {
                continue;
            }

            context.ReportSuppression(Suppression.Create(descriptor, diagnostic));
        }
    }
}

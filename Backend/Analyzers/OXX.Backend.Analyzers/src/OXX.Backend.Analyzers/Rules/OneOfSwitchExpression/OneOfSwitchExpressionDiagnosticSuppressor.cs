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
            // It can't be anything other than the ones in the dictionary, so no need to be defensive here
            var descriptor = SuppressionDescriptors[diagnostic.Id];

            // If it's not a SwitchExpression on a OneOf<T>.Value, we're not interested.
            if (!OneOfUtilities.IsSwitchExpressionOnOneOfValue(context, diagnostic, out _, out _, out _))
            {
                continue;
            }
            
            // Otherwise, suppress it
            context.ReportSuppression(Suppression.Create(descriptor, diagnostic));
        }
    }
}

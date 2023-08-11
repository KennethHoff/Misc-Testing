using System.Collections.Immutable;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Oxx.Backend.Analyzers.Rules.OneOfExhaustiveSwitchExpression;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
[PublicAPI("Roslyn Analyzer")]
public sealed class OneOfExhaustiveSwitchExpressionDiagnosticSuppressor : DiagnosticSuppressor
{
	private static readonly LocalizableString Justification = new LocalizableResourceString(
		nameof(Resources.OXX9001SuppressorJustification), Resources.ResourceManager, typeof(Resources));

	private static readonly string[] SuppressedDiagnosticIds = { "CS8509" };

	private static readonly IReadOnlyDictionary<string, SuppressionDescriptor> SuppressionDescriptors =
		SuppressedDiagnosticIds.ToDictionary(id => id,
			id => new SuppressionDescriptor("OXX9001_SPR", id, Justification));

	public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions { get; } =
		ImmutableArray.CreateRange(SuppressionDescriptors.Values);

	public override void ReportSuppressions(SuppressionAnalysisContext context)
	{
		foreach (var diagnostic in context.ReportedDiagnostics)
		{
			if (SuppressionDescriptors.TryGetValue(diagnostic.Id, out var descriptor))
			{
				HandleSuppression(diagnostic, descriptor);
			}
		}

		return;

		void HandleSuppression(Diagnostic diagnostic, SuppressionDescriptor descriptor)
		{
			// If the diagnostic is not on a switch expression, we're not interested.
			if (diagnostic.Location.SourceTree?.GetRoot().FindNode(diagnostic.Location.SourceSpan) is not
			    SwitchExpressionSyntax switchExpressionSyntax)
			{
				return;
			}

			// If it's not a MemberAccessExpression, we're not interested.
			if (switchExpressionSyntax.GoverningExpression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
			{
				return;
			}

			// If it's not a OneOf, we're not interested.
			var semanticModel = context.GetSemanticModel(diagnostic.Location.SourceTree);
			var typeInfo = semanticModel.GetTypeInfo(memberAccessExpressionSyntax.Expression);
			if (typeInfo.Type is not INamedTypeSymbol { Name: "OneOf" })
			{
				return;
			}

			context.ReportSuppression(Suppression.Create(descriptor, diagnostic));
		}
	}
}

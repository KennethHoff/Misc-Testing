// using System.Collections.Immutable;
// using JetBrains.Annotations;
// using Microsoft.CodeAnalysis;
// using Microsoft.CodeAnalysis.Diagnostics;
//
// namespace Oxx.Backend.Analyzers.Rules.RequiredProperty;
//
// [DiagnosticAnalyzer(LanguageNames.CSharp)]
// [PublicAPI("Roslyn Analyzer")]
// public sealed class RequiredPropertyDiagnosticSuppressor : DiagnosticSuppressor
// {
// 	private static readonly LocalizableString Justification = new LocalizableResourceString(
// 		nameof(Resources.OXX0001SuppressorJustification), Resources.ResourceManager, typeof(Resources));
//
// 	private static readonly SuppressionDescriptor SuppressionDescriptor = new("OXX0001_SPR", "CS8618", Justification);
//
// 	public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions { get; } =
// 		ImmutableArray.Create(SuppressionDescriptor);
//
// 	public override void ReportSuppressions(SuppressionAnalysisContext context)
// 	{
// 		foreach (var diagnostic in context.ReportedDiagnostics)
// 		{
// 			HandleSuppression(diagnostic);
// 		}
//
// 		return;
//
// 		void HandleSuppression(Diagnostic diagnostic)
// 		{
// 			context.ReportSuppression(Suppression.Create(SuppressionDescriptor, diagnostic));
// 		}
// 	}
// }

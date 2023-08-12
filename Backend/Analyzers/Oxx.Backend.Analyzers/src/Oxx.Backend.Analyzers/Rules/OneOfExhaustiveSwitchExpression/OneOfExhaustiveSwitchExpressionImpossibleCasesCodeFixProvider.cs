using System.Collections.Immutable;
using System.Composition;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Oxx.Backend.Analyzers.Constants;
using Oxx.Backend.Analyzers.Utilities;

namespace Oxx.Backend.Analyzers.Rules.OneOfExhaustiveSwitchExpression;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OneOfExhaustiveSwitchExpressionImpossibleCasesCodeFixProvider))]
[Shared]
[PublicAPI("Roslyn Analyzer")]
public sealed class OneOfExhaustiveSwitchExpressionImpossibleCasesCodeFixProvider : CodeFixProvider
{
	public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	public override ImmutableArray<string> FixableDiagnosticIds { get; }
		= ImmutableArray.Create(AnalyzerId.OneOfSwitchExpressionImpossibleCases);

	public override Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		return RegisterCodeFixesAsync(context, context.Diagnostics.First());
	}

	private static async Task RegisterCodeFixesAsync(CodeFixContext context, Diagnostic diagnostic)
	{
		// If the document doesn't have a syntax root, we can't do anything (Impossible?)
		if (await context.Document.GetSyntaxRootAsync(context.CancellationToken) is not { } root)
		{
			ReportImpossibleCodeFix(context, diagnostic, "No syntax root found.");
			return;
		}

		// It will always be a SwitchExpression, due to the Analyzer's SyntaxKind filter.
		var switchExpressionArmSyntax = (SwitchExpressionArmSyntax)root.FindNode(diagnostic.Location.SourceSpan);

		// Adds a Code Fixer for removing the impossible type.
		context.RegisterCodeFix(
			CodeAction.Create(
				title: string.Format(Resources.OXX9002CodeFix),
				createChangedDocument: _ => RemoveImpossibleCase(root, context.Document, switchExpressionArmSyntax),
				equivalenceKey: nameof(Resources.OXX9002CodeFix)),
			diagnostic);
	}

	private static Task<Document> RemoveImpossibleCase(SyntaxNode root, Document document, SwitchExpressionArmSyntax armSyntax)
	{
		// Removes the impossible type from the switch expression.
		// If it's the last type, it will remove the entire switch expression.(?? Not sure why it's nullable)
		if (root.RemoveNode(armSyntax, SyntaxRemoveOptions.AddElasticMarker) is not {} newRoot)
		{
			return Task.FromResult(document.WithText(DiagnosticUtilities.CreateDebuggingSourceText("Root has been removed.")));
		}

		// Replaces the entire document with a new one that contains the new switch expression.
		return Task.FromResult(document.WithSyntaxRoot(newRoot));
	}

	/// <summary>
	/// This is a fallback CodeFix that should never be seen by the user, but useful for debugging. <br />
	/// This cannot be moved to a separate file due to RS1022.
	/// </summary>
	private static void ReportImpossibleCodeFix(CodeFixContext context, Diagnostic diagnostic, string information)
	{
		context.RegisterCodeFix(
			CodeAction.Create(
				title: string.Format(Resources.UnreachableTitle),
				createChangedDocument: c
					=> Task.FromResult(context.Document.WithText(DiagnosticUtilities.CreateDebuggingSourceText(information))),
				equivalenceKey: nameof(Resources.UnreachableCodeFix)),
			diagnostic);
	}
}

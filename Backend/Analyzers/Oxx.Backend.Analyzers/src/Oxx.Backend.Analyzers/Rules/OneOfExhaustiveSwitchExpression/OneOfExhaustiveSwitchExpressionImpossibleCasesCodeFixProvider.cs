using System.Collections.Immutable;
using System.Composition;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
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
		// Unsure if this is necessary, but it's here for reference.
		// await Task.WhenAll(context.Diagnostics.Select(diagnostic => RegisterCodeFixesAsync(context, diagnostic)));

		return RegisterCodeFixesAsync(context, context.Diagnostics.First());
	}

	private static async Task RegisterCodeFixesAsync(CodeFixContext context, Diagnostic diagnostic)
	{
		// If the document doesn't have a syntax root, we can't do anything (Impossible(?))
		if (await context.Document.GetSyntaxRootAsync(context.CancellationToken) is not { } root)
		{
			return;
		}

		// If the diagnostic node isn't an SwitchArm, we're not interested (Impossible(?))
		if (root.FindNode(diagnostic.Location.SourceSpan) is not SwitchExpressionArmSyntax switchExpressionArmSyntax)
		{
			return;
		}

		// Adds a Code Fixer for removing the impossible type.
		context.RegisterCodeFix(
			CodeAction.Create(
				title: string.Format(Resources.OXX9002CodeFix),
				createChangedDocument: c => RemoveImpossibleCase(diagnostic, context.Document, switchExpressionArmSyntax, c),
				equivalenceKey: nameof(Resources.OXX9002CodeFix)),
			diagnostic);
	}

	private static async Task<Document> RemoveImpossibleCase(Diagnostic diagnostic, Document document,
		SwitchExpressionArmSyntax armSyntax, CancellationToken ct)
	{
		// If the document doesn't have a syntax root, we can't do anything (Impossible(?))
		if (await document.GetSyntaxRootAsync(ct) is not { } root)
		{
			return document.WithText(DiagnosticUtilities.CreateDebuggingSourceText("No syntax root found."));
		}

		// Removes the impossible type from the switch expression.
		// If it's the last type, it will remove the entire switch expression.(??)
		if (root.RemoveNode(armSyntax, SyntaxRemoveOptions.KeepNoTrivia) is not {} newRoot)
		{
			return document.WithText(DiagnosticUtilities.CreateDebuggingSourceText("Root has been removed."));
		}

		// Replaces the entire document with a new one that contains the new switch expression.
		return document.WithSyntaxRoot(newRoot);
	}
}

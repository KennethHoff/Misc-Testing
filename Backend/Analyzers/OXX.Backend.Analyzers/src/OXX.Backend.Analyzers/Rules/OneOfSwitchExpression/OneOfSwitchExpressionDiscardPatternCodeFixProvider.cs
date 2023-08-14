using System.Collections.Immutable;
using System.Composition;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OXX.Backend.Analyzers.Constants;
using OXX.Backend.Analyzers.Utilities;

namespace OXX.Backend.Analyzers.Rules.OneOfSwitchExpression;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OneOfSwitchExpressionImpossibleCasesCodeFixProvider))]
[Shared]
[PublicAPI("Roslyn Analyzer")]
public sealed class OneOfSwitchExpressionDiscardPatternCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; }
        = ImmutableArray.Create(AnalyzerId.OneOf.SwitchExpressionDiscardPattern);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.First();

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
                title: string.Format(Resources.OXX9003CodeFix),
                createChangedDocument: _ => RemoveDiscardPattern(root, context.Document, switchExpressionArmSyntax),
                equivalenceKey: nameof(Resources.OXX9003CodeFix)),
            diagnostic);
    }

    private static Task<Document> RemoveDiscardPattern(SyntaxNode root, Document document, SyntaxNode syntaxNode)
    {
        // Removes the discard pattern from the switch expression. No idea why it's nullable.
        if (root.RemoveNode(syntaxNode, SyntaxRemoveOptions.AddElasticMarker) is not { } newRoot)
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
                    => Task.FromResult(
                        context.Document.WithText(DiagnosticUtilities.CreateDebuggingSourceText(information))),
                equivalenceKey: nameof(Resources.UnreachableCodeFix)),
            diagnostic);
    }
}

using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Oxx.Backend.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RequiredPropertyCodeFixProvider)), Shared]
public sealed class RequiredPropertyCodeFixProvider : CodeFixProvider
{
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;
    
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(AnalyzerIds.RequiredProperty);

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.First();

        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

        var diagnosticNode = root?.FindNode(diagnosticSpan);

        if (diagnosticNode is not PropertyDeclarationSyntax declaration)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: string.Format(Resources.OBA0001CodeFixTitle),
                createChangedDocument: c => AddRequiredKeywordAsync(context.Document, declaration, c),
                equivalenceKey: nameof(Resources.OBA0001CodeFixTitle)),
            diagnostic);
    }
    
    private static async Task<Document> AddRequiredKeywordAsync(Document document, PropertyDeclarationSyntax propertyDeclarationSyntax, CancellationToken cancellationToken)
    {
        var newPropertyDeclarationSyntax = propertyDeclarationSyntax.AddModifiers(SyntaxFactory.Token(SyntaxKind.RequiredKeyword));

        var root = await document.GetSyntaxRootAsync(cancellationToken);

        if (root is null)
        {
            return document;
        }

        var newRoot = root.ReplaceNode(propertyDeclarationSyntax, newPropertyDeclarationSyntax);

        return document.WithSyntaxRoot(newRoot);
    }
}
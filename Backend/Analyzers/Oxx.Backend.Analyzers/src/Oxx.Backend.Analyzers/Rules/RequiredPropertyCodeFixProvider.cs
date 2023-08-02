using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Oxx.Backend.Analyzers.Rules;

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

        // Adds a Code Fixer for adding the 'required' keyword.
        // This is the default fixer and is the one that will be applied with `dotnet format`.
        // (This is the default due to it being the first Code Action registered.)
        context.RegisterCodeFix(
            CodeAction.Create(
                title: string.Format(Resources.OXX0001CodeFix1Title),
                createChangedDocument: c => AddRequiredKeywordAsync(context.Document, declaration, c),
                equivalenceKey: nameof(Resources.OXX0001CodeFix1Title)),
            diagnostic);

        // Adds a Code Fixer for adding the nullable annotation. This is an alternative fixer that has to be applied manually.
        context.RegisterCodeFix(
            CodeAction.Create(
                title: string.Format(Resources.OXX0001CodeFix2Title),
                createChangedDocument: c => AddNullableAnnotationAsync(context.Document, declaration, c),
                equivalenceKey: nameof(Resources.OXX0001CodeFix2Title)),
            diagnostic);
    }

    private static async Task<Document> AddRequiredKeywordAsync(Document document,
        PropertyDeclarationSyntax propertyDeclarationSyntax, CancellationToken cancellationToken)
    {
        var newPropertyDeclarationSyntax =
            propertyDeclarationSyntax.AddModifiers(SyntaxFactory.Token(SyntaxKind.RequiredKeyword));

        var root = await document.GetSyntaxRootAsync(cancellationToken);

        if (root is null)
        {
            return document;
        }

        var newRoot = root.ReplaceNode(propertyDeclarationSyntax, newPropertyDeclarationSyntax);

        return document.WithSyntaxRoot(newRoot);
    }

    private static async Task<Document> AddNullableAnnotationAsync(Document document,
        PropertyDeclarationSyntax propertyDeclarationSyntax, CancellationToken cancellationToken)
    {
        var nullableTypeSyntax = SyntaxFactory.NullableType(propertyDeclarationSyntax.Type);
        var newPropertyDeclarationSyntax = propertyDeclarationSyntax.WithType(nullableTypeSyntax);

        var root = await document.GetSyntaxRootAsync(cancellationToken);

        if (root is null)
        {
            return document;
        }

        var newRoot = root.ReplaceNode(propertyDeclarationSyntax, newPropertyDeclarationSyntax);

        return document.WithSyntaxRoot(newRoot);
    }
}
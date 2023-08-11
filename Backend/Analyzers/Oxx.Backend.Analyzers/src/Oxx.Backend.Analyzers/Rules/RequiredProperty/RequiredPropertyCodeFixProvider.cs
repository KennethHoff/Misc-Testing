using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Oxx.Backend.Analyzers.Constants;

namespace Oxx.Backend.Analyzers.Rules.RequiredProperty;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RequiredPropertyCodeFixProvider)), Shared]
public sealed class RequiredPropertyCodeFixProvider : CodeFixProvider
{
	public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	public override ImmutableArray<string> FixableDiagnosticIds { get; } =
		ImmutableArray.Create(AnalyzerId.RequiredProperty);

	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		// Unsure if this is necessary, but it's here for reference.
		// await Task.WhenAll(context.Diagnostics.Select(diagnostic => RegisterCodeFixesAsync(context, diagnostic)));

		await RegisterCodeFixesAsync(context, context.Diagnostics.First());
	}

	private static async Task RegisterCodeFixesAsync(CodeFixContext context, Diagnostic diagnostic)
	{
		// If the document doesn't have a syntax root, we can't do anything.
		if (await context.Document.GetSyntaxRootAsync(context.CancellationToken) is not { } root)
		{
			return;
		}

		// If the diagnostic node isn't a Property, we're not interested.
		if (root.FindNode(diagnostic.Location.SourceSpan) is not PropertyDeclarationSyntax declaration)
		{
			return;
		}

		// Adds a Code Fixer for adding the 'required' keyword.
		// This is the default fixer and is the one that will be applied with `dotnet format`.
		// (This is the default due to it being the first Code Action registered.)
		context.RegisterCodeFix(
			CodeAction.Create(
				title: string.Format(Resources.OXX0001CodeFix1),
				createChangedDocument: c => AddRequiredKeywordAsync(context.Document, declaration, c),
				equivalenceKey: nameof(Resources.OXX0001CodeFix1)),
			diagnostic);

		// Adds a Code Fixer for adding the nullable annotation.
		// This is an alternative fixer that has to be applied manually if the other fixer is not desired.
		context.RegisterCodeFix(
			CodeAction.Create(
				title: string.Format(Resources.OXX0001CodeFix2),
				createChangedDocument: c => AddNullableAnnotationAsync(context.Document, declaration, c),
				equivalenceKey: nameof(Resources.OXX0001CodeFix2)),
			diagnostic);
	}

	private static async Task<Document> AddRequiredKeywordAsync(Document document,
		PropertyDeclarationSyntax propertyDeclarationSyntax, CancellationToken ct)
	{
		// If the document doesn't have a syntax root, we can't do anything.
		if (await document.GetSyntaxRootAsync(ct) is not { } root)
		{
			return document;
		}

		// Adds the 'required' keyword to the property.
		var newPropertyDeclarationSyntax =
			propertyDeclarationSyntax.AddModifiers(SyntaxFactory.Token(SyntaxKind.RequiredKeyword));

		// Replaces the old property with the new property.
		var newRoot = root.ReplaceNode(propertyDeclarationSyntax, newPropertyDeclarationSyntax);

		// Replaces the entire document with a new one that contains the new property.
		return document.WithSyntaxRoot(newRoot);
	}

	private static async Task<Document> AddNullableAnnotationAsync(Document document,
		PropertyDeclarationSyntax propertyDeclarationSyntax, CancellationToken ct)
	{
		// If the document doesn't have a syntax root, we can't do anything.
		if (await document.GetSyntaxRootAsync(ct) is not { } root)
		{
			return document;
		}

		// Creates the nullable annotation.
		var nullableTypeSyntax = SyntaxFactory.NullableType(propertyDeclarationSyntax.Type);

		// Adds the nullable annotation to the property.
		var newPropertyDeclarationSyntax = propertyDeclarationSyntax.WithType(nullableTypeSyntax);

		// Replaces the old property with the new property.
		var newRoot = root.ReplaceNode(propertyDeclarationSyntax, newPropertyDeclarationSyntax);

		// Replaces the entire document with a new one that contains the new property.
		return document.WithSyntaxRoot(newRoot);
	}
}

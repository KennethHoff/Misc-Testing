using System.Collections.Immutable;
using System.Composition;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OXX.Backend.Analyzers.Constants;
using OXX.Backend.Analyzers.Utilities;

namespace OXX.Backend.Analyzers.Rules.MethodsShouldReturnOneOfInsteadOfThrowing;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MethodsShouldReturnOneOfInsteadOfThrowingCodeFixProvider))]
[Shared]
[PublicAPI("Roslyn Analyzer")]
public sealed class MethodsShouldReturnOneOfInsteadOfThrowingCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; }
        = ImmutableArray.Create(AnalyzerId.OneOf.MethodsShouldReturnOneOfInsteadOfThrowing);

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
        
        // If it's a throw statement:
        if (root.FindNode(diagnostic.Location.SourceSpan) is ThrowStatementSyntax throwStatementSyntax)
        {
            // Adds a Code Fixer for converting the method to return a OneOf<T, Exception>.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: string.Format(Resources.OXX9005CodeFix),
                    createChangedDocument: _ => ConvertMethodToReturnOneOf(root, context.Document, throwStatementSyntax),
                    equivalenceKey: nameof(Resources.OXX9005CodeFix)),
                diagnostic);
            return;
        }
        
        // // If it's a throw expression:
        // if (root.FindNode(diagnostic.Location.SourceSpan) is ThrowExpressionSyntax throwExpressionSyntax)
        // {
        //     // Adds a Code Fixer for converting the method to return a OneOf<T, Exception>.
        //     context.RegisterCodeFix(
        //         CodeAction.Create(
        //             title: string.Format(Resources.OXX9005CodeFix),
        //             createChangedDocument: _ => ConvertMethodToReturnOneOf(root, context.Document, throwExpressionSyntax),
        //             equivalenceKey: nameof(Resources.OXX9005CodeFix)),
        //         diagnostic);
        //     return;
        // }
    }

    private Task<Document> ConvertMethodToReturnOneOf(SyntaxNode root, Document contextDocument, ThrowStatementSyntax throwExpressionSyntax)
    {
        // Get the method declaration syntax.
        var methodDeclarationSyntax = throwExpressionSyntax.FirstAncestorOrSelf<MethodDeclarationSyntax>() switch
        {
            { } syntax => syntax,
            _ => throw new NotSupportedException($"The syntax node {throwExpressionSyntax} is not supported.")
        };
        
        // Get the type of the exception.
        var exceptionTypeName = throwExpressionSyntax.Expression switch
        {
            ObjectCreationExpressionSyntax objectCreationExpressionSyntax => objectCreationExpressionSyntax.Type.ToString(),
            _ => throw new NotSupportedException($"The syntax node {throwExpressionSyntax.Expression} is not supported.")
        };
        
        var currentReturnType = methodDeclarationSyntax.ReturnType;
        
        if (currentReturnType.IsKind(SyntaxKind.VoidKeyword))
        {
            currentReturnType = SyntaxFactory.ParseTypeName("Success");
        }

        // Create a new type that returns a OneOf<T, Exception>.
        var newReturnType = SyntaxFactory.ParseTypeName($"OneOf<{currentReturnType}, {exceptionTypeName}>");
        
        // Replace the return type of the method.
        var newMethodDeclarationSyntax = methodDeclarationSyntax.WithReturnType(newReturnType);
        
        // Replace the method declaration syntax with the new one.
        var newRoot = root.ReplaceNode(methodDeclarationSyntax, newMethodDeclarationSyntax);
        
        // Replaces the entire document with a new one that contains the new method.
        return Task.FromResult(contextDocument.WithSyntaxRoot(newRoot));
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

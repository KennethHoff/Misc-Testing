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

namespace OXX.Backend.Analyzers.Rules.OneOfSwitchExpression;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OneOfSwitchExpressionMissingCasesCodeFixProvider))]
[Shared]
[PublicAPI("Roslyn Analyzer")]
public sealed class OneOfSwitchExpressionMissingCasesCodeFixProvider : CodeFixProvider
{
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override ImmutableArray<string> FixableDiagnosticIds { get; }
        = ImmutableArray.Create(AnalyzerId.OneOf.SwitchExpressionMissingCases);

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.First();

        // If the document doesn't have a syntax root, we can't do anything (Impossible(?))
        if (await context.Document.GetSyntaxRootAsync(context.CancellationToken) is not { } root)
        {
            ReportImpossibleCodeFix(context, "No syntax root found.");
            return;
        }

        // It will always be a SwitchExpression, due to the Analyzer's SyntaxKind filter.
        var switchExpressionSyntax = (SwitchExpressionSyntax)root.FindNode(diagnostic.Location.SourceSpan);

        // Conversely, it will also always have a MemberAccessExpressionSyntax, due to what the Analyzer is looking for.
        // A MemberAccessExpressionSyntax is the `.Value` in `oneOfVariable.Value`
        var memberAccessExpressionSyntax = (MemberAccessExpressionSyntax)switchExpressionSyntax.GoverningExpression;

        // If there is no SemanticModel, we can't do anything (Impossible? Not sure what would cause this)
        if (await context.Document.GetSemanticModelAsync(context.CancellationToken) is not { } semanticModel)
        {
            ReportImpossibleCodeFix(context, "No semantic model found.");
            return;
        }

        // Adds a Code Fixer for adding the missing types.
        context.RegisterCodeFix(
            CodeAction.Create(
                title: string.Format(Resources.OXX9001CodeFix),
                createChangedDocument: _ => AddMissingCases(context, root, memberAccessExpressionSyntax, semanticModel,
                    switchExpressionSyntax),
                equivalenceKey: nameof(Resources.OXX9001CodeFix)),
            diagnostic);
    }

    private static Task<Document> AddMissingCases(CodeFixContext context,
        SyntaxNode root,
        MemberAccessExpressionSyntax memberAccessExpressionSyntax,
        SemanticModel semanticModel,
        SwitchExpressionSyntax switchExpressionSyntax)
    {
        // We know that the MemberAccessExpressionSyntax is a OneOf, due to what the Analyzer is looking for.
        var oneOfTypeSymbol =
            (INamedTypeSymbol)semanticModel.GetTypeInfo(memberAccessExpressionSyntax.Expression).ConvertedType!;

        // Creates a new switch expression with the missing types added.
        var newSwitchExpressionSyntax = AddMissingArms(semanticModel, switchExpressionSyntax, oneOfTypeSymbol);

        // Replaces the old switch expression with the new switch expression.
        var newRoot = root.ReplaceNode(switchExpressionSyntax, newSwitchExpressionSyntax);

        // Replaces the entire document with a new one that contains the new switch expression.
        return Task.FromResult(context.Document.WithSyntaxRoot(newRoot));
    }

    private static SwitchExpressionSyntax AddMissingArms(SemanticModel semanticModel,
        SwitchExpressionSyntax switchExpressionSyntax, INamedTypeSymbol oneOfTypeSymbol)
    {
        // Literals are always bad, so only non-literals count towards fulfilling the OneOf.
        var currentNonLiteralArmTypes = new HashSet<ITypeSymbol>(switchExpressionSyntax.Arms
                .Where(arm => !arm.Pattern.IsLiteral())
                .Select(arm => SwitchExpressionUtilities.GetTypeForArm(semanticModel, arm)!),
            EqualityComparer<ITypeSymbol>.Default);

        // Missing types are the types that we need to synthesize arms for.
        var missingTypes = oneOfTypeSymbol.TypeArguments
            .Where(type => !currentNonLiteralArmTypes.Contains(type));

        // Create a new arm for each missing type.
        var newArms = missingTypes.Select(type => CreateSwitchExpressionArm(type, switchExpressionSyntax)).ToList();

        // Return the new switch expression with the new arms added.
        return switchExpressionSyntax.WithArms(
            SyntaxFactory.SeparatedList(switchExpressionSyntax.Arms.Concat(newArms)));
    }

    private static SwitchExpressionArmSyntax CreateSwitchExpressionArm(ITypeSymbol type,
        SwitchExpressionSyntax switchExpressionSyntax)
    {
        // Get the type syntax for the type (e.g. `string` or `int`)
        var typeSyntax =
            SyntaxFactory.IdentifierName(type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat));

        // Create a declaration pattern for the type (e.g. `string x` or `int x`)
        var patternSyntax = SyntaxFactory.DeclarationPattern(typeSyntax,
            SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("x")));

        // Create a throw expression for the arm (e.g. `throw new NotImplementedException()`)
        var expression = SyntaxFactory.ThrowExpression(SyntaxFactory.ObjectCreationExpression(
            SyntaxFactory.IdentifierName("NotImplementedException"), SyntaxFactory.ArgumentList(), null));

        // Return the new arm.
        return SyntaxFactory.SwitchExpressionArm(patternSyntax, expression);
    }

    /// <summary>
    /// This is a fallback CodeFix that should never be seen by the user, but useful for debugging. <br />
    /// This cannot be moved to a separate file due to RS1022.
    /// </summary>
    private static void ReportImpossibleCodeFix(CodeFixContext context, string information)
    {
        context.RegisterCodeFix(
            CodeAction.Create(
                title: string.Format(Resources.UnreachableTitle),
                createChangedDocument: c
                    => Task.FromResult(
                        context.Document.WithText(DiagnosticUtilities.CreateDebuggingSourceText(information))),
                equivalenceKey: nameof(Resources.UnreachableCodeFix)),
            context.Diagnostics);
    }
}

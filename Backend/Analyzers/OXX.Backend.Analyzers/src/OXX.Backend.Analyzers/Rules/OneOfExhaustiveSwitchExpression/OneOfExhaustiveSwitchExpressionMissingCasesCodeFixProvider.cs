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

namespace OXX.Backend.Analyzers.Rules.OneOfExhaustiveSwitchExpression;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OneOfExhaustiveSwitchExpressionMissingCasesCodeFixProvider))]
[Shared]
[PublicAPI("Roslyn Analyzer")]
public sealed class OneOfExhaustiveSwitchExpressionMissingCasesCodeFixProvider : CodeFixProvider
{
	public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	public override ImmutableArray<string> FixableDiagnosticIds { get; }
		= ImmutableArray.Create(AnalyzerId.OneOfSwitchExpressionMissingCases);

	public override Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		return RegisterCodeFixesAsync(context, context.Diagnostics.First());
	}

	private static async Task RegisterCodeFixesAsync(CodeFixContext context, Diagnostic diagnostic)
	{
		// If the document doesn't have a syntax root, we can't do anything (Impossible(?))
		if (await context.Document.GetSyntaxRootAsync(context.CancellationToken) is not { } root)
		{
			ReportImpossibleCodeFix(context, diagnostic, "No syntax root found.");
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
			ReportImpossibleCodeFix(context, diagnostic, "No semantic model found.");
			return;
		}

		// Adds a Code Fixer for adding the missing types.
		context.RegisterCodeFix(
			CodeAction.Create(
				title: string.Format(Resources.OXX9001CodeFix),
				createChangedDocument: _ => AddMissingCase(context.Document, root, memberAccessExpressionSyntax, semanticModel, switchExpressionSyntax),
				equivalenceKey: nameof(Resources.OXX9001CodeFix)),
			diagnostic);
	}

	private static Task<Document> AddMissingCase(Document document,
		SyntaxNode root,
		MemberAccessExpressionSyntax memberAccessExpressionSyntax,
		SemanticModel semanticModel,
		SwitchExpressionSyntax switchExpressionSyntax)
	{
		// We know that the MemberAccessExpressionSyntax is a OneOf, due to what the Analyzer is looking for.
		var oneOfTypeSymbol = (INamedTypeSymbol)semanticModel.GetTypeInfo(memberAccessExpressionSyntax.Expression).Type!;

		var newSwitchExpressionSyntax = AddMissingArms(semanticModel, switchExpressionSyntax, oneOfTypeSymbol);

		// Replaces the old switch expression with the new switch expression.
		var newRoot = root.ReplaceNode(switchExpressionSyntax, newSwitchExpressionSyntax);

		// Replaces the entire document with a new one that contains the new switch expression.
		return Task.FromResult(document.WithSyntaxRoot(newRoot));
	}

	private static SwitchExpressionSyntax AddMissingArms(SemanticModel semanticModel,
		SwitchExpressionSyntax switchExpressionSyntax,
		INamedTypeSymbol oneOfTypeSymbol)
	{
		// If the switch expression has the same exact types as the OneOf, we're not interested.
		HashSet<ITypeSymbol> requiredTypes = new(oneOfTypeSymbol.TypeArguments, SymbolEqualityComparer.Default);
		HashSet<ITypeSymbol> currentTypes = SwitchExpressionUtilities.GetTypeSymbolsForArms(semanticModel,
			switchExpressionSyntax);

		var missingTypes = requiredTypes.Except(currentTypes).ToArray();
		var newArms = SyntaxFactory.SeparatedList(switchExpressionSyntax.Arms);

		AddMissingArms(requiredTypes, missingTypes, ref newArms);

		return switchExpressionSyntax.WithArms(newArms);
	}

	private static void AddMissingArms(HashSet<ITypeSymbol> requiredTypes, ITypeSymbol[] missingTypes,
		ref SeparatedSyntaxList<SwitchExpressionArmSyntax> newArms)
	{
		foreach (var missingType in missingTypes)
		{
			// Get the type symbol for the missing type based on the OneOf's type arguments
			var typeSymbol = requiredTypes.First(typeSymbol => typeSymbol.Equals(missingType, SymbolEqualityComparer.Default));

			// Create a type syntax for the missing type symbol using a syntax format that is short and readable (e.g. "string" instead of "System.String")
			var typeSyntax = SyntaxFactory.IdentifierName(typeSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat));

			// Create a pattern that extracts the value from the OneOf
			var patternSyntax = SyntaxFactory.DeclarationPattern(typeSyntax,
				SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("x")));

			// Create a throw expression that throws a NotImplementedException
			var expressionSyntax = SyntaxFactory.ThrowExpression(SyntaxFactory.ObjectCreationExpression(
				SyntaxFactory.IdentifierName("NotImplementedException"),
				SyntaxFactory.ArgumentList(),
				null));

			var newArm = SyntaxFactory.SwitchExpressionArm(patternSyntax, expressionSyntax);

			newArms = newArms.Add(newArm);
		}
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

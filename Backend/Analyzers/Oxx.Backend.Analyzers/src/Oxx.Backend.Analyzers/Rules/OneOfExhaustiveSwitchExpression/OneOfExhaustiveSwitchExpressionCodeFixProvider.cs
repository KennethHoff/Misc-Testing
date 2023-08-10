using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Oxx.Backend.Analyzers.Constants;

namespace Oxx.Backend.Analyzers.Rules.OneOfExhaustiveSwitchExpression;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OneOfExhaustiveSwitchExpressionCodeFixProvider)), Shared]
public sealed class OneOfExhaustiveSwitchExpressionCodeFixProvider : CodeFixProvider
{
	public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	public override ImmutableArray<string> FixableDiagnosticIds { get; }
		= ImmutableArray.Create(AnalyzerId.OneOfExhaustiveSwitch);

	public override Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		// Unsure if this is necessary, but it's here for reference.
		// await Task.WhenAll(context.Diagnostics.Select(diagnostic => RegisterCodeFixesAsync(context, diagnostic)));

		return RegisterCodeFixesAsync(context, context.Diagnostics.First());
	}

	private static async Task RegisterCodeFixesAsync(CodeFixContext context, Diagnostic diagnostic)
	{
		// If the document doesn't have a syntax root, we can't do anything.
		if (await context.Document.GetSyntaxRootAsync(context.CancellationToken) is not { } root)
		{
			return;
		}

		// If the diagnostic node isn't a SwitchExpression, we're not interested.
		if (root.FindNode(diagnostic.Location.SourceSpan) is not SwitchExpressionSyntax switchExpressionSyntax)
		{
			return;
		}

		// Adds a Code Fixer for adding the missing types.
		context.RegisterCodeFix(
			CodeAction.Create(
				title: string.Format(Resources.OXX9001CodeFixTitle),
				createChangedDocument: c => AddMissingTypesAsync(context.Document, switchExpressionSyntax, c),
				equivalenceKey: nameof(Resources.OXX9001CodeFixTitle)),
			diagnostic);
	}

	private static async Task<Document> AddMissingTypesAsync(Document document,
		SwitchExpressionSyntax switchExpressionSyntax, CancellationToken cancellationToken)
	{
		// If the document doesn't have a syntax root, we can't do anything.
		if (await document.GetSyntaxRootAsync(cancellationToken) is not { } root)
		{
			return document;
		}

		if (switchExpressionSyntax.GoverningExpression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
		{
			return document;
		}

		// If there is no SemanticModel, we can't do anything.
		if (await document.GetSemanticModelAsync(cancellationToken) is not { } semanticModel)
		{
			return document;
		}

		// If it's not a OneOf, we're not interested.
		var typeInfo = semanticModel.GetTypeInfo(memberAccessExpressionSyntax.Expression);
		if (typeInfo.Type is not INamedTypeSymbol { Name: "OneOf" } oneOfTypeSymbol)
		{
			return document;
		}

		var newSwitchExpressionSyntax = ReplaceArms(switchExpressionSyntax, oneOfTypeSymbol);

		// Replaces the old switch expression with the new switch expression.
		var newRoot = root.ReplaceNode(switchExpressionSyntax, newSwitchExpressionSyntax);

		// Replaces the entire document with a new one that contains the new switch expression.
		return document.WithSyntaxRoot(newRoot);
	}

	private static SwitchExpressionSyntax ReplaceArms(SwitchExpressionSyntax switchExpressionSyntax, INamedTypeSymbol oneOfTypeSymbol)
	{
		var requiredTypes = new HashSet<string>(oneOfTypeSymbol.TypeArguments.Select(type => type.Name));
		var switchTypes = new HashSet<string>(switchExpressionSyntax.Arms.Select(arm => arm.Pattern.ToString()));

		var missingTypes = requiredTypes.Except(switchTypes).ToArray();
		var redundantTypes = switchTypes.Except(requiredTypes).ToArray();

		var newArms = SyntaxFactory.SeparatedList(switchExpressionSyntax.Arms);

		AddMissingArms(oneOfTypeSymbol, missingTypes, ref newArms);
		RemoveRedundantArms(redundantTypes, ref newArms);

		return switchExpressionSyntax.WithArms(newArms);
	}

	private static void AddMissingArms(INamedTypeSymbol oneOfTypeSymbol, IEnumerable<string> missingTypes,
		ref SeparatedSyntaxList<SwitchExpressionArmSyntax> newArms)
	{
		foreach (var missingType in missingTypes)
		{
			var typeSymbol = oneOfTypeSymbol.TypeArguments.First(type => type.Name == missingType);
			var typeSyntax = SyntaxFactory.IdentifierName(typeSymbol.Name);

			var newArm = SyntaxFactory.SwitchExpressionArm(
				SyntaxFactory.ConstantPattern(typeSyntax),
				SyntaxFactory.ThrowExpression(SyntaxFactory.ObjectCreationExpression(
					SyntaxFactory.IdentifierName("NotImplementedException"),
					SyntaxFactory.ArgumentList(),
					null)));

			newArms = newArms.Add(newArm);
		}
	}

	private static void RemoveRedundantArms(IEnumerable<string> redundantTypes,
		ref SeparatedSyntaxList<SwitchExpressionArmSyntax> newArms)
	{
		foreach (var redundantType in redundantTypes)
		{
			var redundantArm = newArms.First(arm => arm.Pattern.ToString() == redundantType);
			newArms = newArms.Remove(redundantArm);
		}
	}
}

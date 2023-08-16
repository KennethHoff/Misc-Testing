using System.Collections.Immutable;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using OXX.Backend.Analyzers.Constants;
using OXX.Backend.Analyzers.Utilities;

namespace OXX.Backend.Analyzers.Rules.MethodsShouldReturnOneOfInsteadOfThrowing;

[DiagnosticAnalyzer(LanguageNames.CSharp), PublicAPI("Roslyn Analyzer")]
public sealed class MethodsShouldReturnOneOfInsteadOfThrowingAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor Rule = DiagnosticUtilities.CreateRule(
        AnalyzerId.OneOf.MethodsShouldReturnOneOfInsteadOfThrowing,
        nameof(Resources.OXX9005Title),
        nameof(Resources.OXX9005MessageFormat),
        nameof(Resources.OXX9005Description),
        DiagnosticCategory.Design, DiagnosticSeverity.Warning);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
        = ImmutableArray.Create(Rule, DiagnosticUtilities.UnreachableRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze
                                               | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeMethodForThrowing, SyntaxKind.ThrowExpression, SyntaxKind.ThrowStatement);
        // TODO: Figure out ThrowKeyword
        // context.RegisterSyntaxNodeAction(AnalyzeMethodForThrowing, SyntaxKind.ThrowExpression, SyntaxKind.ThrowStatement, SyntaxKind.ThrowKeyword);
    }

    private void AnalyzeMethodForThrowing(SyntaxNodeAnalysisContext context)
    {
        if (context.Node.IsKind(SyntaxKind.ThrowStatement))
        {
            var throwStatementSyntax = (ThrowStatementSyntax)context.Node;
            var exceptionTypeName = throwStatementSyntax.Expression switch
            {
                ObjectCreationExpressionSyntax objectCreationExpressionSyntax => objectCreationExpressionSyntax.Type.ToString(),
                _ => throw new NotSupportedException($"The syntax node {throwStatementSyntax.Expression} is not supported.")
            };

            context.ReportDiagnostic(Diagnostic.Create(Rule, throwStatementSyntax.GetLocation(), exceptionTypeName));
            return;
        }
        
        if (context.Node.IsKind(SyntaxKind.ThrowExpression))
        {
            var throwExpressionSyntax = (ThrowExpressionSyntax)context.Node;
            var exceptionTypeName = throwExpressionSyntax.Expression switch
            {
                ObjectCreationExpressionSyntax objectCreationExpressionSyntax => objectCreationExpressionSyntax.Type.ToString(),
                _ => throw new NotSupportedException($"The syntax node {throwExpressionSyntax.Expression} is not supported.")
            };

            context.ReportDiagnostic(Diagnostic.Create(Rule, throwExpressionSyntax.GetLocation(), exceptionTypeName));
            return;
        }
    }
}

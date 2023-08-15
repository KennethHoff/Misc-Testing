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
        
        context.RegisterSyntaxNodeAction(AnalyzeMethodForThrowing, SyntaxKind.MethodDeclaration);
    }

    private void AnalyzeMethodForThrowing(SyntaxNodeAnalysisContext context)
    {
        // It will always be a MethodDeclarationSyntax, since we're only analyzing methods. (See SyntaxKind.MethodDeclaration)
        var methodDeclarationSyntax = (MethodDeclarationSyntax) context.Node;
        
        // If the method doesn't throw, we're not interested.
        if (!MethodDeclarationUtilities.Throws(methodDeclarationSyntax, out var syntaxesThatThrow))
        {
            return;
        }
        
        // Report diagnostics for the throw statements
        ReportDiagnosticsForThrowStatements(context, methodDeclarationSyntax, syntaxesThatThrow);
        
        // Otherwise, report the method
        // context.ReportDiagnostic(Diagnostic.Create(Rule, methodDeclarationSyntax.GetLocation()));
    }

    private void ReportDiagnosticsForThrowStatements(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclarationSyntax, IEnumerable<SyntaxNode> syntaxesThatThrow)
    {
        foreach (var syntaxThatThrows in syntaxesThatThrow)
        {
            var exceptionTypeName = syntaxThatThrows switch
            {
                ThrowStatementSyntax throwStatementSyntax => throwStatementSyntax.Expression switch
                {
                    ObjectCreationExpressionSyntax objectCreationExpressionSyntax => objectCreationExpressionSyntax.Type.ToString(),
                    _ => throw new NotSupportedException($"The syntax node {syntaxThatThrows} is not supported.")
                },
                _ => throw new NotSupportedException($"The syntax node {syntaxThatThrows} is not supported.")
            };
            
            context.ReportDiagnostic(Diagnostic.Create(Rule, syntaxThatThrows.GetLocation(), exceptionTypeName));
        }
    }
}

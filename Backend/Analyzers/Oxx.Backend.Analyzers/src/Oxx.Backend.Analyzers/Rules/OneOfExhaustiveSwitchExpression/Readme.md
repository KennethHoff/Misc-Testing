# OneOf\<T> exhaustive switch expression analyzer

This analyzer will warn you if you have a switch expression on a `OneOf<T>` that is not exhaustive.

## [Analyzer](OneOfExhaustiveSwitchExpressionAnalyzer.cs)
![OneOf Exhaustive Switch Expression Analyzer](assets/Analyzer.png)
![OneOf Exhaustive Switch Expression Analyzer with more Context](assets/Analyzer_MoreContext.png)

## [Code Fix](OneOfExhaustiveSwitchExpressionCodeFixProvider.cs)

### Add missing cases
![OneOf Exhaustive Switch Expression Code Fix](assets/CodeFix_Selector.png)

### Fixed
![OneOf Exhaustive Switch Expression Code Fix](assets/CodeFix_Fixed.png)

## [Diagnostic Suppressor](OneOfExhaustiveSwitchExpressionDiagnosticSuppressor.cs)

Suppresses [CS8509](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/pattern-matching-warnings) for all switch expressions on `OneOf<T>`.
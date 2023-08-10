# OneOf\<T> exhaustive switch expression analyzer

This analyzer will warn you if you have a switch expression on a `OneOf<T>` that is not exhaustive.

## [Analyzer](OneOfExhaustiveSwitchExpressionAnalyzer.cs)
![Analyzer - Not Exhaustive](assets/Analyzer_NotExhaustive.png)
![Analyzer - Too Exhaustive](assets/Analyzer_TooExhaustive.png)
![Analyzer - Both](assets/Analyzer_Both.png)

## [Code Fix](OneOfExhaustiveSwitchExpressionCodeFixProvider.cs)

### Add missing cases
![Code Fix - Selector](assets/CodeFix_Selector.png)

### Fixed
![Code Fix - Fixed](assets/CodeFix_Fixed.png)

## [Diagnostic Suppressor](OneOfExhaustiveSwitchExpressionDiagnosticSuppressor.cs)

Suppresses [CS8509](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/pattern-matching-warnings) for all switch expressions on `OneOf<T>`.
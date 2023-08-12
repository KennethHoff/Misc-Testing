# OneOf\<T> exhaustive switch expression analyzer

This analyzer will warn you if you have a switch expression on a `OneOf<T>.Value` that is not exhaustive or is checking for impossible cases.

## [Missing cases Analyzer](OneOfExhaustiveSwitchExpressionMissingCasesAnalyzer.cs)
![Analyzer - Missing cases](assets/Analyzer_MissingCases.png)

![Analyzer - Missing cases - Partial](assets/Analyzer_MissingCases2.png)

## [Impossible cases Analyzer](OneOfExhaustiveSwitchExpressionImpossibleCasesAnalyzer.cs)
![Analyzer - Impossible cases](assets/Analyzer_ImpossibleCases.png)

![Analyzer - Impossible cases - Discard Pattern](assets/Analyzer_ImpossibleCases2.png)

## [Missing cases Code Fix](OneOfExhaustiveSwitchExpressionMissingCasesCodeFixProvider.cs)
![Code Fix - Missing cases](assets/CodeFix_MissingCases.png)

## [Impossible cases Code Fix](OneOfExhaustiveSwitchExpressionImpossibleCasesCodeFixProvider.cs)
![Code Fix - Impossible cases](assets/CodeFix_ImpossibleCases.png)

## [Diagnostic Suppressor](OneOfExhaustiveSwitchExpressionDiagnosticSuppressor.cs)

Suppresses [CS8509](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/pattern-matching-warnings) for all switch expressions on `OneOf<T>.Value`.


## Future Plans

- Code Fixer to convert `OneOf<T>.Match` to `OneOf<T>.Value` switch expression
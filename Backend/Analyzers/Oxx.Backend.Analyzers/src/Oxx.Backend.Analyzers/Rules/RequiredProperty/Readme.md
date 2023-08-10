# Required Property Analyzer

This analyzer ensures all properties are either required or nullable. This is to prevent the use of the default value for a property, which can lead to unexpected behavior.

## [Analyzer](RequiredPropertyAnalyzer.cs)
![Required Property Analyzer](assets/Analyzer.png)

## [Code Fix](RequiredPropertyCodeFixProvider.cs)
![Required Property Code Fix](assets/CodeFix_Selector.png)

### Make property required
![Code Fix - Make Required](assets/CodeFix_Required.png)

### Make property nullable
![Code Fix - Make Nullable](assets/CodeFix_Nullable.png)


## [Diagnostic Suppressor](RequiredPropertyDiagnosticSuppressor.cs)
Suppresses [CS8618](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/nullable-warnings#nonnullable-reference-not-initialized) for all properties.
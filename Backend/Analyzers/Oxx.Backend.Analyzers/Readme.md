# Oxx.Backend.Analyzers

### Roslyn Analyzers
A .NET Standard project with implementations of sample analyzers and code fix providers.
**You must build this project to see the results (warnings) in the IDE.**

- [RequiredPropertyAnalyzer.cs](Oxx.Backend.Analyzers/Oxx.Backend.Analyzers/RequiredPropertyAnalyzer.cs): An analyzer that reports non-nullable properties that are not required.
    ![Required Property Analyzer](assets/RequiredPropertyAnalyzer_AnalyzerExample.png)
- [RequiredPropertyCodeFixProvider.cs](Oxx.Backend.Analyzers/Oxx.Backend.Analyzers/RequiredPropertyCodeFixProvider.cs): Adds two code fixers to the RequiredPropertyAnalyzer that either:
  - Adds the `required` keyword to the property.
  - Makes the property nullable.
    ![Required Property Code Fix](assets/RequiredPropertyAnalyzer_CodeFixExample.png)

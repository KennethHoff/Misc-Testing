# Oxx.Backend.Analyzers

### Roslyn Analyzers
A .NET Standard project with implementations of sample analyzers and code fix providers.
**You must build this project to see the results (warnings) in the IDE.**

- [RequiredPropertyAnalyzer.cs](Oxx.Backend.Analyzers/Oxx.Backend.Analyzers/RequiredPropertyAnalyzer.cs): An analyzer that reports properties that are not required.
- [RequiredPropertyCodeFixProvider.cs](Oxx.Backend.Analyzers/Oxx.Backend.Analyzers/RequiredPropertyCodeFixProvider.cs): A code fix that adds the `required` keyword to the property. The fix is linked to [RequiredPropertyAnalyzer.cs](Oxx.Backend.Analyzers/Oxx.Backend.Analyzers/RequiredPropertyAnalyzer.cs)

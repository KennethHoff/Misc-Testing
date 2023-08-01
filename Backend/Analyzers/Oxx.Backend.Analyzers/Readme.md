# Roslyn Analyzers Sample

A set of three sample projects that includes Roslyn analyzers with code fix providers. Enjoy this template to learn from and modify analyzers for your own needs.

## Content
### Oxx.Backend.Analyzers
A .NET Standard project with implementations of sample analyzers and code fix providers.
**You must build this project to see the results (warnings) in the IDE.**

- [RequiredPropertyAnalyzer.cs](Oxx.Backend.Analyzers/Oxx.Backend.Analyzers/RequiredPropertyAnalyzer.cs): An analyzer that reports properties that are not required.
- [RequiredPropertyCodeFixProvider.cs](Oxx.Backend.Analyzers/Oxx.Backend.Analyzers/RequiredPropertyCodeFixProvider.cs): A code fix that adds the `required` keyword to the property. The fix is linked to [RequiredPropertyAnalyzer.cs](Oxx.Backend.Analyzers/Oxx.Backend.Analyzers/RequiredPropertyAnalyzer.cs)

### Oxx.Backend.Analyzers.Sample
A project that references the sample analyzers. Note the parameters of `ProjectReference` in [Oxx.Backend.Analyzers.Sample.csproj](Oxx.Backend.Analyzers/Oxx.Backend.Analyzers.Sample/Oxx.Backend.Analyzers.Sample.csproj), they make sure that the project is referenced as a set of analyzers. 

### Oxx.Backend.Analyzers.Tests
Unit tests for the sample analyzers and code fix provider. The easiest way to develop language-related features is to start with unit tests.

## How To?
### How to debug?
- Use the [launchSettings.json](Oxx.Backend.Analyzers/Oxx.Backend.Analyzers/Properties/launchSettings.json) profile.
- Debug tests.
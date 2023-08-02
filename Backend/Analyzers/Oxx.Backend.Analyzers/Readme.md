# Oxx.Backend.Analyzers

## Roslyn Analyzers
A .NET Standard project with implementations of sample analyzers and code fix providers.

- [RequiredPropertyAnalyzer.cs](src/Oxx.Backend.Analyzers/Rules/RequiredPropertyAnalyzer.cs): An analyzer that reports non-nullable properties that are not required.
    ![Required Property Analyzer](assets/RequiredPropertyAnalyzer_Analyzer.png)
- [RequiredPropertyCodeFixProvider.cs](src/Oxx.Backend.Analyzers/Rules/RequiredPropertyCodeFixProvider.cs): Adds two code fixers to the RequiredPropertyAnalyzer that either:
  - Adds the `required` keyword to the property.
  - Makes the property nullable.

    ![Required Property Code Fix](assets/RequiredPropertyAnalyzer_CodeFix.png)

### How to use

1. Add the NuGet package to your project.
2. ???
3. Profit!

#### Modify the severity of the analyzer

By default, all analyzers are configured with severity `warning`.

If you have the msbuild property `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` set (which you should), then all analyzers will report `error`.

If you want to change the severity of any analyzer (From this package or otherwise), you can do so in your project's `.editorconfig` file.

In the following example, I will change the severity of the `RequiredPropertyAnalyzer` which has the Analyzer Id of `OXX0001`. (You can see all IDs provided by this package [here](https://github.com/KennethHoff/Misc-Testing/blob/master/Backend/Analyzers/Oxx.Backend.Analyzers/src/Oxx.Backend.Analyzers/Constants/AnalyzerId.cs))

```editorconfig
# Set the severity to `none`. This will disable the analyzer and code fixers.
dotnet_diagnostic.OXX0001.severity = none

# Set the severity to `error`. This will cause the analyzer to report an error.
dotnet_diagnostic.OXX0001.severity = error
```

See [here](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/configuration-options) for more information, but be aware that not everything in that document is supported by Rider.

## Source Generators

There aren't any 🤷🏻

### For contributors

If, after cloning this repository, you do _not_ see the analyzer warnings in the IDE, you may need to build the project first.

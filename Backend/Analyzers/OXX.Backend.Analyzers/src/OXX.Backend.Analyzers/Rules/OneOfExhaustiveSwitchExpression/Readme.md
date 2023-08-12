# OneOf\<T> exhaustive switch expression analyzer

This analyzer will warn you if you have a switch expression on a `OneOf<T>.Value` that is not exhaustive or is checking for impossible cases.

## [Missing cases Analyzer](OneOfExhaustiveSwitchExpressionMissingCasesAnalyzer.cs)
![Analyzer - Missing cases](assets/Analyzer_MissingCases.png)

![Analyzer - Missing cases - Partial](assets/Analyzer_MissingCases2.png)

## [Impossible cases Analyzer](OneOfExhaustiveSwitchExpressionImpossibleCasesAnalyzer.cs)
![Analyzer - Impossible cases](assets/Analyzer_ImpossibleCases.png)

![Analyzer - Impossible cases - Discard Pattern](assets/Analyzer_ImpossibleCases2.png)

## [Missing cases Code Fix](OneOfExhaustiveSwitchExpressionMissingCasesCodeFixProvider.cs)
![Code Fix - Missing cases](assets/CodeFix_MissingCases2.png)

## [Impossible cases Code Fix](OneOfExhaustiveSwitchExpressionImpossibleCasesCodeFixProvider.cs)
![Code Fix - Impossible cases](assets/CodeFix_ImpossibleCases.png)

## [Diagnostic Suppressor](OneOfExhaustiveSwitchExpressionDiagnosticSuppressor.cs)

Suppresses [CS8509](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/pattern-matching-warnings) for all switch expressions on `OneOf<T>.Value`.


## Known Issues

- The analyzer will not detect literal values as valid for the type, and will not report missing cases for them.
  - For example, with the following code:
    ```csharp
    OneOf<bool, string> stringOrBool = "Test";
    var message = stringOrBool.Value switch
    {
        true => "This is a bool",
        "A string" => "lol",
    };
    ```
    The analyzer will report no missing cases here, as both a bool and a string are present.
    This is clearly wrong, as even this simple example will throw a [SwitchExpressionException](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.switchexpressionexception)
- If there are multiple types with the same name in scope, the analyzer might create an invalid arm.
  - For example, with the following code:
    ```csharp
    using OneOf.Types; // Includes a `NotFound` type
    using Microsoft.AspNetCore.Http.HttpResults; // Also includes a `NotFound` type
    
    OneOf<NotFound, string> maybeString = "Test";
    
    var message = maybeString.Value switch
    {
        "A string" => "lol",
        // OXX9001: Switch expression is not exhaustive. You are missing cases for [NotFound].
    };
    
    /* Apply Code Fix */
    
    var message = maybeString.Value switch
    {
        "A string" => "lol",
        // OXX9002: NotFound is not a valid case for OneOf<NotFound, KhApplicationRole>
        NotFound => throw new NotImplementedException(),
        // OXX9001: Switch expression is not exhaustive. You are missing cases for [NotFound].
    };
    ```
    The analyzer will report no missing cases here, as both a bool and a string are present.
    This is clearly wrong, as even this simple example will throw a [SwitchExpressionException](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.switchexpressionexception)

## Future Plans

- Code Fixer to convert `OneOf<T>.Match` to `OneOf<T>.Value` switch expression
  - For example, look at the following code:
    ```csharp
    OneOf<NotFound, string> maybeString = "Test";
    
    var message = maybeString.Match(
        x => throw new NotImplementedException(),
        x => value
    );
    
    /* Apply Code Fix */
    
    OneOf<NotFound, string> maybeString = "Test";
    
    var message = maybeString.Value switch
    {
        NotFound x => throw new NotImplementedException(),
        string x => value,
    };
    ```
    This improves readability and maintainability, as the `Match` method is based on the order of the types, and not the types themselves.


- Code Fixer to convert methods that throws exceptions to `OneOf<T>.Value` switch expression
  - For example, look at the following code:
    ```csharp
    public void MethodThatThrows()
    {
        if (<SomeCondition>)
        {
            throw new MyCustomException("This is an exception");
        }
        // Logic goes here
    }
    
    /* Apply Code Fix */
    
    public OneOf<Success, MyCustomException> MethodThatThrows()
    {
        if (<SomeCondition>)
        {
            return new MyCustomException("This is an exception");
        }
        // Logic goes here
        return new Success();
    }
    ```
    This will allow you to use the `OneOf<T>.Value` switch expression to handle the exception, instead of having to use a try-catch block that you might forget to add, or might swallow up more exceptions than you intended.
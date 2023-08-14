namespace OXX.Backend.Analyzers.Tests;

public static class CodeHelper
{
    public static string AddUsingsAndWrapInsideClass(string code)
    {
        return $$"""
                 using OneOf;

                 namespace MyCode;

                 static class Test
                 {
                    {{code}}
                 }
                 """;
    }
}

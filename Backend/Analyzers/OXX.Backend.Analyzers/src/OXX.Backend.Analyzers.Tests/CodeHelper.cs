namespace OXX.Backend.Analyzers.Tests;

public static class CodeHelper
{
    public static string AddUsingsAndWrapInsideClass(string code)
    {
        return $$"""
                 using OneOf;
                 using System;

                 namespace MyCode;

                 static class Test
                 {
                    {{code}}
                 }
                 """;
    }
}

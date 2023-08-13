using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using OneOf;
using OneOf.Types;

namespace MicroBenchmarks;

[MemoryDiagnoser(false)]
public class OneOfMatchVsValueSwitch
{
    // private static OneOf<NotFound> ReturnValueType_OneOption()
    // {
    //     return new NotFound();
    // }
    //
    // private static OneOf<Person> ReturnReferenceType_OneOption()
    // {
    //     return new Person();
    // }
    //
    // private static OneOf<Person, int, NotFound> ReturnValueType_ThreeOptions()
    // {
    //     return new NotFound();
    // }
    //
    // private static OneOf<Person, int, NotFound> ReturnReferenceType_ThreeOptions()
    // {
    //     return new Person();
    // }
    //
    // private static OneOf<Person, int, NotFound, Error, Enum> ReturnValueType_FiveOptions()
    // {
    //     return new NotFound();
    // }
    //
    // private static OneOf<Person, int, NotFound, Error, Enum> ReturnReferenceType_FiveOptions()
    // {
    //     return new Person();
    // }
    //
    // private static OneOf<Person, int, NotFound, Error, Enum, float, long> ReturnValueType_SevenOptions()
    // {
    //     return new NotFound();
    // }
    //
    // private static OneOf<Person, int, NotFound, Error, Enum, float, long> ReturnReferenceType_SevenOptions()
    // {
    //     return new Person();
    // }
    //
    // // [Benchmark]
    // // public void Match_LocalValueType_ThreeOptions()
    // // {
    // //     OneOf<int, Guid, bool> oneOf = 42;
    // //     
    // //     var hmm = oneOf.Match(
    // //         value => "int",
    // //         value => "Guid",
    // //         value => "bool"
    // //     );
    // // }
    // //
    // // [Benchmark]
    // // public void Switch_LocalValueType_ThreeOptions()
    // // {
    // //     OneOf<int, Guid, bool> oneOf = 42;
    // //     
    // //     var hmm = oneOf.Value switch
    // //     {
    // //         int value => "int",
    // //         Guid value => "Guid",
    // //         bool value => "bool",
    // //     };
    // // }
    // //
    // // [Benchmark]
    // // public void Match_LocalReferenceType_ThreeOptions()
    // // {
    // //     OneOf<Person, Address, Name> oneOf = new Person();
    // //     
    // //     var hmm = oneOf.Match(
    // //         value => "Person",
    // //         value => "Address",
    // //         value => "Name"
    // //     );
    // // }
    // //
    // // [Benchmark]
    // // public void Switch_LocalReferenceType_ThreeOptions()
    // // {
    // //     OneOf<Person, Address, Name> oneOf = new Person();
    // //     
    // //     var hmm = oneOf.Value switch
    // //     {
    // //         Person value => "Person",
    // //         Address value => "Address",
    // //         Name value => "Name",
    // //     };
    // // }
    // //
    // // [Benchmark]
    // // public string Match_FromValueTypeReturningMethod_ThreeOptions()
    // // {
    // //     var result = ReturnValueType();
    // //     
    // //     return result.Match(
    // //         value => "Success",
    // //         value => "NotFound",
    // //         value => "string"
    // //     );
    // // }
    // //
    // // [Benchmark]
    // // public string Switch_FromValueTypeReturningMethod_ThreeOptions()
    // // {
    // //     var result = ReturnValueType();
    // //     
    // //     return result.Value switch
    // //     {
    // //         Success => "Success",
    // //         NotFound => "NotFound",
    // //         string => "string",
    // //     };
    // // }
    // //
    // //
    // // [Benchmark]
    // // public string Match_FromReferenceTypeReturningMethod_ThreeOptions()
    // // {
    // //     var result = ReturnReferenceType();
    // //     
    // //     return result.Match(
    // //         value => "Success",
    // //         value => "NotFound",
    // //         value => "Person"
    // //     );
    // // }
    // //
    // // [Benchmark]
    // // public string Switch_FromReferenceTypeReturningMethod_ThreeOptions()
    // // {
    // //     var result = ReturnReferenceType();
    // //     
    // //     return result.Value switch
    // //     {
    // //         Success => "Success",
    // //         NotFound => "NotFound",
    // //         Person => "Person",
    // //     };
    // // }
    // //
    //
    //
    // // [Benchmark]
    // // public Type Match_LocalValueType_ThreeOptions_GetType()
    // // {
    // //     OneOf<int, Guid, bool> oneOf = 42;
    // //
    // //     return oneOf.Match(
    // //         value => value.GetType(),
    // //         value => value.GetType(),
    // //         value => value.GetType()
    // //     );
    // // }
    // //
    // // [Benchmark]
    // // public Type Switch_LocalValueType_ThreeOptions_GetType()
    // // {
    // //     OneOf<Person, int, NotFound> oneOf = 42;
    // //
    // //     return oneOf.Value switch
    // //     {
    // //         int value => value.GetType(),
    // //         Guid value => value.GetType(),
    // //         bool value => value.GetType(),
    // //     };
    // // }
    // //
    // // [Benchmark]
    // // public Type Match_LocalReferenceType_ThreeOptions_GetType()
    // // {
    // //     OneOf<Person, int, NotFound> oneOf = new Person();
    // //
    // //     return oneOf.Match(
    // //         value => value.GetType(),
    // //         value => value.GetType(),
    // //         value => value.GetType()
    // //     );
    // // }
    // //
    // // [Benchmark]
    // // public Type Switch_LocalReferenceType_ThreeOptions_GetType()
    // // {
    // //     OneOf<Person, int, NotFound> oneOf = new Person();
    // //
    // //     return oneOf.Value switch
    // //     {
    // //         Person value => value.GetType(),
    // //         int value => value.GetType(),
    // //         NotFound value => value.GetType(),
    // //     };
    // // }
    // //
    // // [Benchmark]
    // // public Type Match_MethodReturningValueType_ThreeOptions_GetType()
    // // {
    // //     var oneOf = ReturnValueType_ThreeOptions();
    // //     
    // //     return oneOf.Match(
    // //         value => value.GetType(),
    // //         value => value.GetType(),
    // //         value => value.GetType()
    // //     );
    // // }
    // //
    // // [Benchmark]
    // // public Type Switch_MethodReturningValueType_ThreeOptions_GetType()
    // // {
    // //     var oneOf = ReturnValueType_ThreeOptions();
    // //     
    // //     return oneOf.Value switch
    // //     {
    // //         Person value => value.GetType(),
    // //         int value => value.GetType(),
    // //         NotFound value => value.GetType(),
    // //     };
    // // }
    // //
    // //
    // // [Benchmark]
    // // public Type Match_MethodReturningReferenceType_ThreeOptions_GetType()
    // // {
    // //     var oneOf = ReturnReferenceType_ThreeOptions();
    // //     
    // //     return oneOf.Match(
    // //         value => value.GetType(),
    // //         value => value.GetType(),
    // //         value => value.GetType()
    // //     );
    // // }
    // //
    // // [Benchmark]
    // // public Type Switch_MethodReturningReferenceType_ThreeOptions_GetType()
    // // {
    // //     var oneOf = ReturnReferenceType_ThreeOptions();
    // //     
    // //     return oneOf.Value switch
    // //     {
    // //         Person value => value.GetType(),
    // //         int value => value.GetType(),
    // //         NotFound value => value.GetType(),
    // //     };
    // // }
    //
    // [Benchmark]
    // public Type Match_MethodReturningReferenceType_OneOption_GetType()
    // {
    //     var oneOf = ReturnReferenceType_OneOption();
    //     
    //     return oneOf.Match(
    //         value => value.GetType()
    //     );
    // }
    //
    // [Benchmark]
    // public Type Switch_MethodReturningReferenceType_OneOption_GetType()
    // {
    //     var oneOf = ReturnReferenceType_OneOption();
    //     
    //     return oneOf.Value switch
    //     {
    //         Person value => value.GetType(),
    //     };
    // }
    //
    // [Benchmark]
    // public Type Match_MethodReturningReferenceType_ThreeOptions_GetType()
    // {
    //     var oneOf = ReturnReferenceType_ThreeOptions();
    //     
    //     return oneOf.Match(
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType()
    //     );
    // }
    //
    // [Benchmark]
    // public Type Switch_MethodReturningReferenceType_ThreeOptions_GetType()
    // {
    //     var oneOf = ReturnReferenceType_ThreeOptions();
    //     
    //     return oneOf.Value switch
    //     {
    //         Person value => value.GetType(),
    //         int value => value.GetType(),
    //         NotFound value => value.GetType(),
    //     };
    // }
    //
    // [Benchmark]
    // public Type Match_MethodReturningReferenceType_FiveOptions_GetType()
    // {
    //     var oneOf = ReturnReferenceType_FiveOptions();
    //     
    //     return oneOf.Match(
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType()
    //     );
    // }
    //
    // [Benchmark]
    // public Type Switch_MethodReturningReferenceType_FiveOptions_GetType()
    // {
    //     var oneOf = ReturnReferenceType_FiveOptions();
    //     
    //     return oneOf.Value switch
    //     {
    //         Person value => value.GetType(),
    //         int value => value.GetType(),
    //         NotFound value => value.GetType(),
    //         Error value => value.GetType(),
    //         Enum value => value.GetType(),
    //     };
    // }
    //
    // [Benchmark]
    // public Type Match_MethodReturningReferenceType_SevenOptions_GetType()
    // {
    //     var oneOf = ReturnReferenceType_SevenOptions();
    //
    //     return oneOf.Match(
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType()
    //     );
    // }
    //
    // [Benchmark]
    // public Type Switch_MethodReturningReferenceType_SevenOptions_GetType()
    // {
    //     var oneOf = ReturnReferenceType_SevenOptions();
    //     
    //     return oneOf.Value switch
    //     {
    //         Person value => value.GetType(),
    //         int value => value.GetType(),
    //         NotFound value => value.GetType(),
    //         Error value => value.GetType(),
    //         Enum value => value.GetType(),
    //         float value => value.GetType(),
    //         long value => value.GetType(),
    //     };
    // }
    //
    //
    //
    //
    // [Benchmark]
    // public Type Match_MethodReturningValueType_OneOption_GetType()
    // {
    //     var oneOf = ReturnValueType_OneOption();
    //     
    //     return oneOf.Match(
    //         value => value.GetType()
    //     );
    // }
    //
    // [Benchmark]
    // public Type Switch_MethodReturningValueType_OneOption_GetType()
    // {
    //     var oneOf = ReturnValueType_OneOption();
    //     
    //     return oneOf.Value switch
    //     {
    //         NotFound value => value.GetType(),
    //     };
    // }
    //
    // [Benchmark]
    // public Type Match_MethodReturningValueType_ThreeOptions_GetType()
    // {
    //     var oneOf = ReturnValueType_ThreeOptions();
    //     
    //     return oneOf.Match(
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType()
    //     );
    // }
    //
    // [Benchmark]
    // public Type Switch_MethodReturningValueType_ThreeOptions_GetType()
    // {
    //     var oneOf = ReturnValueType_ThreeOptions();
    //     
    //     return oneOf.Value switch
    //     {
    //         Person value => value.GetType(),
    //         int value => value.GetType(),
    //         NotFound value => value.GetType(),
    //     };
    // }
    //
    // [Benchmark]
    // public Type Match_MethodReturningValueType_FiveOptions_GetType()
    // {
    //     var oneOf = ReturnValueType_FiveOptions();
    //     
    //     return oneOf.Match(
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType()
    //     );
    // }
    //
    // [Benchmark]
    // public Type Switch_MethodReturningValueType_FiveOptions_GetType()
    // {
    //     var oneOf = ReturnValueType_FiveOptions();
    //     
    //     return oneOf.Value switch
    //     {
    //         Person value => value.GetType(),
    //         int value => value.GetType(),
    //         NotFound value => value.GetType(),
    //         Error value => value.GetType(),
    //         Enum value => value.GetType(),
    //     };
    // }
    //
    // [Benchmark]
    // public Type Match_MethodReturningValueType_SevenOptions_GetType()
    // {
    //     var oneOf = ReturnValueType_SevenOptions();
    //
    //     return oneOf.Match(
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType()
    //     );
    // }
    //
    // [Benchmark]
    // public Type Switch_MethodReturningValueType_SevenOptions_GetType()
    // {
    //     var oneOf = ReturnValueType_SevenOptions();
    //     
    //     return oneOf.Value switch
    //     {
    //         Person value => value.GetType(),
    //         int value => value.GetType(),
    //         NotFound value => value.GetType(),
    //         Error value => value.GetType(),
    //         Enum value => value.GetType(),
    //         float value => value.GetType(),
    //         long value => value.GetType(),
    //     };
    // }
    //
    //
    // [Benchmark]
    // public Type Match_LocalValueType_OneOption_GetType()
    // {
    //     OneOf<NotFound> oneOf = new NotFound();
    //     
    //     return oneOf.Match(
    //         value => value.GetType()
    //     );
    // }
    //
    // [Benchmark]
    // public Type Switch_LocalValueType_OneOption_GetType()
    // {
    //     OneOf<NotFound> oneOf = new NotFound();
    //     
    //     return oneOf.Value switch
    //     {
    //         NotFound value => value.GetType(),
    //     };
    // }
    //
    // [Benchmark]
    // public Type Match_LocalValueType_ThreeOptions_GetType()
    // {
    //     OneOf<Person, int, NotFound> oneOf = new NotFound();
    //     
    //     return oneOf.Match(
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType()
    //     );
    // }
    //
    // [Benchmark]
    // public Type Switch_LocalValueType_ThreeOptions_GetType()
    // {
    //     OneOf<Person, int, NotFound> oneOf = new NotFound();
    //     
    //     return oneOf.Value switch
    //     {
    //         Person value => value.GetType(),
    //         int value => value.GetType(),
    //         NotFound value => value.GetType(),
    //     };
    // }
    //
    // [Benchmark]
    // public Type Match_LocalValueType_FiveOptions_GetType()
    // {
    //     OneOf<Person, int, NotFound, Error, Enum> oneOf = new NotFound();
    //     
    //     return oneOf.Match(
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType()
    //     );
    // }
    //
    // [Benchmark]
    // public Type Switch_LocalValueType_FiveOptions_GetType()
    // {
    //     OneOf<Person, int, NotFound, Error, Enum> oneOf = new NotFound();
    //     
    //     return oneOf.Value switch
    //     {
    //         Person value => value.GetType(),
    //         int value => value.GetType(),
    //         NotFound value => value.GetType(),
    //         Error value => value.GetType(),
    //         Enum value => value.GetType(),
    //     };
    // }
    //
    // [Benchmark]
    // public Type Match_LocalValueType_SevenOptions_GetType()
    // {
    //     OneOf<Person, int, NotFound, Error, Enum, float, long> oneOf = new NotFound();
    //
    //     return oneOf.Match(
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType(),
    //         value => value.GetType()
    //     );
    // }
    //
    // [Benchmark]
    // public Type Switch_LocalValueType_SevenOptions_GetType()
    // {
    //     OneOf<Person, int, NotFound, Error, Enum, float, long> oneOf = new NotFound();
    //     
    //     return oneOf.Value switch
    //     {
    //         Person value => value.GetType(),
    //         int value => value.GetType(),
    //         NotFound value => value.GetType(),
    //         Error value => value.GetType(),
    //         Enum value => value.GetType(),
    //         float value => value.GetType(),
    //         long value => value.GetType(),
    //     };
    // }


    [Benchmark]
    public void DoesItActuallyMatter_Switch()
    {
        OneOf<Person, int, NotFound, Error, Enum, float, long> oneOf = new NotFound();

        var format = (oneOf.Value switch
        {
            Person value => value.GetType(),
            int value => value.GetType(),
            NotFound value => value.GetType(),
            Error value => value.GetType(),
            Enum value => value.GetType(),
            float value => value.GetType(),
            long value => value.GetType(),
        }).ToString();
        Console.WriteLine(format);
    }
    
    [Benchmark]
    public void DoesItActuallyMatter_Match()
    {
        OneOf<Person, int, NotFound, Error, Enum, float, long> oneOf = new NotFound();

        var format = (oneOf.Match(
            value => value.GetType(),
            value => value.GetType(),
            value => value.GetType(),
            value => value.GetType(),
            value => value.GetType(),
            value => value.GetType(),
            value => value.GetType())).ToString();
        Console.WriteLine(format);
    }
}

internal sealed class Person;

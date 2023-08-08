using BenchmarkDotNet.Attributes;

namespace MicroBenchmarks;

[MemoryDiagnoser(false)]
public class StringComparisonComparisons
{
    const string a = "fOo";
    const string b = "bAr";
    
    [Benchmark]
    public bool StringEquals_Ordinal()
    {
        return string.Equals(a, b, StringComparison.Ordinal);
    }
    
    [Benchmark]
    public bool StringEquals_Invariant()
    {
        return string.Equals(a, b, StringComparison.InvariantCulture);
    }
    
    [Benchmark]
    public bool StringEquals_OrdinalIgnoreCase()
    {
        return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
    }
    
    [Benchmark]
    public bool StringEquals_InvariantIgnoreCase()
    {
        return string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);
    }
    
    [Benchmark]
    public bool EqualsMethod_Ordinal()
    {
        return a.Equals(b, StringComparison.Ordinal);
    }
    
    [Benchmark]
    public bool EqualsMethod_Invariant()
    {
        return a.Equals(b, StringComparison.InvariantCulture);
    }
    
    [Benchmark]
    public bool EqualsMethod_OrdinalIgnoreCase()
    {
        return a.Equals(b, StringComparison.OrdinalIgnoreCase);
    }
    
    [Benchmark]
    public bool EqualsMethod_InvariantIgnoreCase()
    {
        return a.Equals(b, StringComparison.InvariantCultureIgnoreCase);
    }
    
    // Do not ever use this
    [Benchmark]
    public bool ToUpperOperator_Current()
    {
        return a.ToUpper() == b.ToUpper();
    }
    
    // Do not ever use this
    [Benchmark]
    public bool ToLowerOperator_Current()
    {
        return a.ToLower() == b.ToLower();
    }
    
    [Benchmark]
    public bool ToUpperOperator_Invariant()
    {
        return a.ToUpperInvariant() == b.ToUpperInvariant();
    }
    
    [Benchmark]
    public bool ToLowerOperator_Invariant()
    {
        return a.ToLowerInvariant() == b.ToLowerInvariant();
    }
}

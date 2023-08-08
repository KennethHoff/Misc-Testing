```

BenchmarkDotNet v0.13.7, openSUSE Tumbleweed
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK 8.0.100-preview.6.23330.14
  [Host]     : .NET 8.0.0 (8.0.23.32907), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.0 (8.0.23.32907), X64 RyuJIT AVX2


```
|                           Method |      Mean |     Error |    StdDev | Allocated |
|--------------------------------- |----------:|----------:|----------:|----------:|
|             StringEquals_Ordinal |  2.828 ns | 0.0492 ns | 0.0436 ns |         - |
|           StringEquals_Invariant | 34.821 ns | 0.4267 ns | 0.3782 ns |         - |
|   StringEquals_OrdinalIgnoreCase |  3.589 ns | 0.0429 ns | 0.0402 ns |         - |
| StringEquals_InvariantIgnoreCase | 34.861 ns | 0.2335 ns | 0.2184 ns |         - |
|             EqualsMethod_Ordinal |  2.792 ns | 0.0604 ns | 0.0565 ns |         - |
|           EqualsMethod_Invariant | 35.074 ns | 0.6064 ns | 0.5673 ns |         - |
|   EqualsMethod_OrdinalIgnoreCase |  3.404 ns | 0.0490 ns | 0.0459 ns |         - |
| EqualsMethod_InvariantIgnoreCase | 34.871 ns | 0.6046 ns | 0.5655 ns |         - |
|          ToUpperOperator_Current | 52.114 ns | 0.8017 ns | 0.7107 ns |      64 B |
|          ToLowerOperator_Current | 50.728 ns | 0.5077 ns | 0.4501 ns |      64 B |
|        ToUpperOperator_Invariant | 35.644 ns | 0.5416 ns | 0.5066 ns |      64 B |
|        ToLowerOperator_Invariant | 36.297 ns | 0.6241 ns | 0.5837 ns |      64 B |

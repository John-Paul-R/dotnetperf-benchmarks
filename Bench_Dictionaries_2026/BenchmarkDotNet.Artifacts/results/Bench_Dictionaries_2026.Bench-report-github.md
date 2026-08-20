```

BenchmarkDotNet v0.15.8, Linux Arch Linux
AMD Ryzen 7 5800H with Radeon Graphics 1.10GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3


```
| Method               | MaxItems | Mean       | Error     | StdDev    |
|--------------------- |--------- |-----------:|----------:|----------:|
| **BaseDictionary**       | **100**      |   **5.217 μs** | **0.0209 μs** | **0.0185 μs** |
| ConcurrentDictionary | 100      |   4.371 μs | 0.0385 μs | 0.0341 μs |
| ImmutableDictionaryT | 100      |   9.034 μs | 0.0286 μs | 0.0223 μs |
| FrozenDictionaryT    | 100      |   5.680 μs | 0.0179 μs | 0.0168 μs |
| **BaseDictionary**       | **1000**     |   **7.142 μs** | **0.0311 μs** | **0.0276 μs** |
| ConcurrentDictionary | 1000     |   7.195 μs | 0.0249 μs | 0.0233 μs |
| ImmutableDictionaryT | 1000     |  13.516 μs | 0.0759 μs | 0.0634 μs |
| FrozenDictionaryT    | 1000     |   7.948 μs | 0.0404 μs | 0.0378 μs |
| **BaseDictionary**       | **10000**    |  **10.571 μs** | **0.0580 μs** | **0.0543 μs** |
| ConcurrentDictionary | 10000    |  11.513 μs | 0.2249 μs | 0.2104 μs |
| ImmutableDictionaryT | 10000    |  32.629 μs | 0.6106 μs | 0.8358 μs |
| FrozenDictionaryT    | 10000    |  11.345 μs | 0.1089 μs | 0.0966 μs |
| **BaseDictionary**       | **100000**   |  **12.106 μs** | **0.0560 μs** | **0.0524 μs** |
| ConcurrentDictionary | 100000   |  14.333 μs | 0.1443 μs | 0.1279 μs |
| ImmutableDictionaryT | 100000   | 107.136 μs | 2.1408 μs | 3.5174 μs |
| FrozenDictionaryT    | 100000   |  15.155 μs | 0.2290 μs | 0.1912 μs |

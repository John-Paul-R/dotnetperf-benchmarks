```

BenchmarkDotNet v0.15.8, Linux Arch Linux
AMD Ryzen 7 5800H with Radeon Graphics 1.10GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.302
  [Host]    : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3


```
| Method               | Job       | Runtime   | MaxItems | Mean       | Error     | StdDev    |
|--------------------- |---------- |---------- |--------- |-----------:|----------:|----------:|
| **BaseDictionary**       | **.NET 10.0** | **.NET 10.0** | **100**      |   **4.843 μs** | **0.0255 μs** | **0.0250 μs** |
| ConcurrentDictionary | .NET 10.0 | .NET 10.0 | 100      |   4.397 μs | 0.0324 μs | 0.0270 μs |
| ImmutableDictionaryT | .NET 10.0 | .NET 10.0 | 100      |   9.045 μs | 0.0289 μs | 0.0256 μs |
| FrozenDictionaryT    | .NET 10.0 | .NET 10.0 | 100      |   5.645 μs | 0.0286 μs | 0.0223 μs |
| BaseDictionary       | .NET 6.0  | .NET 6.0  | 100      |         NA |        NA |        NA |
| ConcurrentDictionary | .NET 6.0  | .NET 6.0  | 100      |         NA |        NA |        NA |
| ImmutableDictionaryT | .NET 6.0  | .NET 6.0  | 100      |         NA |        NA |        NA |
| FrozenDictionaryT    | .NET 6.0  | .NET 6.0  | 100      |         NA |        NA |        NA |
| **BaseDictionary**       | **.NET 10.0** | **.NET 10.0** | **1000**     |   **6.820 μs** | **0.0190 μs** | **0.0159 μs** |
| ConcurrentDictionary | .NET 10.0 | .NET 10.0 | 1000     |   7.213 μs | 0.0270 μs | 0.0239 μs |
| ImmutableDictionaryT | .NET 10.0 | .NET 10.0 | 1000     |  14.332 μs | 0.2587 μs | 0.3951 μs |
| FrozenDictionaryT    | .NET 10.0 | .NET 10.0 | 1000     |   7.910 μs | 0.0290 μs | 0.0257 μs |
| BaseDictionary       | .NET 6.0  | .NET 6.0  | 1000     |         NA |        NA |        NA |
| ConcurrentDictionary | .NET 6.0  | .NET 6.0  | 1000     |         NA |        NA |        NA |
| ImmutableDictionaryT | .NET 6.0  | .NET 6.0  | 1000     |         NA |        NA |        NA |
| FrozenDictionaryT    | .NET 6.0  | .NET 6.0  | 1000     |         NA |        NA |        NA |
| **BaseDictionary**       | **.NET 10.0** | **.NET 10.0** | **10000**    |  **10.257 μs** | **0.0523 μs** | **0.0437 μs** |
| ConcurrentDictionary | .NET 10.0 | .NET 10.0 | 10000    |  12.021 μs | 0.2334 μs | 0.2952 μs |
| ImmutableDictionaryT | .NET 10.0 | .NET 10.0 | 10000    |  31.968 μs | 0.6209 μs | 0.8074 μs |
| FrozenDictionaryT    | .NET 10.0 | .NET 10.0 | 10000    |  10.941 μs | 0.0518 μs | 0.0432 μs |
| BaseDictionary       | .NET 6.0  | .NET 6.0  | 10000    |         NA |        NA |        NA |
| ConcurrentDictionary | .NET 6.0  | .NET 6.0  | 10000    |         NA |        NA |        NA |
| ImmutableDictionaryT | .NET 6.0  | .NET 6.0  | 10000    |         NA |        NA |        NA |
| FrozenDictionaryT    | .NET 6.0  | .NET 6.0  | 10000    |         NA |        NA |        NA |
| **BaseDictionary**       | **.NET 10.0** | **.NET 10.0** | **100000**   |  **12.115 μs** | **0.2333 μs** | **0.3419 μs** |
| ConcurrentDictionary | .NET 10.0 | .NET 10.0 | 100000   |  14.333 μs | 0.0934 μs | 0.0873 μs |
| ImmutableDictionaryT | .NET 10.0 | .NET 10.0 | 100000   | 104.273 μs | 1.6923 μs | 1.5002 μs |
| FrozenDictionaryT    | .NET 10.0 | .NET 10.0 | 100000   |  15.094 μs | 0.0806 μs | 0.0630 μs |
| BaseDictionary       | .NET 6.0  | .NET 6.0  | 100000   |         NA |        NA |        NA |
| ConcurrentDictionary | .NET 6.0  | .NET 6.0  | 100000   |         NA |        NA |        NA |
| ImmutableDictionaryT | .NET 6.0  | .NET 6.0  | 100000   |         NA |        NA |        NA |
| FrozenDictionaryT    | .NET 6.0  | .NET 6.0  | 100000   |         NA |        NA |        NA |

Benchmarks with issues:
  Bench.BaseDictionary: .NET 6.0(Runtime=.NET 6.0) [MaxItems=100]
  Bench.ConcurrentDictionary: .NET 6.0(Runtime=.NET 6.0) [MaxItems=100]
  Bench.ImmutableDictionaryT: .NET 6.0(Runtime=.NET 6.0) [MaxItems=100]
  Bench.FrozenDictionaryT: .NET 6.0(Runtime=.NET 6.0) [MaxItems=100]
  Bench.BaseDictionary: .NET 6.0(Runtime=.NET 6.0) [MaxItems=1000]
  Bench.ConcurrentDictionary: .NET 6.0(Runtime=.NET 6.0) [MaxItems=1000]
  Bench.ImmutableDictionaryT: .NET 6.0(Runtime=.NET 6.0) [MaxItems=1000]
  Bench.FrozenDictionaryT: .NET 6.0(Runtime=.NET 6.0) [MaxItems=1000]
  Bench.BaseDictionary: .NET 6.0(Runtime=.NET 6.0) [MaxItems=10000]
  Bench.ConcurrentDictionary: .NET 6.0(Runtime=.NET 6.0) [MaxItems=10000]
  Bench.ImmutableDictionaryT: .NET 6.0(Runtime=.NET 6.0) [MaxItems=10000]
  Bench.FrozenDictionaryT: .NET 6.0(Runtime=.NET 6.0) [MaxItems=10000]
  Bench.BaseDictionary: .NET 6.0(Runtime=.NET 6.0) [MaxItems=100000]
  Bench.ConcurrentDictionary: .NET 6.0(Runtime=.NET 6.0) [MaxItems=100000]
  Bench.ImmutableDictionaryT: .NET 6.0(Runtime=.NET 6.0) [MaxItems=100000]
  Bench.FrozenDictionaryT: .NET 6.0(Runtime=.NET 6.0) [MaxItems=100000]

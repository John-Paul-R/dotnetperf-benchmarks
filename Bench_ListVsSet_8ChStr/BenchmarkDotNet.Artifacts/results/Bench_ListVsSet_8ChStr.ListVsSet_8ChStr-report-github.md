``` ini

BenchmarkDotNet=v0.13.1, OS=arch 
AMD Ryzen 7 5800H with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK=10.0.302
  [Host]   : .NET 6.0.36 (6.0.3625.6901), X64 RyuJIT
  .NET 6.0 : .NET 6.0.36 (6.0.3625.6901), X64 RyuJIT

Job=.NET 6.0  Runtime=.NET 6.0  

```
|            Method | MaxItems |        Mean |    Error |   StdDev |
|------------------ |--------- |------------:|---------:|---------:|
|      **ListContains** |        **4** |    **18.42 μs** | **0.354 μs** | **0.347 μs** |
|   HashSetContains |        4 |    11.19 μs | 0.223 μs | 0.229 μs |
| SortedSetContains |        4 |    51.17 μs | 0.300 μs | 0.281 μs |
|      **ListContains** |        **8** |    **31.23 μs** | **0.158 μs** | **0.148 μs** |
|   HashSetContains |        8 |    10.86 μs | 0.063 μs | 0.053 μs |
| SortedSetContains |        8 |    78.11 μs | 0.406 μs | 0.380 μs |
|      **ListContains** |       **16** |    **48.00 μs** | **0.484 μs** | **0.453 μs** |
|   HashSetContains |       16 |    11.71 μs | 0.142 μs | 0.133 μs |
| SortedSetContains |       16 |   110.00 μs | 0.489 μs | 0.458 μs |
|      **ListContains** |       **32** |    **94.74 μs** | **0.347 μs** | **0.290 μs** |
|   HashSetContains |       32 |    11.86 μs | 0.092 μs | 0.086 μs |
| SortedSetContains |       32 |   146.98 μs | 0.825 μs | 0.732 μs |
|      **ListContains** |       **64** |   **179.04 μs** | **0.585 μs** | **0.518 μs** |
|   HashSetContains |       64 |    10.64 μs | 0.087 μs | 0.082 μs |
| SortedSetContains |       64 |   183.46 μs | 1.009 μs | 0.944 μs |
|      **ListContains** |      **128** |   **302.81 μs** | **0.797 μs** | **0.707 μs** |
|   HashSetContains |      128 |    11.30 μs | 0.222 μs | 0.352 μs |
| SortedSetContains |      128 |   232.97 μs | 1.582 μs | 1.480 μs |
|      **ListContains** |      **256** |   **594.45 μs** | **1.673 μs** | **1.565 μs** |
|   HashSetContains |      256 |    10.52 μs | 0.088 μs | 0.078 μs |
| SortedSetContains |      256 |   306.75 μs | 1.245 μs | 1.165 μs |
|      **ListContains** |      **512** | **1,709.52 μs** | **7.219 μs** | **6.753 μs** |
|   HashSetContains |      512 |    10.49 μs | 0.043 μs | 0.038 μs |
| SortedSetContains |      512 |   331.51 μs | 2.469 μs | 2.309 μs |

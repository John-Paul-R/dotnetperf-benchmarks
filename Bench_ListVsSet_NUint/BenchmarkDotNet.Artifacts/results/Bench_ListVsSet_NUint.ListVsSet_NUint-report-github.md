``` ini

BenchmarkDotNet=v0.13.1, OS=arch 
AMD Ryzen 7 5800H with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK=10.0.302
  [Host]   : .NET 6.0.36 (6.0.3625.6901), X64 RyuJIT
  .NET 6.0 : .NET 6.0.36 (6.0.3625.6901), X64 RyuJIT

Job=.NET 6.0  Runtime=.NET 6.0  

```
|            Method | MaxItems |      Mean |     Error |    StdDev |
|------------------ |--------- |----------:|----------:|----------:|
|      **ListContains** |        **4** |  **4.637 μs** | **0.0143 μs** | **0.0127 μs** |
|   HashSetContains |        4 |  3.876 μs | 0.0093 μs | 0.0072 μs |
| SortedSetContains |        4 |  7.419 μs | 0.0231 μs | 0.0204 μs |
|      **ListContains** |        **8** |  **4.923 μs** | **0.0179 μs** | **0.0159 μs** |
|   HashSetContains |        8 |  3.841 μs | 0.0091 μs | 0.0085 μs |
| SortedSetContains |        8 |  9.399 μs | 0.0269 μs | 0.0251 μs |
|      **ListContains** |       **16** |  **5.731 μs** | **0.0081 μs** | **0.0071 μs** |
|   HashSetContains |       16 |  4.003 μs | 0.0111 μs | 0.0099 μs |
| SortedSetContains |       16 | 11.480 μs | 0.0170 μs | 0.0151 μs |
|      **ListContains** |       **32** |  **7.394 μs** | **0.0252 μs** | **0.0224 μs** |
|   HashSetContains |       32 |  4.037 μs | 0.0163 μs | 0.0152 μs |
| SortedSetContains |       32 | 13.369 μs | 0.0330 μs | 0.0308 μs |
|      **ListContains** |       **64** | **11.594 μs** | **0.0294 μs** | **0.0261 μs** |
|   HashSetContains |       64 |  4.076 μs | 0.0147 μs | 0.0138 μs |
| SortedSetContains |       64 | 16.765 μs | 0.0608 μs | 0.0569 μs |
|      **ListContains** |      **128** | **19.711 μs** | **0.0545 μs** | **0.0483 μs** |
|   HashSetContains |      128 |  4.003 μs | 0.0059 μs | 0.0053 μs |
| SortedSetContains |      128 | 18.504 μs | 0.0458 μs | 0.0383 μs |
|      **ListContains** |      **256** | **31.971 μs** | **0.1334 μs** | **0.1114 μs** |
|   HashSetContains |      256 |  4.008 μs | 0.0144 μs | 0.0120 μs |
| SortedSetContains |      256 | 22.503 μs | 0.0386 μs | 0.0361 μs |
|      **ListContains** |      **512** | **54.650 μs** | **0.2174 μs** | **0.1927 μs** |
|   HashSetContains |      512 |  4.055 μs | 0.0091 μs | 0.0076 μs |
| SortedSetContains |      512 | 25.262 μs | 0.1002 μs | 0.0889 μs |

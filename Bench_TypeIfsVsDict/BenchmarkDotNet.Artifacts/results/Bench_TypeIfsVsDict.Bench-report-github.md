```

BenchmarkDotNet v0.14.0, Arch Linux
AMD Ryzen 7 5800H with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.1026.32716), X64 RyuJIT AVX2
  DefaultJob : .NET 10.0.10 (10.0.1026.32716), X64 RyuJIT AVX2


```
| Method | N   | Mean     | Error     | StdDev    | Gen0   | Allocated |
|------- |---- |---------:|----------:|----------:|-------:|----------:|
| Ifs    | 200 | 2.471 μs | 0.0547 μs | 0.1596 μs | 0.0763 |     640 B |
| Dict   | 200 | 3.420 μs | 0.1083 μs | 0.3175 μs | 0.0763 |     640 B |

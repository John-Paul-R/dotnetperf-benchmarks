```

BenchmarkDotNet v0.15.8, Linux Arch Linux
AMD Ryzen 7 5800H with Radeon Graphics 1.10GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3


```
| Method   | N   | Mean     | Error     | StdDev    | Gen0   | Allocated |
|--------- |---- |---------:|----------:|----------:|-------:|----------:|
| Ifs      | 200 | 2.115 μs | 0.0264 μs | 0.0234 μs | 0.0763 |     640 B |
| Dict     | 200 | 3.126 μs | 0.0118 μs | 0.0099 μs | 0.0763 |     640 B |
| ListFind | 200 | 7.680 μs | 0.0965 μs | 0.0903 μs | 2.1744 |   18241 B |

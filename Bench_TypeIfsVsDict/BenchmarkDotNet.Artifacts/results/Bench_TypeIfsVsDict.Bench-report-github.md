```

BenchmarkDotNet v0.15.8, Linux Arch Linux
AMD Ryzen 7 5800H with Radeon Graphics 1.10GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v3


```
| Method   | N   | Mean     | Error     | StdDev    | Gen0   | Allocated |
|--------- |---- |---------:|----------:|----------:|-------:|----------:|
| Ifs      | 200 | 2.128 μs | 0.0236 μs | 0.0197 μs | 0.0763 |     640 B |
| Dict     | 200 | 3.009 μs | 0.0575 μs | 0.0538 μs | 0.0763 |     640 B |
| ListFind | 200 | 7.666 μs | 0.1343 μs | 0.1190 μs | 2.1667 |   18241 B |
| ListLoop | 200 | 4.268 μs | 0.0570 μs | 0.0506 μs | 0.0763 |     640 B |

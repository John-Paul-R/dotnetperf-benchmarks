namespace FakeConcurrent;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
// // * Summary *
//
// BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1645 (21H2)
// AMD Ryzen Threadripper 2920X, 1 CPU, 24 logical and 12 physical cores
// .NET SDK=6.0.100
//   [Host]     : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
//   DefaultJob : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
//
//
// |         Method | TaskCount |        Mean |      Error |     StdDev |  Gen 0 | Allocated |
// |--------------- |---------- |------------:|-----------:|-----------:|-------:|----------:|
// |  WhenAllResult |         8 |   180.95 ns |   2.879 ns |   2.404 ns | 0.0553 |     232 B |
// |    AwaitSerial |         8 |    95.02 ns |   1.302 ns |   1.016 ns | 0.0305 |     128 B |
// | FullSyncSerial |         8 |    22.06 ns |   0.465 ns |   0.554 ns | 0.0134 |      56 B |
// |  WhenAllResult |        64 | 1,070.65 ns |  11.731 ns |  10.399 ns | 0.2155 |     904 B |
// |    AwaitSerial |        64 |   405.37 ns |   7.961 ns |   7.447 ns | 0.0839 |     352 B |
// | FullSyncSerial |        64 |   131.70 ns |   1.610 ns |   1.506 ns | 0.0669 |     280 B |
// |  WhenAllResult |       512 | 8,354.64 ns | 105.293 ns |  98.491 ns | 1.4954 |   6,280 B |
// |    AwaitSerial |       512 | 2,930.98 ns |  58.189 ns | 107.856 ns | 0.5112 |   2,144 B |
// | FullSyncSerial |       512 |   919.68 ns |  14.540 ns |  13.601 ns | 0.4950 |   2,072 B |
//
// // * Hints *
// Outliers
//   Bench.WhenAllResult: Default  -> 2 outliers were removed, 3 outliers were detected (180.01 ns, 191.36 ns, 194.36 ns)
//   Bench.AwaitSerial: Default    -> 3 outliers were removed, 4 outliers were detected (95.70 ns, 100.98 ns..104.75 ns)
//   Bench.FullSyncSerial: Default -> 2 outliers were removed (27.10 ns, 29.01 ns)
//   Bench.WhenAllResult: Default  -> 1 outlier  was  removed (1.11 us)
//   Bench.WhenAllResult: Default  -> 1 outlier  was  detected (8.16 us)
//   Bench.AwaitSerial: Default    -> 2 outliers were removed (3.23 us, 3.30 us)
//   Bench.FullSyncSerial: Default -> 1 outlier  was  detected (895.51 ns)
//
// // * Legends *
//   TaskCount : Value of the 'TaskCount' parameter
//   Mean      : Arithmetic mean of all measurements
//   Error     : Half of 99.9% confidence interval
//   StdDev    : Standard deviation of all measurements
//   Gen 0     : GC Generation 0 collects per 1000 operations
//   Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  1 ns      : 1 Nanosecond (0.000000001 sec)

class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<Bench>();
    }
}

[MemoryDiagnoser]
public class Bench
{
    private const int RandSeed = 826528;

    [Params(8, 64, 512)]
    public static int TaskCount { get; set; }

    private Task<int>[] Tasks; 

    [GlobalSetup]
    public void GlobalSetup()
    {
        Tasks = Enumerable.Range(0, TaskCount)
            .Select(num => Task.FromResult(num))
            .ToArray();
    }

    [Benchmark]
    public int[] WhenAllResult() => Task.WhenAll(Tasks).Result;

    [Benchmark]
    public async Task<int[]> AwaitSerial()
    {
        int[] results = new int[TaskCount];
        for (int i = 0; i < TaskCount; i++) {
            results[i] = await Tasks[i];
        }

        return results;
    }

    [Benchmark]
    public int[] FullSyncSerial()
    {
        int[] results = new int[TaskCount];
        for (int i = 0; i < TaskCount; i++) {
            results[i] = Tasks[i].Result;
        }

        return results;
    }
}

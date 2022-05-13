namespace FakeConcurrentDelay;
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
// |         Method | TaskCount |        Mean |     Error |   StdDev |      Median |   Gen 0 | Allocated |
// |--------------- |---------- |------------:|----------:|---------:|------------:|--------:|----------:|
// |  WhenAllResult |         8 |    15.60 ms |  0.119 ms | 0.111 ms |    15.69 ms |       - |      3 KB |
// |    AwaitSerial |         8 |   125.16 ms |  1.283 ms | 1.200 ms |   125.63 ms |       - |      4 KB |
// | FullSyncSerial |         8 |   125.02 ms |  1.480 ms | 1.385 ms |   125.83 ms |       - |      4 KB |
// |  WhenAllResult |        64 |    15.63 ms |  0.262 ms | 0.245 ms |    15.45 ms |       - |     24 KB |
// |    AwaitSerial |        64 |   999.11 ms | 10.115 ms | 9.462 ms | 1,001.23 ms |       - |     29 KB |
// | FullSyncSerial |        64 |   999.70 ms |  8.434 ms | 7.889 ms | 1,002.57 ms |       - |     32 KB |
// |  WhenAllResult |       512 |    15.70 ms |  0.309 ms | 0.356 ms |    15.46 ms | 31.2500 |    190 KB |
// |    AwaitSerial |       512 | 7,999.90 ms |  6.320 ms | 5.912 ms | 7,998.23 ms |       - |    195 KB |
// | FullSyncSerial |       512 | 8,000.00 ms |  7.346 ms | 6.872 ms | 8,000.52 ms |       - |    230 KB |
//
// // * Hints *
// Outliers
//   Bench.AwaitSerial: Default -> 3 outliers were detected (122.75 ms..123.17 ms)
//
// // * Legends *
//   TaskCount : Value of the 'TaskCount' parameter
//   Mean      : Arithmetic mean of all measurements
//   Error     : Half of 99.9% confidence interval
//   StdDev    : Standard deviation of all measurements
//   Median    : Value separating the higher half of all measurements (50th percentile)
//   Gen 0     : GC Generation 0 collects per 1000 operations
//   Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
//   1 ms      : 1 Millisecond (0.001 sec)

// Run time: 00:08:46 (526.2 sec), executed benchmarks: 9

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

    private Func<Task<int>>[] FakeAsyncFuncs;

    [GlobalSetup]
    public void GlobalSetup()
    {
        FakeAsyncFuncs = Enumerable.Range(0, TaskCount)
            .Select(num => () => Task.Delay(millisecondsDelay: 1).ContinueWith((_) => num))
            .ToArray();
    }

    [Benchmark]
    public int[] WhenAllResult() => Task.WhenAll(FakeAsyncFuncs.Select(f => f.Invoke())).Result;

    [Benchmark]
    public async Task<int[]> AwaitSerial()
    {
        int[] results = new int[TaskCount];
        for (int i = 0; i < TaskCount; i++) {
            results[i] = await FakeAsyncFuncs[i].Invoke();
        }

        return results;
    }

    [Benchmark]
    public int[] FullSyncSerial()
    {
        int[] results = new int[TaskCount];
        for (int i = 0; i < TaskCount; i++) {
            results[i] = FakeAsyncFuncs[i].Invoke().Result;
        }

        return results;
    }
}

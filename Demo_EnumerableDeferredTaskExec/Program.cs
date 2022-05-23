namespace Demo_EnumerableDeferredTaskExec;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

// // * Summary *
//
// BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1645 (21H2)
// AMD Ryzen Threadripper 2920X, 1 CPU, 24 logical and 12 physical cores
//     .NET SDK=6.0.100
//     [Host]     : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
// Job-TZRWKD : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
//
//     InvocationCount=1  UnrollFactor=1  
//
//     |         Method | TaskCount |     Mean |    Error |   StdDev | Allocated |
//     |--------------- |---------- |---------:|---------:|---------:|----------:|
//     | FullConcurrent |       512 | 13.88 ms | 0.272 ms | 0.334 ms |    204 KB |
//     |        Batched |       512 | 58.76 ms | 0.587 ms | 0.490 ms |    200 KB |
//
// // * Warnings *
//     MinIterationTime
// Bench.FullConcurrent: InvocationCount=1, UnrollFactor=1 -> The minimum observed iteration time is 13.3797 ms which is very small. It's recommended to increase it to at least 100.0000 ms using more operati
// ons.
//     Bench.Batched: InvocationCount=1, UnrollFactor=1        -> The minimum observed iteration time is 57.7780 ms which is very small. It's recommended to increase it to at least 100.0000 ms using more operati
// ons.
//
// // * Hints *
//     Outliers
// Bench.FullConcurrent: InvocationCount=1, UnrollFactor=1 -> 3 outliers were removed (25.54 ms..28.92 ms)
// Bench.Batched: InvocationCount=1, UnrollFactor=1        -> 4 outliers were removed (73.47 ms..74.37 ms)
//
// // * Legends *
// TaskCount : Value of the 'TaskCount' parameter
// Mean      : Arithmetic mean of all measurements
// Error     : Half of 99.9% confidence interval
// StdDev    : Standard deviation of all measurements
// Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
// 1 ms      : 1 Millisecond (0.001 sec)


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
    [Params(512)]
    public static int TaskCount { get; set; }

    private IEnumerable<Task<int>> Tasks; 

    [IterationSetup]
    public void IterationSetup()
    {
        Tasks = Enumerable.Range(0, TaskCount)
            .Select(num => Task.Delay(1).ContinueWith((_) => num));
    }

    [Benchmark]
    public int[] FullConcurrent() => Task.WhenAll(Tasks.Select(f => f)).Result;

    // If deferred execution is happening, this should take approx 4x as long as `FullConcurrent`
    [Benchmark]
    public int[][] Batched()
    {
        var batchSize = TaskCount / 4;
        var batchCount = (TaskCount + (batchSize - 1)) / batchSize; // rounded up

        int[][] results = new int[batchCount][];
        var batches = Tasks.Select(f => f).Chunk(batchSize);
        int i = 0;
        foreach (var batch in batches) {
            results[i++] = Task.WhenAll(batch).Result;
        }

        return results;
    }

}

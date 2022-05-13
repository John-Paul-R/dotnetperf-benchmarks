namespace Demo_EnumerableDeferredTaskExec;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

// // * Summary *
//
// BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1645 (21H2)
// AMD Ryzen Threadripper 2920X, 1 CPU, 24 logical and 12 physical cores
//     .NET SDK=6.0.100
//     [Host]     : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
// Job-UOZRCX : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
//
//     InvocationCount=1  UnrollFactor=1  
//
//     |         Method | TaskCount |      Mean |    Error |   StdDev |    Median | Allocated |
//     |--------------- |---------- |----------:|---------:|---------:|----------:|----------:|
//     | FullConcurrent |       512 |  13.76 ms | 0.269 ms | 0.442 ms |  13.77 ms |    203 KB |
//     |        Batched |       512 | 123.80 ms | 2.501 ms | 7.376 ms | 118.69 ms |    201 KB |
//
// // * Warnings *
//     MultimodalDistribution
// Bench.Batched: InvocationCount=1, UnrollFactor=1 -> It seems that the distribution is bimodal (mValue = 3.23)
//     MinIterationTime
// Bench.FullConcurrent: InvocationCount=1, UnrollFactor=1 -> The minimum observed iteration time is 11.6975 ms which is very small. It's recommended to increase it to at least 100.0000 ms using more operati
// ons.
//
// // * Hints *
//     Outliers
// Bench.FullConcurrent: InvocationCount=1, UnrollFactor=1 -> 5 outliers were removed, 6 outliers were detected (11.70 ms, 15.08 ms..29.09 ms)
//
// // * Legends *
// TaskCount : Value of the 'TaskCount' parameter
// Mean      : Arithmetic mean of all measurements
// Error     : Half of 99.9% confidence interval
// StdDev    : Standard deviation of all measurements
// Median    : Value separating the higher half of all measurements (50th percentile)
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
    public void IteratioNSetup()
    {
        Tasks = Enumerable.Range(0, TaskCount)
            .Select(num => Task.Delay(1).ContinueWith((_) => num));
    }

    [Benchmark]
    public int[] FullConcurrent() => Task.WhenAll(Tasks.Select(f => f)).Result;

    // If deferred execution is happening, this should take approx 8x as long as `FullConcurrent`
    [Benchmark]
    public int[][] Batched()
    {
        var batchSize = TaskCount / 8;
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

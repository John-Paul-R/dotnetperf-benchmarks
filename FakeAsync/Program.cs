namespace FakeAsync;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
// // * Summary *
//
// BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1645 (21H2)
// AMD Ryzen Threadripper 2920X, 1 CPU, 24 logical and 12 physical cores
//     .NET SDK=6.0.100
//     [Host]     : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
// DefaultJob : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
//
//
//     |          Method |       Mean |     Error |    StdDev |     Median |  Gen 0 | Allocated |
//     |---------------- |-----------:|----------:|----------:|-----------:|-------:|----------:|
//     |           Async | 24.5213 ns | 0.5131 ns | 1.1044 ns | 24.4299 ns | 0.0172 |      72 B |
//     |      FromResult | 15.3571 ns | 0.3288 ns | 0.4500 ns | 15.3283 ns | 0.0172 |      72 B |
//     | FromResultAsync | 37.1894 ns | 0.7712 ns | 1.2232 ns | 36.9111 ns | 0.0344 |     144 B |
//     |            Sync |  0.0046 ns | 0.0069 ns | 0.0065 ns |  0.0005 ns |      - |         - |
//
// // * Warnings *
// ZeroMeasurement
// Bench.Sync: Default -> The method duration is indistinguishable from the empty method duration
//
// // * Hints *
// Outliers
// Bench.Async: Default      -> 2 outliers were removed (30.26 ns, 30.82 ns)
// Bench.FromResult: Default -> 2 outliers were removed (18.69 ns, 19.06 ns)
//
// // * Legends *
// Mean      : Arithmetic mean of all measurements
// Error     : Half of 99.9% confidence interval
// StdDev    : Standard deviation of all measurements
// Median    : Value separating the higher half of all measurements (50th percentile)
// Gen 0     : GC Generation 0 collects per 1000 operations
// Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
// 1 ns      : 1 Nanosecond (0.000000001 sec)

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

    [GlobalSetup]
    public void GlobalSetup()
    {
    }

    private static async Task<int> GetNumberAsync()
        => RandSeed;

    private static Task<int> GetNumberFromResult()
        => Task.FromResult(RandSeed);

    private static async Task<int> GetNumberFromResultAsync()
        => await Task.FromResult(RandSeed);

    private static int GetNumber()
        => RandSeed;

    [Benchmark]
    public int Async() => GetNumberAsync().Result;

    [Benchmark]
    public int FromResult() => GetNumberFromResult().Result;
    
    [Benchmark]
    public int FromResultAsync() => GetNumberFromResultAsync().Result;

    [Benchmark]
    public int Sync() => GetNumber();
}

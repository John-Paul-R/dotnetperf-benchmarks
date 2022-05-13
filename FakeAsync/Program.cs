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
//     |          Method |     Mean |    Error |   StdDev |  Gen 0 | Allocated |
//     |---------------- |---------:|---------:|---------:|-------:|----------:|
//     |           Async | 46.87 ns | 0.878 ns | 0.821 ns | 0.0344 |     144 B |
//     |      FromResult | 36.38 ns | 0.704 ns | 0.691 ns | 0.0344 |     144 B |
//     | FromResultAsync | 62.99 ns | 1.135 ns | 1.062 ns | 0.0516 |     216 B |
//     |            Sync | 22.50 ns | 0.471 ns | 0.524 ns | 0.0172 |      72 B |
//
// // * Legends *
//     Mean      : Arithmetic mean of all measurements
// Error     : Half of 99.9% confidence interval
// StdDev    : Standard deviation of all measurements
//     Gen 0     : GC Generation 0 collects per 1000 operations
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
    public async Task<int> Async() => await GetNumberAsync();

    [Benchmark]
    public async Task<int> FromResult() => await GetNumberFromResult();
    
    [Benchmark]
    public async Task<int> FromResultAsync() => await GetNumberFromResultAsync();

    [Benchmark]
    public async Task<int> Sync() => GetNumber();
}

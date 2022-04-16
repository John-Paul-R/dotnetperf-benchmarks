using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace ListRemoveElements;

// // * Summary *
//
// BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1586 (21H2)
// AMD Ryzen Threadripper 2920X, 1 CPU, 24 logical and 12 physical cores
// .NET SDK=6.0.100
//   [Host]     : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
//   DefaultJob : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
//
//
// |            Method | MaxItems |        Mean |     Error |    StdDev |      Median |  Gen 0 | Allocated |
// |------------------ |--------- |------------:|----------:|----------:|------------:|-------:|----------:|
// |    ListCopyRemove |        8 |    95.11 ns |  1.890 ns |  2.711 ns |    95.15 ns | 0.0229 |      96 B |
// |   ListIndexRemove |        8 |    46.09 ns |  0.686 ns |  0.608 ns |    45.98 ns |      - |         - |
// | ListClearAddRange |        8 |   230.89 ns |  3.263 ns |  2.548 ns |   231.45 ns | 0.0744 |     312 B |
// |     ListRemoveAll |        8 |    47.19 ns |  0.970 ns |  2.606 ns |    45.83 ns |      - |         - |
// |    ListCopyRemove |       64 |   443.06 ns |  5.175 ns |  4.588 ns |   441.27 ns | 0.0973 |     408 B |
// |   ListIndexRemove |       64 |   329.50 ns |  2.622 ns |  2.190 ns |   329.38 ns |      - |         - |
// | ListClearAddRange |       64 |   975.03 ns | 19.357 ns | 21.515 ns |   984.21 ns | 0.3052 |   1,280 B |
// |     ListRemoveAll |       64 |   308.13 ns |  1.449 ns |  1.210 ns |   308.10 ns |      - |         - |
// |    ListCopyRemove |      512 | 3,321.44 ns | 65.485 ns | 98.014 ns | 3,321.58 ns | 0.7019 |   2,944 B |
// |   ListIndexRemove |      512 | 2,647.81 ns | 17.974 ns | 16.812 ns | 2,641.72 ns |      - |         - |
// | ListClearAddRange |      512 | 5,602.77 ns | 69.852 ns | 65.340 ns | 5,595.23 ns | 2.0294 |   8,520 B |
// |     ListRemoveAll |      512 | 2,463.78 ns | 17.615 ns | 14.709 ns | 2,460.23 ns |      - |         - |
//
// // * Hints *
// Outliers
//   Bench.ListIndexRemove: Default   -> 1 outlier  was  removed (51.45 ns)
//   Bench.ListClearAddRange: Default -> 3 outliers were removed (243.35 ns..288.05 ns)
//   Bench.ListCopyRemove: Default    -> 1 outlier  was  removed (491.31 ns)
//   Bench.ListIndexRemove: Default   -> 2 outliers were removed (340.64 ns, 340.78 ns)
//   Bench.ListRemoveAll: Default     -> 2 outliers were removed (317.21 ns, 322.21 ns)
//   Bench.ListRemoveAll: Default     -> 2 outliers were removed (2.55 us, 2.56 us)
//
// // * Legends *
//   MaxItems  : Value of the 'MaxItems' parameter
//   Mean      : Arithmetic mean of all measurements
//   Error     : Half of 99.9% confidence interval
//   StdDev    : Standard deviation of all measurements
//   Median    : Value separating the higher half of all measurements (50th percentile)
//   Gen 0     : GC Generation 0 collects per 1000 operations
//   Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
//   1 ns      : 1 Nanosecond (0.000000001 sec)


class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<Bench>();
    }
}

// [SimpleJob(RuntimeMoniker.Net60)]
[MemoryDiagnoser]
public class Bench
{
    private const int RandSeed = 826528;
    private const float RemovePercent = 0.2f;

    [Params(8, 64, 512)]
    public int MaxItems { get; set; }

    private const int StrLen = 8;
    private const int BitCount = StrLen * 6;
    private const int ByteCount = (BitCount + 7) / 8; // rounded up

    private List<string> _list = null!;

        
    [GlobalSetup]
    public void GlobalSetup()
    {
        var rand = new Random(RandSeed);

        var randStrings = Enumerable.Range(0, MaxItems)
            .Select(_ =>
            {
                var bytes = new byte[ByteCount];
                rand.NextBytes(bytes);
                var randString = Convert.ToBase64String(bytes);
                    
                if (rand.NextDouble() < RemovePercent) {
                    return randString + 'a';
                }

                return randString;
            })
            .ToList();

        _list = randStrings.ToList();
    }

    private Predicate<string> RemoveCondition = (str) => str.Contains('a');
        
    // Note, we are intentionally modeling a case where the .Equals would
    // not work, (a condition must be run per-elem) despite using .Equals,
    // that is why we're not just iterating through _valuesToRemove
    [Benchmark]
    public void ListCopyRemove()
    {
        foreach (var value in _list.ToList()) {
            if (RemoveCondition(value)) {
                _list.Remove(value);
            }
        }
    }

    [Benchmark]
    public void ListIndexRemove()
    {
        for (int i = 0; i < _list.Count; i++) {
            if (RemoveCondition(_list[i])) {
                _list.RemoveAt(i);
            }
        }
    }

    // Doesn't work, as the underlying stream is modified.
    // [Benchmark]
    // public void ListRemoveAfter()
    // {
    //     using var test = _list.Select(v => v).GetEnumerator();
    //     while (test.MoveNext()) {
    //         if (RemoveCondition(test.Current)) {
    //             _list.Remove(test.Current);
    //         }
    //     }
    // }

    [Benchmark]
    public void ListClearAddRange()
    {
        var newList = _list.Where(val => !RemoveCondition(val)).ToList();
        _list.Clear();
        _list.AddRange(newList);
    }

    [Benchmark]
    public void ListRemoveAll()
    {
        _list.RemoveAll(RemoveCondition);
    }

}
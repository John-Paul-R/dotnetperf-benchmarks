using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Bench_CaseInsensitiveStringComparison;
// * Summary *

// BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1645 (21H2)
// AMD Ryzen Threadripper 2920X, 1 CPU, 24 logical and 12 physical cores
// .NET SDK=6.0.100
//   [Host]     : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
//   DefaultJob : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
//
//
// |                                        Method | StrLen |       Mean |      Error |     StdDev |     Median |  Gen 0 | Allocated |
// |---------------------------------------------- |------- |-----------:|-----------:|-----------:|-----------:|-------:|----------:|
// |                           ToLowerEq_NoneMatch |      8 |  45.468 ns |  0.3098 ns |  0.2898 ns |  45.459 ns | 0.0095 |      40 B |
// |            Equals_OrdinalIgnoreCase_NoneMatch |      8 |   4.554 ns |  0.0418 ns |  0.0370 ns |   4.544 ns |      - |         - |
// |                ToLowerEq_CaseInsensitiveMatch |      8 |  61.760 ns |  0.5026 ns |  0.4455 ns |  61.797 ns | 0.0191 |      80 B |
// | Equals_OrdinalIgnoreCase_CaseInsensitiveMatch |      8 |   6.115 ns |  0.0325 ns |  0.0272 ns |   6.122 ns |      - |         - |
// |                           ToLowerEq_NoneMatch |     64 |  99.867 ns |  1.4988 ns |  1.4020 ns |  99.488 ns | 0.0362 |     152 B |
// |            Equals_OrdinalIgnoreCase_NoneMatch |     64 |   4.584 ns |  0.0874 ns |  0.0817 ns |   4.550 ns |      - |         - |
// |                ToLowerEq_CaseInsensitiveMatch |     64 | 126.177 ns |  2.5313 ns |  2.5995 ns | 125.558 ns | 0.0726 |     304 B |
// | Equals_OrdinalIgnoreCase_CaseInsensitiveMatch |     64 |  26.359 ns |  0.5578 ns |  1.5734 ns |  25.375 ns |      - |         - |
// |                           ToLowerEq_NoneMatch |    512 | 593.493 ns | 11.8745 ns | 34.2607 ns | 573.245 ns | 0.2499 |   1,048 B |
// |            Equals_OrdinalIgnoreCase_NoneMatch |    512 |   4.588 ns |  0.0250 ns |  0.0222 ns |   4.583 ns |      - |         - |
// |                ToLowerEq_CaseInsensitiveMatch |    512 | 728.033 ns | 14.4712 ns | 33.2499 ns | 709.820 ns | 0.5007 |   2,096 B |
// | Equals_OrdinalIgnoreCase_CaseInsensitiveMatch |    512 | 170.633 ns |  1.7253 ns |  1.3470 ns | 170.938 ns |      - |         - |
//
// // * Warnings *
// MultimodalDistribution
//   Bench.ToLowerEq_NoneMatch: Default -> It seems that the distribution can have several modes (mValue = 2.84)
//
// // * Hints *
// Outliers
//   Bench.Equals_OrdinalIgnoreCase_NoneMatch: Default            -> 1 outlier  was  removed (7.25 ns)
//   Bench.ToLowerEq_CaseInsensitiveMatch: Default                -> 1 outlier  was  removed (64.39 ns)
//   Bench.Equals_OrdinalIgnoreCase_CaseInsensitiveMatch: Default -> 2 outliers were removed, 3 outliers were detected (7.61 ns, 7.79 ns, 8.45 ns)
//   Bench.ToLowerEq_NoneMatch: Default                           -> 4 outliers were removed (111.38 ns..120.56 ns)
//   Bench.ToLowerEq_CaseInsensitiveMatch: Default                -> 1 outlier  was  removed (150.07 ns)
//   Bench.ToLowerEq_NoneMatch: Default                           -> 1 outlier  was  removed (764.92 ns)
//   Bench.Equals_OrdinalIgnoreCase_NoneMatch: Default            -> 1 outlier  was  removed (6.24 ns)
//   Bench.ToLowerEq_CaseInsensitiveMatch: Default                -> 1 outlier  was  removed (949.04 ns)
//   Bench.Equals_OrdinalIgnoreCase_CaseInsensitiveMatch: Default -> 3 outliers were removed (196.09 ns..201.88 ns)
//
// // * Legends *
//   StrLen    : Value of the 'StrLen' parameter
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

[MemoryDiagnoser]
public class Bench
{
    private const int RandSeed = 826528;

    [Params(8, 64, 512)]
    public static int StrLen { get; set; }
    private static int BitCount => StrLen * 6;
    private static int ByteCount => (BitCount + 7) / 8; // rounded up

    private string string01 = null!;
    private string string02 = null!;

    private string caseInsensitiveMatch01 = null!;
    private string caseInsensitiveMatch02 = null!;

    static string GetRandString(Random rand)
    {
        var bytes = new byte[ByteCount];
        rand.NextBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    [GlobalSetup]
    public void GlobalSetup()
    {
        var rand = new Random(RandSeed);

        // const int strlen = 16;

        char[] MakeRandChars(int count)
        => Enumerable.Range(0, count)
            .Select(_ => rand.NextDouble() > 0.5 ? 'A' : 'a')
            .ToArray();

        char[] MakeOppositeChars(char[] chars)
            => chars
                .Select(ch => ch == 'A' ? 'a' : 'A')
                .ToArray();

        string01 = new string(MakeRandChars(StrLen));
        string02 = new string('b', StrLen);
        caseInsensitiveMatch01 = new string(MakeRandChars(StrLen));
        caseInsensitiveMatch02 = new string(MakeOppositeChars(caseInsensitiveMatch01.ToCharArray()));
    }

    [Benchmark]
    public bool ToLowerEq_NoneMatch()
        => string01.ToLower() == string02.ToLower();

    [Benchmark]
    public bool Equals_OrdinalIgnoreCase_NoneMatch()
        => string01.Equals(string02, StringComparison.OrdinalIgnoreCase);

    [Benchmark]
    public bool ToLowerEq_CaseInsensitiveMatch()
        => caseInsensitiveMatch01.ToLower() == caseInsensitiveMatch02.ToLower();

    [Benchmark]
    public bool Equals_OrdinalIgnoreCase_CaseInsensitiveMatch()
        => caseInsensitiveMatch01.Equals(caseInsensitiveMatch02, StringComparison.OrdinalIgnoreCase);

}

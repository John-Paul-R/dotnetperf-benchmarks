namespace ConstAndVariable;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
// Relevant sharplab: https://sharplab.io/#gist:83fc46fc28a3b288cf6260e514ac07d2
// // * Summary *
//
// BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1645 (21H2)
// AMD Ryzen Threadripper 2920X, 1 CPU, 24 logical and 12 physical cores
//     .NET SDK=6.0.100
//     [Host]     : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
// DefaultJob : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
//
//
//     |                            Method |        Mean |     Error |    StdDev |      Median |  Gen 0 | Allocated |
//     |---------------------------------- |------------:|----------:|----------:|------------:|-------:|----------:|
//     |               ConstAddWhereClause |   0.0118 ns | 0.0154 ns | 0.0144 ns |   0.0081 ns |      - |         - |
//     |                FuncAddWhereClause | 112.0758 ns | 2.2500 ns | 2.2098 ns | 111.5232 ns | 0.1702 |     712 B |
//     | ConstAddWhereClause_DynamicString | 113.0918 ns | 1.5706 ns | 1.3115 ns | 113.1639 ns | 0.1683 |     704 B |
//     |  FuncAddWhereClause_DynamicString | 115.7690 ns | 1.3958 ns | 1.2373 ns | 115.2523 ns | 0.1683 |     704 B |
//
// // * Warnings *
//     ZeroMeasurement
// Bench.ConstAddWhereClause: Default -> The method duration is indistinguishable from the empty method duration
//
// // * Hints *
// Outliers
// Bench.FuncAddWhereClause: Default                -> 2 outliers were removed (133.52 ns, 139.92 ns)
// Bench.ConstAddWhereClause_DynamicString: Default -> 2 outliers were removed (122.24 ns, 123.45 ns)
// Bench.FuncAddWhereClause_DynamicString: Default  -> 1 outlier  was  removed (125.42 ns)
//
// // * Legends *
// Mean      : Arithmetic mean of all measurements
// Error     : Half of 99.9% confidence interval
// StdDev    : Standard deviation of all measurements
// Median    : Value separating the higher half of all measurements (50th percentile)
// Gen 0     : GC Generation 0 collects per 1000 operations
// Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
// 1 ns      : 1 Nanosecond (0.000000001 sec)
//     
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
    private const string SqlQueryString = @"
        SELECT
          CAST(
            CASE
              WHEN up.subscription_expires > NOW()
              THEN DATEDIFF(up.subscription_expires, NOW())
              ELSE 0
            END AS INTEGER
          ) AS days_left,
        FROM user_preferences up
        WHERE up.pref_user_id = @userId
        ";

    private static string GetSqlQueryString()
    {
        return @"
        SELECT
          CAST(
            CASE
              WHEN up.subscription_expires > NOW()
              THEN DATEDIFF(up.subscription_expires, NOW())
              ELSE 0
            END AS INTEGER
          ) AS days_left,
        FROM user_preferences up
        WHERE up.pref_user_id = @userId
        ";
    }

    private const int RandSeed = 826528;

    private static readonly int StrLen = "--MyWhereClause--".Length;
    private static readonly int BitCount = StrLen * 6;
    private static readonly int ByteCount = (BitCount + 7) / 8; // rounded up
    static string GetRandString(Random rand)
    {
        var bytes = new byte[ByteCount];
        rand.NextBytes(bytes);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }

    private static string DynamicQueryStringFragment = null!;
    
    [GlobalSetup]
    public void GlobalSetup()
    {
        var rand = new Random(RandSeed);
        DynamicQueryStringFragment = GetRandString(rand);
    }

    [Benchmark]
    public string ConstAddWhereClause()
    {
        return $"{SqlQueryString} --MyWhereClause--";
    }

    [Benchmark]
    public string FuncAddWhereClause()
    {
        var sqlQueryString = GetSqlQueryString();
        return $"{sqlQueryString} --MyWhereClause--";
    }
    
    [Benchmark]
    public string ConstAddWhereClause_DynamicString()
    {
        return $"{SqlQueryString} {DynamicQueryStringFragment}";
    }

    [Benchmark]
    public string FuncAddWhereClause_DynamicString()
    {
        var sqlQueryString = GetSqlQueryString();
        return $"{sqlQueryString} {DynamicQueryStringFragment}";
    }
}

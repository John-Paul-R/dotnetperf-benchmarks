using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

// // * Summary *
//
// BenchmarkDotNet=v0.13.2, OS=Windows 10 (10.0.19044.1889/21H2/November2021Update)
// AMD Ryzen Threadripper 2920X, 1 CPU, 24 logical and 12 physical cores
//     .NET SDK=6.0.100
//     [Host]     : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT AVX2
// DefaultJob : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT AVX2
//
//
//     |                Method |     Mean |    Error |   StdDev |    Gen0 |   Gen1 | Allocated |
//     |---------------------- |---------:|---------:|---------:|--------:|-------:|----------:|
//     |               SplitBy | 13.38 us | 0.101 us | 0.085 us |  3.0670 | 0.7629 |  12.53 KB |
//     |               GroupBy | 22.72 us | 0.189 us | 0.177 us |  4.1504 | 0.0305 |  16.97 KB |
//     |              TwoWhere | 16.08 us | 0.321 us | 0.554 us |  3.0823 | 0.0305 |  12.63 KB |
//     |          WhereExclude | 22.66 us | 0.215 us | 0.191 us |  4.0283 |      - |  16.48 KB |
//     |       AggregateAppend | 69.50 us | 0.767 us | 0.680 us | 17.8223 | 8.9111 | 109.52 KB |
//     | AggregateAppendToList | 46.00 us | 0.884 us | 1.180 us | 28.7476 | 0.0610 | 117.48 KB |
//
// // * Hints *
//     Outliers
// Bench.SplitBy: Default         -> 2 outliers were removed (13.66 us, 13.66 us)
// Bench.WhereExclude: Default    -> 1 outlier  was  removed (23.34 us)
// Bench.AggregateAppend: Default -> 1 outlier  was  removed (72.08 us)

namespace Bench_SplitBy_AggregateEdition;

public static class EnumerableExtensions
{
    public static (IList<T> True, IList<T> False) SplitBy<T>(this IEnumerable<T> source, Predicate<T> splitPredicate)
    {
        var trueList = new List<T>();
        var falseList = new List<T>();
        foreach (var elem in source)
        {
            if (splitPredicate(elem)) {
                trueList.Add(elem);
            } else {
                falseList.Add(elem);
            }
        }

        return (True: trueList, False: falseList);
    }

    public static (IEnumerable<T> True, IEnumerable<T> False) SplitByAggregate<T>(this IEnumerable<T> source,
        Predicate<T> splitPredicate)
    => source.Aggregate(
        seed: (Enumerable.Empty<T>(), Enumerable.Empty<T>()),
        (accum, cur) => splitPredicate(cur)
            ? (accum.Item1.Append(cur), accum.Item2)
            : (accum.Item1, accum.Item2.Append(cur)));
}

public class Program
{
    public static void Main()
    {
        BenchmarkRunner.Run<Bench>();
    }
}

[MemoryDiagnoser]
public class Bench
{
    private const int IterationCount = 1000;
    private const int MaxElementValue = 100;
    private readonly List<int> _testList;
    private const int RandSeed = 1812372198;
    private static bool Compare(int i) => i > 50;
    
    public Bench()
    {
        var rand = new Random(RandSeed);
        _testList = Enumerable
            .Range(0, IterationCount)
            .Select(i => rand.Next(MaxElementValue))
            .ToList();
    }

    [Benchmark]
    public (IList<int> True, IList<int> False) SplitBy()
    {
        return _testList.SplitBy(Compare);
    }

    [Benchmark]
    public (IList<int>, IList<int>) GroupBy()
    {
        var results = _testList
            .GroupBy((i) => i > 50)
            .ToList();

        return (
            results.Single(grouping => grouping.Key == true).ToList(),
            results.Single(grouping => grouping.Key == false).ToList());
    }

    [Benchmark]
    public (IList<int> True, IList<int> False) TwoWhere()
    {
        return (
            True: _testList.Where(Compare).ToList(),
            False: _testList.Where(el => !Compare(el)).ToList());
    }

    [Benchmark]
    public (IList<int> True, IList<int> False) WhereExclude()
    {
        var trues = _testList.Where(Compare);
        var enumerable = trues.ToList();
        return (
            True: enumerable,
            False: _testList.Except(enumerable).ToList());
    }

    [Benchmark]
    public (IEnumerable<int> True, IEnumerable<int> False) AggregateAppend()
    {
        return _testList.SplitByAggregate(Compare);
    }
    
    [Benchmark]
    public (IList<int> True, IList<int> False) AggregateAppendToList()
    {
        var res = _testList.SplitByAggregate(Compare);
        return (res.True.ToList(), res.False.ToList());
    }
}
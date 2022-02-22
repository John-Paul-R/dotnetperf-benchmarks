using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Bench_SplitBy_Top3;

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

    public static (IList<T> True, IList<T> False) SplitByEnumerator<T>(this IEnumerable<T> source, Predicate<T> splitPredicate)
    {
        var trueList = new List<T>();
        var falseList = new List<T>();
        var enumerator = source.GetEnumerator();

        while (enumerator.MoveNext()) {
            if (splitPredicate(enumerator.Current)) {
                trueList.Add(enumerator.Current);
            } else {
                falseList.Add(enumerator.Current);
            }
        }

        enumerator.Dispose();

        return (True: trueList, False: falseList);
    }


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
    private const int MaxElementValue = 100;
    private const int RandSeed = 524679;
    private List<int> _testList = null!;
    private static bool Compare(int i) => i > 50;

    [Params(32, 128, 512, 1024, 4096, 16384)]
    public int MaxItems { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var rand = new Random(RandSeed);
        _testList = Enumerable
            .Range(0, MaxItems)
            .Select(_ => rand.Next(MaxElementValue))
            .ToList();
    }

    [Benchmark]
    public (IList<int> True, IList<int> False) SplitBy()
    {
        return _testList.SplitBy(Compare);
    }

    [Benchmark]
    public (IList<int> True, IList<int> False) SplitByEnumerator()
    {
        return _testList.SplitByEnumerator(Compare);
    }

    [Benchmark]
    public (IList<int>, IList<int>) GroupBy()
    {
        var results = _testList
            .GroupBy(Compare)
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

}
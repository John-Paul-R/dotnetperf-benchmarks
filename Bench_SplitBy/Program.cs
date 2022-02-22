using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

// |           Method |     Mean |    Error |   StdDev |   Gen 0 |  Gen 1 | Allocated |
// |----------------- |---------:|---------:|---------:|--------:|-------:|----------:|
// |          SplitBy | 13.88 us | 0.219 us | 0.205 us |  2.0752 | 0.4120 |      9 KB |
// | SplitByOverAlloc | 13.94 us | 0.200 us | 0.156 us |  1.9531 | 0.6409 |      8 KB |
// |         TwoWhere | 15.96 us | 0.307 us | 0.449 us |  3.0823 | 0.7629 |     13 KB |
// |          GroupBy | 23.89 us | 0.274 us | 0.257 us |  3.1433 | 0.6104 |     13 KB |
// |     WhereExclude | 25.73 us | 0.299 us | 0.279 us |  4.0283 |      - |     16 KB |
// |     SplitByQueue | 28.44 us | 0.261 us | 0.244 us |  6.1035 | 1.5259 |     25 KB |
// |    SplitByLinked | 34.35 us | 0.266 us | 0.262 us | 12.4817 | 3.1128 |     51 KB |

namespace Bench_SplitBy;

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

    public static (IList<T> True, IList<T> False) SplitByOverAlloc<T>(this IEnumerable<T> source, Predicate<T> splitPredicate)
    {
        var sourceList = source ?? throw new ArgumentNullException(nameof(source));
        if (!source.TryGetNonEnumeratedCount(out var count)) {
            sourceList = source.ToList();
            count = sourceList.Count();
        }

        var trueList = new List<T>(count);
        var falseList = new List<T>(count);
        foreach (var elem in sourceList)
        {
            if (splitPredicate(elem)) {
                trueList.Add(elem);
            } else {
                falseList.Add(elem);
            }
        }

        return (True: trueList, False: falseList);
    }


    public static (IList<T> True, IList<T> False) SplitByLinked<T>(this IEnumerable<T> source, Predicate<T> splitPredicate)
    {
        var trueList = new LinkedList<T>();
        var falseList = new LinkedList<T>();
        foreach (var elem in source)
        {
            if (splitPredicate(elem)) {
                trueList.AddLast(elem);
            } else {
                falseList.AddLast(elem);
            }
        }

        return (True: trueList.ToList(), False: falseList.ToList());
    }
    
    public static (IList<T> True, IList<T> False) SplitByQueue<T>(this IEnumerable<T> source, Predicate<T> splitPredicate)
    {
        var trueList = new Queue<T>();
        var falseList = new Queue<T>();
        foreach (var elem in  source)
        {
            if (splitPredicate(elem)) {
                trueList.Enqueue(elem);
            } else {
                falseList.Enqueue(elem);
            }
        }

        return (True: trueList.ToList(), False: falseList.ToList());
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
    private const int IterationCount = 1000;
    private const int MaxElementValue = 100;
    private readonly List<int> _testList;
    private static bool Compare(int i) => i > 50;
    
    public Bench()
    {
        var rand = new Random();
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
    public (IList<int> True, IList<int> False) SplitByQueue()
    {
        return _testList.SplitByQueue(Compare);
    }
    
    [Benchmark]
    public (IList<int> True, IList<int> False) SplitByLinked()
    {
        return _testList.SplitByLinked(Compare);
    }

    [Benchmark]
    public (IList<int> True, IList<int> False) SplitByOverAlloc()
    {
        return _testList.SplitByOverAlloc(Compare);
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
}
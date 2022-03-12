using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

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
    private List<int> _testList = null!;
    
    private static readonly Predicate<int> Comparer = i => i > MaxElementValue / 2;
    private static readonly Func<int, bool> OppositeComparer = i => i <= MaxElementValue / 2;

    [GlobalSetup]
    public void Setup()
    {
        var rand = new Random();
        _testList = Enumerable
            .Range(0, IterationCount)
            .Select(i => rand.Next(MaxElementValue))
            .ToList();
    }

    [Benchmark]
    public void ListRemoveAll()
    {
        _testList.RemoveAll(Comparer);
    }
    
    [Benchmark]
    public void LinqWhereToList()
    {
        _testList.Where(OppositeComparer).ToList();
    }

    [Benchmark]
    public void LinqWhereCount()
    {
        _testList.Where(OppositeComparer).Count();
    }
    
    [Benchmark]
    public void LinqWhereAny()
    {
        _testList.Where(OppositeComparer).Any();
    }

    [Benchmark]
    public void ListAny()
    {
        _testList.Any();
    }

}
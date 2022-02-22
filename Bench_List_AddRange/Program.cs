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
    
    private List<int> _outList = null!;
    
    [GlobalSetup]
    public void Setup()
    {
        var rand = new Random();
        _testList = Enumerable
            .Range(0, IterationCount)
            .Select(i => rand.Next(MaxElementValue))
            .ToList();

        _outList = new List<int>();
    }

    [Benchmark]
    public void AddList()
    {
        new List<int>().AddRange(_testList);
    }
    
    [Benchmark]
    public void AddListAsEnumerable()
    {
        new List<int>().AddRange(_testList.AsEnumerable());
    }

    

}
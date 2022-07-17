using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Bench_DictVsNestedLoops;

public class Container
{
    public string A { get; set; }
}

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
    private const int BenchmarkAccessCount = 1000;

    [Params(10, 100, 1000, 10000, 100000)]
    public int MaxItems { get; set; }

    private string[] _randomKeys = null!;

    private List<KeyValuePair<string, Container>> _keyValuePairs;
        
    [GlobalSetup]
    public void GlobalSetup()
    {
        _keyValuePairs = Enumerable.Range(0, MaxItems)
            .Select(i => new KeyValuePair<string, Container>(i.ToString(), new Container(){ A = i.ToString()}))
            .ToList();
            
        // Init keys to access in benchmarks
        var r = new Random(RandSeed);
        _randomKeys = Enumerable.Range(0, BenchmarkAccessCount)
            .Select(_ => r.Next(MaxItems).ToString())
            .ToArray();
    }

    [Benchmark]
    public Container? Dictionary()
    {
        Container? almostANoOp = null;
        var dictionary = new Dictionary<string, Container>(_keyValuePairs);
        foreach (var key in _randomKeys) {
            almostANoOp = dictionary[key];
        }

        return almostANoOp;
    }

    [Benchmark]
    public Container? Loops()
    {
        Container? almostANoOp = null;
        foreach (var kv in _keyValuePairs) {
            foreach (var key in _randomKeys) {
                if (kv.Key == key) {
                    almostANoOp = kv.Value;
                }
            }
        }

        return almostANoOp;
    }

}
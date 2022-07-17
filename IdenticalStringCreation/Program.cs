namespace IdenticalStringCreation;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

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
    [Params(1, 8, 64, 512)]
    public static int NumIterations;
    private static string[] NumberStrings = null!;
    private static int CheckInt;

    [GlobalSetup]
    public void GlobalSetup()
    {
        NumberStrings = Enumerable.Range(1000, NumIterations)
            .Select(num => num.ToString())
             .ToArray();
        CheckInt = 1000 + NumIterations + 1;
    }

    [Benchmark]
    public bool RepeatToString()
    {
        return NumberStrings.All(num => num.Length <= CheckInt.ToString().Length);
    }

    [Benchmark]
    public bool CacheToString()
    {
        var checkIntString = CheckInt.ToString();
        return NumberStrings.All(num => num.Length <= checkIntString.Length);
    }
}

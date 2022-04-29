using BenchmarkDotNet.Attributes;

namespace Closures;
// // * Summary *
//
// BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1645 (21H2)
// AMD Ryzen Threadripper 2920X, 1 CPU, 24 logical and 12 physical cores
// .NET SDK=6.0.100
//   [Host]     : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
//   DefaultJob : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
//
//
// |                  Method | MaxItems |        Mean |     Error |    StdDev |  Gen 0 |  Gen 1 | Allocated |
// |------------------------ |--------- |------------:|----------:|----------:|-------:|-------:|----------:|
// |    SelectToList_Closure |        8 |    357.2 ns |   6.47 ns |   6.06 ns | 0.1450 |      - |     608 B |
// | SelectToList_MinClosure |        8 |    368.8 ns |   7.30 ns |   7.50 ns | 0.1450 |      - |     608 B |
// |                 ForLoop |        8 |    123.8 ns |   2.52 ns |   2.90 ns | 0.0899 |      - |     376 B |
// |    SelectToList_Closure |       64 |  1,857.2 ns |  26.51 ns |  22.14 ns | 0.8049 |      - |   3,368 B |
// | SelectToList_MinClosure |       64 |  1,955.6 ns |  23.55 ns |  22.03 ns | 0.8049 |      - |   3,368 B |
// |                 ForLoop |       64 |    792.9 ns |   9.00 ns |   8.41 ns | 0.6247 |      - |   2,616 B |
// |    SelectToList_Closure |      512 | 13,547.9 ns | 270.78 ns | 300.97 ns | 5.9509 | 0.1068 |  24,944 B |
// | SelectToList_MinClosure |      512 | 13,488.5 ns | 115.80 ns | 108.32 ns | 5.9509 |      - |  24,944 B |
// |                 ForLoop |      512 |  6,412.7 ns | 113.11 ns | 100.27 ns | 4.9057 | 0.6104 |  20,536 B |
//
// // * Hints *
// Outliers
//   Bench.SelectToList_MinClosure: Default -> 1 outlier  was  removed (408.49 ns)
//   Bench.ForLoop: Default                 -> 1 outlier  was  removed (136.20 ns)
//   Bench.SelectToList_Closure: Default    -> 2 outliers were removed (1.96 us, 2.09 us)
//   Bench.ForLoop: Default                 -> 1 outlier  was  removed (7.22 us)
//
// // * Legends *
//   MaxItems  : Value of the 'MaxItems' parameter
//   Mean      : Arithmetic mean of all measurements
//   Error     : Half of 99.9% confidence interval
//   StdDev    : Standard deviation of all measurements
//   Gen 0     : GC Generation 0 collects per 1000 operations
//   Gen 1     : GC Generation 1 collects per 1000 operations
//   Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
//   1 ns      : 1 Nanosecond (0.000000001 sec)
//
// // * Diagnostic Output - MemoryDiagnoser *
//
//
// // ***** BenchmarkRunner: End *****

public class CreatureChild
{
    public string Name { get; init; }
    public bool HasTail { get; init; }
    public int LengthMeters { get; init; }
}

public class CreaturesMapper
{
    public string[] Name { get; set; }
    public bool[] HasTail { get; set; }
    public int[] LengthMeters { get; set; }

    public List<CreatureChild> GetChildren()
    => Name
        .Select((name, i) => new CreatureChild {
            Name = name,
            HasTail = HasTail[i],
            LengthMeters = LengthMeters[i],
        })
        .ToList();
}

[MemoryDiagnoser]
public class Bench
{
    private const int RandSeed = 826528;

    [Params(8, 64, 512)]
    public int MaxItems { get; set; }

    private const int StrLen = 8;
    private const int BitCount = StrLen * 6;
    private const int ByteCount = (BitCount + 7) / 8; // rounded up

    private CreaturesMapper _creaturesMapper = null!;

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

        _creaturesMapper = new CreaturesMapper {
            Name = new string[MaxItems],
            HasTail = new bool[MaxItems],
            LengthMeters = new int[MaxItems],
        };
        
        for (int i = 0; i < MaxItems; i++) {
            _creaturesMapper.Name[i] = GetRandString(rand);
            _creaturesMapper.HasTail[i] = rand.NextDouble() > 0.5;
            _creaturesMapper.LengthMeters[i] = rand.Next(5);
        }
    }

    [Benchmark]
    public List<CreatureChild> SelectToList_Closure()
    {
        return _creaturesMapper.Name
            .Select((name, i) => new CreatureChild {
                Name = name,
                HasTail = _creaturesMapper.HasTail[i],
                LengthMeters = _creaturesMapper.LengthMeters[i],
            })
            .ToList();
    }

    [Benchmark]
    public List<CreatureChild> SelectToList_MinClosure()
    {
        return _creaturesMapper.GetChildren();
    }

    [Benchmark]
    public List<CreatureChild> ForLoop()
    {
        List<CreatureChild> outList = new(_creaturesMapper.Name.Length);
        for (int i = 0; i < _creaturesMapper.Name.Length; i++) {
            outList.Add(new CreatureChild
            {
                Name = _creaturesMapper.Name[i],
                HasTail = _creaturesMapper.HasTail[i],
                LengthMeters = _creaturesMapper.LengthMeters[i],

            });
        }

        return outList;
    }

}


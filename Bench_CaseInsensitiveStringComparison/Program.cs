using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Bench_CaseInsensitiveStringComparison;

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

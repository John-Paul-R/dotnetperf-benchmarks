using BenchmarkDotNet.Running;

namespace Closures;

class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<Bench>();
    }
}
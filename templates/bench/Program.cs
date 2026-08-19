using BenchTemplate;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<Bench>();

namespace BenchTemplate
{
    [MemoryDiagnoser]
    public class Bench
    {
        [Benchmark]
        public void ApproachA()
        {
            // TODO: implement approach A
        }

        [Benchmark]
        public void ApproachB()
        {
            // TODO: implement approach B
        }
    }
}

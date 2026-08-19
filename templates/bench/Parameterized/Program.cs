using BenchTemplate;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<Bench>();

namespace BenchTemplate
{
    [MemoryDiagnoser]
    public class Bench
    {
        [Params(8, 64, 512)]
        public int N { get; set; }

        private int[] _data = null!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var rand = new Random(42);
            _data = Enumerable.Range(0, N).Select(_ => rand.Next(1000)).ToArray();
        }

        [Benchmark]
        public int ApproachA()
        {
            // TODO: implement approach A
            var sum = 0;
            for (var i = 0; i < _data.Length; i++)
                sum += _data[i];
            return sum;
        }

        [Benchmark]
        public int ApproachB()
        {
            // TODO: implement approach B
            return _data.Sum();
        }
    }
}

using Bench_EnumToString;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

// // * Summary *
//
// BenchmarkDotNet=v0.13.5, OS=manjaro
// AMD Ryzen Threadripper 2920X, 1 CPU, 24 logical and 12 physical cores
// .NET SDK=7.0.203
//   [Host]     : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2
//   DefaultJob : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2
//
//
// |       Method |      Mean |    Error |   StdDev |   Gen0 | Allocated |
// |------------- |----------:|---------:|---------:|-------:|----------:|
// | EnumToString | 154.51 ns | 3.125 ns | 3.720 ns | 0.0017 |     144 B |
// |       Switch |  16.64 ns | 0.172 ns | 0.161 ns |      - |         - |
// |   JustBoxing |  45.19 ns | 0.628 ns | 0.557 ns | 0.0017 |     144 B |

BenchmarkRunner.Run<Bench>();

namespace Bench_EnumToString
{
    public enum Color
    {
        Red,
        Green,
        Blue,
        Magenta,
        Orange,
        Purple,
    }

    [MemoryDiagnoser]
    public class Bench
    {
        [Benchmark]
        public void EnumToString()
        {
            Color.Red.ToString();
            Color.Green.ToString();
            Color.Blue.ToString();
            Color.Magenta.ToString();
            Color.Orange.ToString();
            Color.Purple.ToString();
        }

        [Benchmark]
        public void Switch()
        {
            ToStringSwitch(Color.Red);
            ToStringSwitch(Color.Green);
            ToStringSwitch(Color.Blue);
            ToStringSwitch(Color.Magenta);
            ToStringSwitch(Color.Orange);
            ToStringSwitch(Color.Purple);
        }

        [Benchmark]
        public void JustBoxing()
        {
            NoOp(Color.Red);
            NoOp(Color.Green);
            NoOp(Color.Blue);
            NoOp(Color.Magenta);
            NoOp(Color.Orange);
            NoOp(Color.Purple);
        }

        private object NoOp(object obj)
        {
            return obj;
        }

        private string ToStringSwitch(Color color)
        => color switch
        {
            Color.Red => nameof(Color.Red),
            Color.Green => nameof(Color.Green),
            Color.Blue => nameof(Color.Blue),
            Color.Magenta => nameof(Color.Magenta),
            Color.Orange => nameof(Color.Orange),
            Color.Purple => nameof(Color.Purple),
        };
    }
}

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Bench_EnumerableAppendVsListAdd;

// // * Summary *
//
// BenchmarkDotNet=v0.13.5, OS=manjaro
// AMD Ryzen Threadripper 2920X, 1 CPU, 24 logical and 12 physical cores
// .NET SDK=7.0.203
//   [Host]     : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2
//   DefaultJob : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT AVX2
//
// |          Method | NumElements |         Mean |      Error |     StdDev |   Gen0 | Allocated |
// |---------------- |------------ |-------------:|-----------:|-----------:|-------:|----------:|
// | WriteEnumerable |           8 |    266.54 ns |   2.334 ns |   2.069 ns | 0.0105 |     912 B |
// |       WriteList |           8 |     86.32 ns |   0.754 ns |   0.668 ns | 0.0020 |     176 B |
// |  ReadEnumerable |           8 |    234.42 ns |   3.029 ns |   2.685 ns | 0.0031 |     264 B |
// |        ReadList |           8 |     16.64 ns |   0.384 ns |   0.377 ns | 0.0003 |      24 B |
// | WriteEnumerable |          64 |  2,089.96 ns |  21.459 ns |  20.073 ns | 0.0839 |    7184 B |
// |       WriteList |          64 |    488.67 ns |   1.735 ns |   1.449 ns | 0.0134 |    1144 B |
// |  ReadEnumerable |          64 |  1,035.18 ns |   3.066 ns |   2.868 ns | 0.0076 |     712 B |
// |        ReadList |          64 |     64.35 ns |   0.715 ns |   0.597 ns | 0.0002 |      24 B |
// | WriteEnumerable |         512 | 16,702.72 ns | 221.315 ns | 207.018 ns | 0.6714 |   57360 B |
// |       WriteList |         512 |  2,851.87 ns |  53.651 ns |  57.406 ns | 0.0992 |    8384 B |
// |  ReadEnumerable |         512 |  7,614.91 ns | 101.965 ns |  95.379 ns | 0.0458 |    4296 B |
// |        ReadList |         512 |    487.61 ns |   9.622 ns |  10.695 ns |      - |      24 B |

BenchmarkRunner.Run<Bench>();
namespace Bench_EnumerableAppendVsListAdd
{
    [MemoryDiagnoser]
    public class Bench
    {
        [Params(8, 64, 512)]
        public static int NumElements;
        private static IEnumerable<string> _enumerableForReading = new List<string>();
        private static List<string> _listForReading = new List<string>();

        [GlobalSetup]
        public static void GlobalSetup()
        {
            for (int i = 0; i < NumElements; i++) {
                _enumerableForReading = _enumerableForReading.Append("Item");
            }
            for (int i = 0; i < NumElements; i++) {
                _listForReading.Add("Item");
            }
        }

        [Benchmark]
        public object WriteEnumerable()
        {
            IEnumerable<string> enumerable = new List<string>();
            for (int i = 0; i < NumElements; i++) {
                enumerable = enumerable.Append("Item");
            }

            return enumerable;
        }

        [Benchmark]
        public object WriteList()
        {
            List<string> list = new List<string>();
            for (int i = 0; i < NumElements; i++) {
                list.Add("Item");
            }

            return list;
        }

        [Benchmark]
        public object ReadEnumerable()
        {
            int i = 0;
            foreach (var s in _enumerableForReading) {
                i += s.Length;
            }

            return i;
        }

        [Benchmark]
        public object ReadList()
        {
            int i = 0;
            foreach (var s in _listForReading) {
                i += s.Length;
            }

            return i;
        }
    }
}

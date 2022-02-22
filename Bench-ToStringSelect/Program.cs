
using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

//
// BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1466 (21H2)
// AMD Ryzen Threadripper 2920X, 1 CPU, 24 logical and 12 physical cores
//     .NET SDK=6.0.100
//     [Host]     : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
// DefaultJob : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
//
//
//  |                        Method |     Mean |    Error |   StdDev |   Median |   Gen 0 |  Gen 1 | Allocated |
//  |------------------------------ |---------:|---------:|---------:|---------:|--------:|-------:|----------:|
//  | ToStringTwoSelectsOneToString | 45.98 us | 0.421 us | 0.373 us | 45.92 us | 26.5503 | 1.1597 |    109 KB |
//  |  ToStringOneSelectOneToString | 48.26 us | 1.077 us | 3.175 us | 49.57 us | 26.4893 | 0.3052 |    108 KB |
//  |                    ToStringX2 | 64.28 us | 0.575 us | 0.480 us | 64.28 us | 34.0576 | 0.0610 |    139 KB |
//  |                ToStringInterp | 80.22 us | 1.035 us | 0.969 us | 80.21 us | 26.4893 | 0.6104 |    108 KB |
//
//
//     ---
//
//  lang vers 8:
//
// BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1466 (21H2)
// AMD Ryzen Threadripper 2920X, 1 CPU, 24 logical and 12 physical cores
//     .NET SDK=6.0.100
//     [Host]     : .NET Core 3.1.10 (CoreCLR 4.700.20.51601, CoreFX 4.700.20.51901), X64 RyuJIT
// DefaultJob : .NET Core 3.1.10 (CoreCLR 4.700.20.51601, CoreFX 4.700.20.51901), X64 RyuJIT
//
//
//  |                        Method |      Mean |    Error |   StdDev |   Gen 0 |   Gen 1 | Allocated |
//  |------------------------------ |----------:|---------:|---------:|--------:|--------:|----------:|
//  | ToStringTwoSelectsOneToString |  62.52 us | 0.886 us | 0.829 us | 26.5503 |  0.0610 |    109 KB |
//  |  ToStringOneSelectOneToString |  58.60 us | 1.123 us | 1.460 us | 26.4893 |  0.0610 |    108 KB |
//  |                    ToStringX2 |  88.30 us | 1.729 us | 2.248 us | 34.0576 | 11.2305 |    139 KB |
//  |                ToStringInterp | 145.12 us | 1.899 us | 1.684 us | 32.2266 |  0.2441 |    132 KB |


    
namespace Bench_ToStringSelect
{

    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<Bench>();
        }
    }

    public class Item
    {
        public string Value { get; set; }
        public string Label { get; set; }
    }

    [MemoryDiagnoser]
    public class Bench
    {
        private const int RandSeed = 542789;
        private const int MaxValue = 1000;
        private const int ElementCount = 1000;

        private List<int> _elements = null!;

        [GlobalSetup]
        public void Setup()
        {
            var rand = new Random(RandSeed);
            _elements = Enumerable
                .Range(0, ElementCount)
                .Select(i => rand.Next(MaxValue))
                .ToList();
        }

        [Benchmark]
        public void ToStringTwoSelectsOneToString()
        {
            _elements.Select(i => i.ToString()).Select(s => new Item {
                    Value = s,
                    Label = $"{s} HZ",
                })
                .ToList();
        }

        [Benchmark]
        public void ToStringOneSelectOneToString()
        {
            _elements.Select(s =>
                {
                    var st = s.ToString();
                    return new Item {
                        Value = st,
                        Label = $"{st} HZ",
                    };
                })
                .ToList();
        }


        [Benchmark]
        public void ToStringX2()
        {
            _elements.Select(s => new Item {
                    Value = s.ToString(),
                    Label = $"{s.ToString()} HZ",
                })
                .ToList();
        }

        [Benchmark]
        public void ToStringInterp()
        {
            _elements.Select(s => new Item {
                    Value = s.ToString(),
                    Label = $"{s} HZ",
                })
                .ToList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

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
                .Select(i => rand.NextBytes())
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
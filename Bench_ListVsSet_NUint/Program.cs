using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace Bench_ListVsSet_NUint
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<ListVsSet_NUint>();
        }
    }

    [SimpleJob(RuntimeMoniker.Net60)]
    public class ListVsSet_NUint
    {
        private const int RandSeed = 826528;
        private const int BenchmarkAccessCount = 1000;

        [Params(4, 8, 16, 32, 64, 128, 256, 512)]
        public int MaxItems { get; set; }

        private List<nuint> _list = null!;
        private HashSet<nuint> _hashSet = null!;
        private SortedSet<nuint> _sortedSet = null!;
        private nuint[] _randomKeys = null!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var rand = new Random(RandSeed);

            var randValues = Enumerable.Range(0, MaxItems)
                .Select(_ => (nuint)rand.Next())
                .ToList();

            _list = randValues.ToList();
            _hashSet = randValues.ToHashSet();
            _sortedSet = new SortedSet<nuint>(randValues);

            var r = new Random(RandSeed);
            _randomKeys = Enumerable.Range(0, BenchmarkAccessCount)
                .Select(_ => r.Next(MaxItems))
                .Select(i => randValues[i])
                .ToArray();
        }

        [Benchmark]
        public void ListContains()
        {
            foreach (var key in _randomKeys) {
                _list.Contains(key);
            }
        }

        [Benchmark]
        public void HashSetContains()
        {
            foreach (var key in _randomKeys) {
                _hashSet.Contains(key);
            }
        }

        [Benchmark]
        public void SortedSetContains()
        {
            foreach (var key in _randomKeys) {
                _sortedSet.Contains(key);
            }
        }
    }
}

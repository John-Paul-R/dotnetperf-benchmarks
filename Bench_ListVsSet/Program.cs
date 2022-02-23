using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace Bench_ListVsSet
{
    public class Container
    {
        public string A { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Bench>();
        }
    }

    [SimpleJob(RuntimeMoniker.Net60)]
    public class Bench
    {
        private const int RandSeed = 826528;
        private const int BenchmarkAccessCount = 1000;

        [Params(4, 8, 16, 32, 64, 128, 256, 512)]
        public int MaxItems { get; set; }

        private const int StrLen = 8;
        private const int BitCount = StrLen * 6;
        private const int ByteCount = (BitCount + 7) / 8; // rounded up

        private List<string> _list = null!;
        private HashSet<string> _hashSet = null!;
        private SortedSet<string> _sortedSet = null!;
        private string[] _randomKeys = null!;
        
        
        [GlobalSetup]
        public void GlobalSetup()
        {
            var rand = new Random(RandSeed);

            var randStrings = Enumerable.Range(0, MaxItems)
                .Select(_ =>
                {
                    var bytes = new byte[ByteCount];
                    rand.NextBytes(bytes);
                    return Convert.ToBase64String(bytes);
                })
                .ToList();

            _list = randStrings.ToList();
            _hashSet = randStrings.ToHashSet();
            _sortedSet = new SortedSet<string>(randStrings);

            // Init values to access in benchmarks
            var r = new Random(RandSeed);
            _randomKeys = Enumerable.Range(0, BenchmarkAccessCount)
                .Select(_ => r.Next(MaxItems))
                .Select(i => randStrings[i])
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

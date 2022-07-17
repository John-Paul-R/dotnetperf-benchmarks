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
    // See https://stackoverflow.com/questions/16612936/immutable-dictionary-vs-dictionary-vs-c5
    // for original inspiration for these benchmarks.

    public class Container
    {
        public string A { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // BenchmarkRunner.Run<Bench>();
            Go();
        }

        public static void Go()
        {
            var keyValuePairs = Enumerable.Range(0, 1000)
                .Select(i => new KeyValuePair<string, Container>(i.ToString(), new Container(){ A = i.ToString()}))
                .ToList();

            ReadOnlyDictionary<string, Container> dictionary = null!;
            ReadOnlyDictionary<string, Container> concurrentDictionary = null!;
            ImmutableDictionary<string, Container> immutableDictionary = null!;
            
            // Init Dictionaries
            immutableDictionary = keyValuePairs.ToImmutableDictionary();
            
            // ImmutableDictionary.CreateRange<string, object>();

            dictionary = new ReadOnlyDictionary<string, Container>(
                new Dictionary<string, Container>(keyValuePairs));

            concurrentDictionary = new ReadOnlyDictionary<string, Container>(
                new ConcurrentDictionary<string, Container>(keyValuePairs));
            
            var test = immutableDictionary["0"];
            test.A = "testing";

            Console.WriteLine(test.A);
            Console.WriteLine(immutableDictionary["0"].A);

        }

    }

    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    public class Bench
    {
        private const int RandSeed = 826528;
        private const int BenchmarkAccessCount = 1000;

        [Params(100, 1000, 10000, 100000)]
        public int MaxItems { get; set; }

        private ReadOnlyDictionary<string, Container> _dictionary = null!;
        private ReadOnlyDictionary<string, Container> _concurrentDictionary = null!;
        private ImmutableDictionary<string, Container> _immutableDictionary = null!;
        private string[] _randomKeys = null!;
        
        
        [GlobalSetup]
        public void GlobalSetup()
        {
            var rand = new Random(RandSeed);
            
            var keyValuePairs = Enumerable.Range(0, MaxItems)
                .Select(i => new KeyValuePair<string, Container>(i.ToString(), new Container(){ A = i.ToString()}))
                .ToList();

            // Init Dictionaries
            _immutableDictionary = keyValuePairs.ToImmutableDictionary();
            
            // ImmutableDictionary.CreateRange<string, object>();

            _dictionary = new ReadOnlyDictionary<string, Container>(
                new Dictionary<string, Container>(keyValuePairs));

            _concurrentDictionary = new ReadOnlyDictionary<string, Container>(
                new ConcurrentDictionary<string, Container>(keyValuePairs));

            // Init keys to access in benchmarks
            var r = new Random(RandSeed);
            _randomKeys = Enumerable.Range(0, BenchmarkAccessCount)
                .Select(i => r.Next(MaxItems).ToString())
                .ToArray();

            var test = _immutableDictionary["0"];
            test.A = "testing";
            
            Console.WriteLine(test.A);
            Console.WriteLine(_immutableDictionary["0"].A);

        }

        [Benchmark]
        public void BaseDictionary()
        {
            foreach (var key in _randomKeys) {
                _dictionary.TryGetValue(key, out var value);
            }
        }

        [Benchmark]
        public void ConcurrentDictionary()
        {
            foreach (var key in _randomKeys) {
                _concurrentDictionary.TryGetValue(key, out var value);
            }
        }

        [Benchmark]
        public void ImmutableDictionaryT()
        {
            foreach (var key in _randomKeys) {
                _immutableDictionary.TryGetValue(key, out var value);
            }
        }

    }
}

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Bench_Dictionaries_2026
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
            BenchmarkRunner.Run<Bench>();
        }
    }

    [SimpleJob]
    public class Bench
    {
        private const int RandSeed = 826528;
        private const int BenchmarkAccessCount = 1000;

        [Params(100, 1000, 10000, 100000)]
        public int MaxItems { get; set; }

        private ReadOnlyDictionary<string, Container> _dictionary = null!;
        private ReadOnlyDictionary<string, Container> _concurrentDictionary = null!;
        private ImmutableDictionary<string, Container> _immutableDictionary = null!;
        private FrozenDictionary<string, Container> _frozenDictionary = null!;
        private string[] _randomKeys = null!;


        [GlobalSetup]
        public void GlobalSetup()
        {
            var keyValuePairs = Enumerable.Range(0, MaxItems)
                .Select(i => new KeyValuePair<string, Container>(i.ToString(), new Container(){ A = i.ToString()}))
                .ToList();

            // Init Dictionaries
            _immutableDictionary = keyValuePairs.ToImmutableDictionary();

            _dictionary = new ReadOnlyDictionary<string, Container>(
                new Dictionary<string, Container>(keyValuePairs));

            _concurrentDictionary = new ReadOnlyDictionary<string, Container>(
                new ConcurrentDictionary<string, Container>(keyValuePairs));

            _frozenDictionary = keyValuePairs.ToFrozenDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // Init keys to access in benchmarks
            var r = new Random(RandSeed);
            _randomKeys = Enumerable.Range(0, BenchmarkAccessCount)
                .Select(i => r.Next(MaxItems).ToString())
                .ToArray();
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

        [Benchmark]
        public void FrozenDictionaryT()
        {
            foreach (var key in _randomKeys) {
                _frozenDictionary.TryGetValue(key, out var value);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

// // * Summary *
//
// BenchmarkDotNet=v0.13.1, OS=manjaro
// AMD Ryzen Threadripper 2920X, 1 CPU, 24 logical and 12 physical cores
// .NET SDK=7.0.203
//   [Host]     : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT
//   DefaultJob : .NET 7.0.5 (7.0.523.17405), X64 RyuJIT
//
//
// |         Method |     Mean |    Error |   StdDev |  Gen 0 |  Gen 1 | Allocated |
// |--------------- |---------:|---------:|---------:|-------:|-------:|----------:|
// |           Func | 23.11 us | 0.190 us | 0.178 us | 0.9155 | 0.6409 |     39 KB |
// |      ValueFunc | 37.43 us | 0.143 us | 0.127 us | 1.2817 | 1.2207 |     39 KB |
// | ValueFuncNoBox | 11.25 us | 0.093 us | 0.082 us | 0.7172 | 0.3510 |     39 KB |

namespace Bench_ToStringSelect;

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<Bench>();
    }
}

public interface IFunc<T, TRes> where T : struct where TRes : struct
{
    TRes Invoke(T param);
}

public struct TimesFiveSelector : IFunc<int, int>
{
    public int Invoke(int val) => val * 5;
}

[MemoryDiagnoser]
public class Bench
{
    private const int RandSeed = 542789;
    private const int MaxValue = 1000;
    private const int ElementCount = 10000;

    private List<int> _elements = null!;

    private Func<int, int> _func = val => val * 5;
    private IFunc<int, int> _valueFunc = new TimesFiveSelector();
    private TimesFiveSelector _valueFuncNoBox = default;

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
    public object Func()
    {
        int[] ret = new int[_elements.Count];
        for (int i = 0; i < ret.Length; i++) {
            ret[i] = _func.Invoke(_elements[i]);
        }

        return ret;
    }

    [Benchmark]
    public object ValueFunc()
    {
        int[] ret = new int[_elements.Count];
        for (int i = 0; i < ret.Length; i++) {
            ret[i] = _valueFunc.Invoke(_elements[i]);
        }

        return ret;
    }

    [Benchmark]
    public object ValueFuncNoBox()
    {
        int[] ret = new int[_elements.Count];
        for (int i = 0; i < ret.Length; i++) {
            ret[i] = _valueFuncNoBox.Invoke(_elements[i]);
        }

        return ret;
    }
}

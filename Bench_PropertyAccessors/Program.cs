using System.Linq.Expressions;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Bench_PropertyAccessors;
// * Summary *

// BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1526 (21H2)
// AMD Ryzen Threadripper 2920X, 1 CPU, 24 logical and 12 physical cores
//     .NET SDK=6.0.100
//     [Host]     : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
// DefaultJob : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
//
//
//     |                                        Method |        Mean |     Error |    StdDev |      Median |  Gen 0 | Allocated |
//     |---------------------------------------------- |------------:|----------:|----------:|------------:|-------:|----------:|
//     |                                PropertyGetter |   0.0099 ns | 0.0152 ns | 0.0143 ns |   0.0014 ns |      - |         - |
//     |                                   SanityCheck |   0.0179 ns | 0.0273 ns | 0.0228 ns |   0.0103 ns |      - |         - |
//     |                         TimeToGetPropertyInfo |  25.2605 ns | 0.2803 ns | 0.2484 ns |  25.2517 ns |      - |         - |
//     |                 Reflect_PropertyInfo_GetValue |  64.0829 ns | 1.3025 ns | 1.0877 ns |  63.7649 ns |      - |         - |
//     | Reflect_PropertyInfo_GetAccessors_InvokeFirst | 135.0083 ns | 2.7300 ns | 2.2797 ns | 135.0109 ns | 0.0305 |     128 B |
//     |                Reflect_PropertyInfo_GetMethod |  64.7460 ns | 1.3182 ns | 1.3537 ns |  64.9362 ns |      - |         - |
//     |                 Reflect_Precompiled_GetMethod |   1.6953 ns | 0.0406 ns | 0.0380 ns |   1.6904 ns |      - |         - |
//     |              Reflect_Precompiled_GetGetMethod |   1.4588 ns | 0.0304 ns | 0.0285 ns |   1.4468 ns |      - |         - |
//     |                                  FuncAccessor |   1.4441 ns | 0.0267 ns | 0.0250 ns |   1.4410 ns |      - |         - |
//     |                            ExpressionAccessor |   0.7457 ns | 0.0292 ns | 0.0273 ns |   0.7493 ns |      - |         - |
//
// // * Warnings *
// ZeroMeasurement
// Bench.PropertyGetter: Default -> The method duration is indistinguishable from the empty method duration
// Bench.SanityCheck: Default    -> The method duration is indistinguishable from the empty method duration

public class Program
{
    public static void Main()
    {
        BenchmarkRunner.Run<Bench>();
    }
}

public class MyObject
{
    private string _stringField  = "UNINITIALIZED FIELD";
    public string MyStringProperty { get; init; } = "UNINITIALIZED PROPERTY";

    public void SetStringField(string val) => _stringField = val; 
    public string GetStringField() => _stringField;

}

[MemoryDiagnoser]
public class Bench
{
    private MyObject _testObject = null!;

    [GlobalSetup]
    public void Setup()
    {
        _testObject = new MyObject {MyStringProperty = "This is String Property content!"};
        _testObject.SetStringField("This is String Field content!");
    }

    [Benchmark]
    public string PropertyGetter()
    {
        return _testObject.MyStringProperty;
    }

    [Benchmark]
    public string SanityCheck()
    {
        return _testObject.GetStringField();
    }

    private static readonly PropertyInfo MyStringProperty_PropertyInfo = typeof(MyObject)
        .GetProperty(nameof(MyObject.MyStringProperty))!;

    [Benchmark]
    public PropertyInfo TimeToGetPropertyInfo()
    {
        return typeof(MyObject)
            .GetProperty(nameof(MyObject.MyStringProperty))!;
    }

    [Benchmark]
    public string Reflect_PropertyInfo_GetValue()
    {
        return (string) MyStringProperty_PropertyInfo
            .GetValue(_testObject)!;
    }

    [Benchmark]
    public string Reflect_PropertyInfo_GetAccessors_InvokeFirst()
    {
        return (string) MyStringProperty_PropertyInfo
            .GetAccessors()[0]
            .Invoke(_testObject, Array.Empty<object>())!;
    }

    [Benchmark]
    public string Reflect_PropertyInfo_GetMethod()
    {
        return (string) MyStringProperty_PropertyInfo
            .GetMethod!
            .Invoke(_testObject, Array.Empty<object>())!;
    }

    // Precompile delegate from memberinfo
    private static readonly Func<MyObject, string> ReflectionGeneratedAccessor
        = MyStringProperty_PropertyInfo
            .GetMethod!
            .CreateDelegate<Func<MyObject, string>>();
    [Benchmark]
    public string Reflect_Precompiled_GetMethod()
    {
        return ReflectionGeneratedAccessor(_testObject);
    }
    
    // Precompile delegate from memberinfo
    private static readonly Func<MyObject, string> ReflectionGeneratedAccessorV2
        = MyStringProperty_PropertyInfo
            .GetGetMethod(nonPublic: false)
            !.CreateDelegate<Func<MyObject, string>>();
    [Benchmark]
    public string Reflect_Precompiled_GetGetMethod()
    {
        return ReflectionGeneratedAccessorV2(_testObject);
    }

    private static readonly Func<MyObject, string> MyStringPropertyAccessor = (obj) => obj.MyStringProperty;
    [Benchmark]
    public string FuncAccessor()
    {
        return MyStringPropertyAccessor(_testObject);
    }

    private static readonly Expression<Func<MyObject, string>> MyStringPropertyExpression = (obj) => obj.MyStringProperty;
    private static readonly Func<MyObject, string> MyStringPropertyExpressionCompiled
        = MyStringPropertyExpression.Compile();
    [Benchmark]
    public string ExpressionAccessor()
    {
        return MyStringPropertyExpressionCompiled(_testObject);
    }

}
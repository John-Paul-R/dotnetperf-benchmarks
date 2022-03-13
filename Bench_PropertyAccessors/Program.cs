using System.Linq.Expressions;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Bench_PropertyAccessors;

public class Program
{
    public static void Main()
    {
        BenchmarkRunner.Run<Bench>();
    }
}

public class MyObject
{
    public string MyStringProperty { get; init; } = "UNINITIALIZED PROPERTY";
}

[MemoryDiagnoser]
public class Bench
{
    private MyObject _testObject = null!;

    [GlobalSetup]
    public void Setup()
    {
        _testObject = new MyObject {MyStringProperty = "This is String Property content!"};
    }

    [Benchmark]
    public string PropertyGetter()
    {
        return _testObject.MyStringProperty;
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
using System.Linq.Expressions;
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

    [Benchmark]
    public string Reflect_GetProperty_GetValue()
    {
        return (string) typeof(MyObject)
            .GetProperty(nameof(MyObject.MyStringProperty))
            !.GetValue(_testObject)!;
    }

    [Benchmark]
    public string Reflect_GetProperty_GetAccessors_InvokeFirst()
    {
        return (string) typeof(MyObject)
            .GetProperty(nameof(MyObject.MyStringProperty))
            !.GetAccessors()[0]
            .Invoke(_testObject, Array.Empty<object>())!;
    }

    [Benchmark]
    public string Reflect_GetProperty_GetMethod()
    {
        return (string) typeof(MyObject)
            .GetProperty(nameof(MyObject.MyStringProperty))
            !.GetMethod!
            .Invoke(_testObject, Array.Empty<object>())!;
    }

    // Precompile delegate from memberinfo
    private static readonly Func<MyObject, string> ReflectionGeneratedAccessor
        = typeof(MyObject)
            .GetProperty(nameof(MyObject.MyStringProperty))
            !.GetMethod!
            .CreateDelegate<Func<MyObject, string>>();
    [Benchmark]
    public string Reflect_Precompiled_GetMethod()
    {
        return ReflectionGeneratedAccessor(_testObject);
    }
    
    // Precompile delegate from memberinfo
    private static readonly Func<MyObject, string> ReflectionGeneratedAccessorV2
        = typeof(MyObject)
            .GetProperty(nameof(MyObject.MyStringProperty))
            !.GetGetMethod(nonPublic: false)
            !.CreateDelegate<Func<MyObject, string>>();
    [Benchmark]
    public string Reflect_Precompiled_GetGetMethod()
    {
        return ReflectionGeneratedAccessorV2(_testObject);
    }
    
    // Precompile delegate from memberinfo
    private static readonly Func<MyObject, string> DelegateAccessor
        = (Func<MyObject, string>) Delegate.CreateDelegate(
            typeof(Func<MyObject, string>),
            typeof(MyObject)
                .GetProperty(nameof(MyObject.MyStringProperty))
                !.GetGetMethod(nonPublic: false)!);
    [Benchmark]
    public string Reflect_PrecompiledV3_Delegate()
    {
        return DelegateAccessor(_testObject);
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
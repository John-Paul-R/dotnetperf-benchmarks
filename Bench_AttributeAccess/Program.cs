using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Reflection;

public class Program
{
    public static void Main()
    {
        BenchmarkRunner.Run<Bench>();
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class PrimaryColorAttribute : Attribute
{
    
}

public enum AnnotatedColors
{
    [PrimaryColor] Red,
    Green,
    [PrimaryColor] Blue,
    [PrimaryColor] Yellow,
    Teal,
    Pink,
    Maroon,
    Violet,
}

[MemoryDiagnoser]
public class Bench
{
    private List<AnnotatedColors> _testList = null!;
    
    [GlobalSetup]
    public void Setup()
    {
        _testList = Enum.GetValues<AnnotatedColors>().ToList();
    }


    [Benchmark]
    public void IsPrimary_GetType_GetCustomAttribute()
    {
        foreach (var val in _testList) {
            _IsPrimary_GetType_GetCustomAttribute<PrimaryColorAttribute>(val);
        }
    }
    private static bool _IsPrimary_GetType_GetCustomAttribute<TAttribute>(Enum enumValue) where TAttribute : Attribute
        => enumValue.GetType()
            .GetField(enumValue.ToString())!
            .GetCustomAttributes<TAttribute>(inherit: false)
            .Any();


    [Benchmark]
    public void IsPrimary_TypeofTEnum_GetCustomAttribute()
    {
        foreach (var val in _testList) {
            _IsPrimary_TypeofTEnum_GetCustomAttribute<AnnotatedColors, PrimaryColorAttribute>(val);
        }
    }
    private static bool _IsPrimary_TypeofTEnum_GetCustomAttribute<TEnum, TAttribute>(TEnum enumValue)
        where TEnum : Enum
        where TAttribute : Attribute
        => typeof(TEnum)
            .GetField(enumValue.ToString())!
            .GetCustomAttributes<TAttribute>(inherit: false)
            .Any();


    [Benchmark]
    public void IsPrimary_TypeofTEnum_IsDefined()
    {
        foreach (var val in _testList) {
            _IsPrimary_TypeofTEnum_IsDefined<AnnotatedColors, PrimaryColorAttribute>(val);
        }
    }
    private static bool _IsPrimary_TypeofTEnum_IsDefined<TEnum, TAttribute>(TEnum enumValue)
        where TEnum : Enum
        where TAttribute : Attribute
        => typeof(TEnum)
            .GetField(enumValue.ToString())!
            .IsDefined(typeof(TAttribute));


    [Benchmark]
    public void IsPrimary_GetType_IsDefined()
    {
        foreach (var val in _testList) {
            _IsPrimary_GetType_IsDefined<PrimaryColorAttribute>(val);
        }
    }
    private static bool _IsPrimary_GetType_IsDefined<TAttribute>(Enum enumValue)
        where TAttribute : Attribute
        => enumValue.GetType()
            .GetField(enumValue.ToString())!
            .IsDefined(typeof(TAttribute));


    private static Type EnumType = typeof(AnnotatedColors);
    [Benchmark]
    public void IsPrimary_CachedType_IsDefined()
    {
        // var enumType = _testList.First().GetType();
        foreach (var val in _testList) {
            _IsPrimary_CachedType_IsDefined<PrimaryColorAttribute>(EnumType, val);
        }
    }
    private static bool _IsPrimary_CachedType_IsDefined<TAttribute>(Type enumType, Enum enumValue)
        where TAttribute : Attribute
        => enumType
            .GetField(enumValue.ToString())!
            .IsDefined(typeof(TAttribute));
    
}
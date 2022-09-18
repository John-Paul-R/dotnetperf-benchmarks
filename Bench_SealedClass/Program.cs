// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<Bench>();

public class Bench
{
    private BaseClass _baseClass = new BaseClass();
    private Inheritor _inheritor = new Inheritor();
    private InheritorSealed _inheritorSealed = new InheritorSealed();

    [Benchmark]
    public void Base_VirtualInt() => _baseClass.VirtualInt();

    [Benchmark]
    public void Base_Int() => _baseClass.Int();

    [Benchmark]
    public void Inheritor_VirtualInt() => _inheritor.VirtualInt();

    [Benchmark]
    public void Inheritor_Int() => _inheritor.Int();

    [Benchmark]
    public void InheritorSealed_VirtualInt() => _inheritorSealed.VirtualInt();

    [Benchmark]
    public void InheritorSealed_Int() => _inheritorSealed.Int();

}

public class BaseClass
{
    public virtual int VirtualInt() => 5;

    public int Int() => 5;
}

public sealed class InheritorSealed : BaseClass
{
    public override int VirtualInt() => 10;

    public int Int() => 10;
}

public class Inheritor : BaseClass
{
    public override int VirtualInt() => 10;

    public int Int() => 10;
}
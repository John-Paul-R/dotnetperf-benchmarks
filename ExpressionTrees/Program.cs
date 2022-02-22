
using System.Linq.Expressions;

namespace ExpressionTrees;

class RootNodeWithId : Element
{
    public string Id { get; set; }
    public Element Child { get; set; }
}

internal class Element
{
    public RootNodeWithId Parent { get; set; }
    public Element Child { get; set; }
}

internal class Leaf
{
    public Element Parent { get; set; }
}

class RootIdLoggerAttribute<T> : Attribute
{
    private readonly Expression<Func<T, string>> _idAccessor;

    // This param is invalid (cs0181). Essentially, only fairly simple types can be used as attribute metadata.
    public RootIdLoggerAttribute(Expression<Func<T, string>> idAccessor)
    {
        _idAccessor = idAccessor;
    }
}

class Program
{
    [RootIdLogger<Element>()]
    public static void Main(string[] args)
    {
        
    }
}
// See https://aka.ms/new-console-template for more information

// var firstSet = new[] {"Name1"};
// var secondSet = new[] {"Name2"};
// var thirdSet = new[] {"Name3"};
// var fourthSet = new[] {"Name3"};
//
// IEnumerable<string> names = firstSet.Concat(secondSet).Concat(thirdSet);
//
// IEnumerable<(string, string, string)> zipped = firstSet.Zip(secondSet, thirdSet);
//
// Console.WriteLine("Hello, World!");

using System.Runtime.Serialization;
using Newtonsoft.Json;

abstract class Shape
{
    public abstract int Area { get; }
}

class Rectangle : Shape
{
    public int Length { get; set; }
    public int Width { get; set; }
    
    public override int Area { get => Length * Width; }
}

class Program
{
    static bool IsRectangle(Shape shape)
        => shape is Rectangle rect && rect.Length == rect.Width;

    public static void Main(string[] args)
    {
        Shape myShape = new Rectangle {
            Length = 50,
            Width = 2,
        };

        if (myShape is Rectangle rect) {
            if (rect.Length == rect.Width) {
                Console.WriteLine($"Found square {rect}");
            } else {
                Console.WriteLine($"Found rectangle {rect}");
            }
        } else {
            Console.WriteLine("Shape did not have a length of 50");
        }

        var chars = new string('-', 5);

        Console.WriteLine(myShape switch {
            Rectangle rectangle => rectangle.Length == rectangle.Width
                ? $"Found square {rectangle}"
                : $"Found rectangle {rectangle}", 
            _ => "Shape did not have a length of 50",
        });
        
        // ---

        Console.WriteLine(JsonConvert.SerializeObject(new Rectangle(), Formatting.None));
        Console.WriteLine(JsonConvert.SerializeObject(new Rectangle(), Formatting.Indented));
        
        var allObjects = new List<SecureObject>().AsQueryable();
        var allowedObjects = allObjects.GetWithSecurity("testing");

        
        var allChildObjects = new List<SecureChildObject>().AsQueryable();
        var allowedChildObjects = allObjects.GetWithSecurity("testing");

        allChildObjects.Where(o => o.Test_GetAllowedGroups().Contains("the group"));

        Console.WriteLine(8192D);
        Console.WriteLine(8192.0D);
        Console.WriteLine(8192);
        double[] nums = new[] {
            0,
            0.1,
            62.5,
            8000,
            16000,
            8182
        };
        Console.WriteLine(string.Join('\n', nums));
        
        Console.WriteLine(string.Join('\n', Enum.GetNames(typeof(Names))));
        
        Console.WriteLine($"{Names.Cindy} {Names.Cindy.ToString()}");

        string? possiblyNullString = null;//Random.Shared.NextDouble() > 0.5 ? "test" : null;

        var res = possiblyNullString?.Replace(" ", "").ToLower();
    }
}

public interface ISecureResource
{
    public string[] AllowedGroups { get; }

    // public Expression<Func<ISecureResource, string[]>> GetAllowedGroups();
}

public class SecureObject : ISecureResource
{
    public string[] AllowedGroups { get; set; } = new[] {"group01"};
}

public class SecureChildObject : ISecureResource
{
    public SecureObject Parent { get; set; }
    // Will this work for SQL-converted "Where" clauses?
    public string[] AllowedGroups => Parent.AllowedGroups;

    public string[] Test_GetAllowedGroups() => Parent.AllowedGroups;
}

public static class SecureObjectExtensions
{
    public static IQueryable<T> GetWithSecurity<T>(
        this IQueryable<T> queryable,
        string group) where T : ISecureResource 
    => queryable.Where(so => so.AllowedGroups.Contains(group));
}

public enum Names
{
    Jeff = 1,
    [EnumMember(Value = "testing")]
    Cindy = 1,
    Qincy = 2,
}
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
    }
}


// See https://aka.ms/new-console-template for more information

using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;

Console.WriteLine("Hello, World!");

ValueTask<string> GetStringAsync()
    => new ValueTask<string>("testing");

async Task<string> ActuallyGetStringAsync()
    => await GetStringAsync();

var myString = await GetStringAsync();

// var dict = new ConcurrentDictionary<string, string>();
//
// if (dict.TryGetValue(null, out var value)) {
//     Console.WriteLine($"Value was {value}");
// } else {
//     Console.WriteLine("Warning, null");
// }

var values = new List<int?> { 1, null, 6, 8, null };

var groups = values.GroupBy(el => el);

Console.WriteLine(string.Join(",",groups.Select(g => $"{g.Key}: {g.Count()}\n")));

int? i = 5;

Console.WriteLine(i == 5);

var list = new List<DateTime>();
list.Add(new DateTime(1980, 5, 5));
list.Add(new DateTime(1982, 10, 20));
list.Add(new DateTime(1984, 1, 4));
list.Add(new DateTime(1979, 6, 19));

var outList = list.OrderByDescending(v => v);

Console.WriteLine(
    string.Join("\n", outList.Select(date => date.ToString())));

{

DateTime? testTime = null;

Console.WriteLine(testTime == null);

DateTime testTime2 = DateTime.MaxValue;

if (testTime2 == null) {
    Console.WriteLine();
}

}

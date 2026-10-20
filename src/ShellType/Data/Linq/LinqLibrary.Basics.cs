using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

public static partial class LinqLibrary
{
    private static readonly CommandEntry[] Filtering =
    [
        L("var evens = numbers.Where(n => n % 2 == 0);", "Keep only the elements that match a condition: the even numbers", Easy),
        L("var adults = people.Where(p => p.Age >= 18);", "Keep the people who are 18 or older", Easy),
        L("var shortWords = words.Where(w => w.Length < 5);", "Keep the words shorter than five characters", Easy),
        L("var locals = people.Where(p => p.City == \"Oslo\" && p.Age > 30);", "Combine conditions with && to filter on several properties at once", Medium),
        L("var everyOther = numbers.Where((n, i) => i % 2 == 0);", "The second lambda parameter is the index: keep every other element", Hard),
        L("var texts = items.OfType<string>();", "Keep only the elements of a given type, cast to it", Medium),
    ];

    private static readonly CommandEntry[] Projection =
    [
        L("var names = people.Select(p => p.Name);", "Turn each element into something else: a person into their name", Easy),
        L("var squares = numbers.Select(n => n * n);", "Square every number", Easy),
        L("var upper = words.Select(w => w.ToUpper());", "Upper-case every word", Easy),
        L("var cards = people.Select(p => new { p.Name, p.Age });", "Project into an anonymous type with just the properties you need", Medium),
        L("var numbered = words.Select((w, i) => $\"{i + 1}. {w}\");", "Select with an index to number each element", Hard),
        L("var letters = words.SelectMany(w => w.ToCharArray());", "Flatten: every word becomes its characters, all in one sequence", Medium),
        L("var pairs = numbers.Zip(words);", "Pair up two sequences element by element into tuples", Medium),
    ];

    private static readonly CommandEntry[] Ordering =
    [
        L("var sorted = numbers.Order();", "Sort the numbers in ascending order (.NET 7 and later)", Easy),
        L("var byAge = people.OrderBy(p => p.Age);", "Sort by a key, smallest first", Easy),
        L("var oldestFirst = people.OrderByDescending(p => p.Age);", "Sort by a key, largest first", Easy),
        L("var byCity = people.OrderBy(p => p.City).ThenBy(p => p.Name);", "Sort by city, and by name within the same city", Medium),
        L("var latest = orders.OrderByDescending(o => o.Date).ThenByDescending(o => o.Total);", "Newest orders first; on the same date, the biggest first", Hard),
        L("var backwards = words.Reverse();", "Reverse the order of the elements", Easy),
    ];

    private static readonly CommandEntry[] Partitioning =
    [
        L("var firstThree = numbers.Take(3);", "Take the first three elements", Easy),
        L("var rest = numbers.Skip(3);", "Skip the first three and keep the rest", Easy),
        L("var page = people.Skip(2).Take(2);", "Paging: skip the first page of two people and take the second", Medium),
        L("var lastTwo = numbers.TakeLast(2);", "Take the last two elements", Easy),
        L("var leading = numbers.TakeWhile(n => n < 10);", "Take elements until the condition first fails, then stop", Medium),
        L("var afterHeader = lines.SkipWhile(l => l.StartsWith(\"#\"));", "Skip elements while the condition holds, keep everything after", Hard),
        L("var batches = numbers.Chunk(3);", "Split into arrays of at most three elements each", Medium),
    ];
}

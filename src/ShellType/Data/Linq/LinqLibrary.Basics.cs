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
}

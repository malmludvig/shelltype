using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

public static partial class LinqLibrary
{
    private static readonly CommandEntry[] Sets =
    [
        L("var unique = numbers.Distinct();", "Remove duplicates", Easy),
        L("var onePerCity = people.DistinctBy(p => p.City);", "Keep the first element for each distinct key", Medium),
        L("var both = numbers.Union(other);", "Every element in either sequence, without duplicates", Easy),
        L("var shared = numbers.Intersect(other);", "Only the elements that are in both sequences", Easy),
        L("var onlyHere = numbers.Except(other);", "The elements in the first sequence that are not in the second", Medium),
        L("var all = numbers.Concat(other);", "Append one sequence to another, duplicates kept", Easy),
    ];

    private static readonly CommandEntry[] Grouping =
    [
        L("var byCity = people.GroupBy(p => p.City);", "Group elements by a key; each group has a Key and its elements", Easy),
        L("var perCity = people.CountBy(p => p.City);", "Count the elements per key, as key/count pairs (.NET 9 and later)", Medium),
        L("var sizes = people.GroupBy(p => p.City).Select(g => new { City = g.Key, Count = g.Count() });", "Group, then turn each group into a city and how many live there", Hard),
        L("var spent = orders.GroupBy(o => o.PersonId).Select(g => new { g.Key, Total = g.Sum(o => o.Total) });", "Total amount spent per customer", Hard),
        L("var byLength = words.ToLookup(w => w.Length);", "Build a lookup: like GroupBy, but evaluated at once and indexable by key", Medium),
    ];
}

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
}

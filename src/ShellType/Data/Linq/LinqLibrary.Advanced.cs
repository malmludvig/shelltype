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

    private static readonly CommandEntry[] Joining =
    [
        L("var receipts = orders.Join(people, o => o.PersonId, p => p.Id, (o, p) => new { p.Name, o.Total });", "Inner join: match each order to its person by id", Hard),
        L("var withOrders = people.GroupJoin(orders, p => p.Id, o => o.PersonId, (p, os) => new { p.Name, Count = os.Count() });", "Group join: every person with the orders that belong to them", Hard),
        L("var labelled = numbers.Zip(words, (n, w) => $\"{n}: {w}\");", "Combine two sequences pairwise with your own function", Medium),
    ];

    private static readonly CommandEntry[] Conversion =
    [
        L("var list = numbers.ToList();", "Run the query now and store the result in a List<T>", Easy),
        L("var array = words.ToArray();", "Run the query now and store the result in an array", Easy),
        L("var byId = people.ToDictionary(p => p.Id);", "Build a dictionary keyed by a property; duplicate keys throw", Medium),
        L("var ages = people.ToDictionary(p => p.Name, p => p.Age);", "Build a dictionary with your own key and value", Medium),
        L("var cities = people.Select(p => p.City).ToHashSet();", "Collect into a HashSet for fast lookups without duplicates", Medium),
        L("var csv = string.Join(\",\", numbers);", "Not LINQ itself, but how you join a sequence into one string", Easy),
        L("var ids = numbers.Cast<long>();", "Cast every element to another type; throws on elements that do not fit", Medium),
        L("var indexed = words.Index();", "Pair each element with its index, as (Index, Item) tuples (.NET 9 and later)", Medium),
    ];
}

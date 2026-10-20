using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

public static partial class LinqLibrary
{
    private static readonly CommandEntry[] Aggregation =
    [
        L("var count = numbers.Count();", "Count the elements", Easy),
        L("var bigOrders = orders.Count(o => o.Total > 100);", "Count only the elements that match a condition", Easy),
        L("var sum = numbers.Sum();", "Add up all the numbers", Easy),
        L("var revenue = orders.Sum(o => o.Total);", "Add up a property: the total of every order", Easy),
        L("var average = people.Average(p => p.Age);", "The mean of a property: the average age", Easy),
        L("var youngest = people.Min(p => p.Age);", "The smallest value of a property (the age, not the person)", Easy),
        L("var oldest = people.MaxBy(p => p.Age);", "The element with the largest key: the oldest person, not their age", Medium),
        L("var product = numbers.Aggregate((a, b) => a * b);", "Fold the sequence into one value: multiply everything together", Hard),
        L("var sentence = words.Aggregate((a, b) => a + \" \" + b);", "Fold the words into one string separated by spaces", Hard),
    ];

    private static readonly CommandEntry[] Elements =
    [
        L("var first = numbers.First();", "The first element; throws if the sequence is empty", Easy),
        L("var firstAdult = people.First(p => p.Age >= 18);", "The first element that matches; throws if none does", Easy),
        L("var match = people.FirstOrDefault(p => p.Name == \"Ada\");", "The first match, or null (default) if there is none", Medium),
        L("var last = numbers.Last();", "The last element", Easy),
        L("var one = people.Single(p => p.Id == 7);", "The only match; throws if there are zero or several", Medium),
        L("var third = numbers.ElementAt(2);", "The element at an index, counting from zero", Easy),
        L("var safe = numbers.DefaultIfEmpty(0);", "Use a default value when the sequence is empty", Medium),
    ];
}

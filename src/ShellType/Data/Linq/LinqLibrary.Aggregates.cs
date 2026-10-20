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
}

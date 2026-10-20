using ShellType.Models;

namespace ShellType.Data;

public static partial class LinqLibrary
{
    /// <summary>Small tasks solved as a sequence of statements, each building on the one before, typed in order in operations mode.</summary>
    public static IReadOnlyList<Operation> Operations { get; } =
    [
        // ---- short: 2 to 4 steps ----
        Op("sum the even numbers", "add up only the even numbers.",
            ("var evens = numbers.Where(n => n % 2 == 0);", "Filter first, so only even numbers are left"),
            ("var total = evens.Sum();", "Then add up what is left"),
            ("Console.WriteLine(total);", "Print the result")),

        Op("list adult names", "show the names of everyone 18 or older, sorted.",
            ("var adults = people.Where(p => p.Age >= 18);", "Keep the adults"),
            ("var names = adults.Select(p => p.Name).Order();", "Take just the names and sort them"),
            ("Console.WriteLine(string.Join(\", \", names));", "Print them on one line")),

        Op("find a person", "look up Ada without crashing if she is missing.",
            ("var ada = people.FirstOrDefault(p => p.Name == \"Ada\");", "FirstOrDefault gives null instead of throwing"),
            ("if (ada is null) return;", "Stop if nobody matched"),
            ("Console.WriteLine(ada.Age);", "Safe to use her now")),

        Op("top three orders", "show the three biggest orders.",
            ("var biggest = orders.OrderByDescending(o => o.Total);", "Sort the largest first"),
            ("var top = biggest.Take(3).ToList();", "Keep three, and run the query once with ToList"),
            ("top.ForEach(o => Console.WriteLine(o.Total));", "Print each total")),

        Op("check the input", "make sure every number is valid before using them.",
            ("var any = numbers.Any();", "Is there anything at all?"),
            ("var valid = numbers.All(n => n >= 0);", "Are all of them zero or more?"),
            ("var first = numbers.DefaultIfEmpty(0).First();", "Read the first one, with 0 when the array is empty")),
    ];

    private static Operation Op(string name, string goal, params (string Text, string Description)[] steps) =>
        new(name, goal, steps.Select(s => new CommandEntry(s.Text, s.Description, CommandCategory.Text, Difficulty.Medium, Prompt)).ToList());
}

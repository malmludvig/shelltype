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

        // ---- medium: 5 to 7 steps ----
        Op("people per city", "count how many people live in each city, biggest city first.",
            ("var groups = people.GroupBy(p => p.City);", "One group per city"),
            ("var counts = groups.Select(g => new { City = g.Key, Count = g.Count() });", "Turn each group into a city and a count"),
            ("var ranked = counts.OrderByDescending(c => c.Count);", "Biggest city first"),
            ("var top = ranked.First();", "The city with the most people"),
            ("Console.WriteLine($\"{top.City}: {top.Count}\");", "Print it")),

        Op("page through people", "show page 3 of the people list, ten per page, sorted by name.",
            ("var pageSize = 10;", "How many to show on a page"),
            ("var pageNumber = 3;", "The page to show, counting from 1"),
            ("var sorted = people.OrderBy(p => p.Name);", "Paging only makes sense in a stable order"),
            ("var page = sorted.Skip((pageNumber - 1) * pageSize).Take(pageSize);", "Skip the earlier pages, take one page"),
            ("var pages = (people.Count + pageSize - 1) / pageSize;", "Total number of pages, rounded up"),
            ("Console.WriteLine($\"page {pageNumber} of {pages}\");", "Show where we are")),

        Op("clean up words", "turn the word list into unique, lower-case words in alphabetical order.",
            ("var trimmed = words.Select(w => w.Trim());", "Remove stray spaces"),
            ("var nonEmpty = trimmed.Where(w => w.Length > 0);", "Drop empty strings"),
            ("var lower = nonEmpty.Select(w => w.ToLower());", "Same case, so duplicates match"),
            ("var unique = lower.Distinct();", "Remove duplicates"),
            ("var sorted = unique.Order().ToList();", "Sort and run the query")),

        Op("name the orders", "print each order with the name of the person who placed it.",
            ("var byId = people.ToDictionary(p => p.Id);", "A dictionary makes looking up a person by id fast"),
            ("var recent = orders.Where(o => o.Date.Year == 2026);", "Only this year's orders"),
            ("var named = recent.Select(o => new { byId[o.PersonId].Name, o.Total });", "Look up each order's person"),
            ("var sorted = named.OrderBy(n => n.Name);", "Group visually by sorting on name"),
            ("foreach (var n in sorted) Console.WriteLine($\"{n.Name} {n.Total}\");", "Print every line")),
    ];

    private static Operation Op(string name, string goal, params (string Text, string Description)[] steps) =>
        new(name, goal, steps.Select(s => new CommandEntry(s.Text, s.Description, CommandCategory.Text, Difficulty.Medium, Prompt)).ToList());
}

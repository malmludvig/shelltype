using ShellType.Models;

namespace ShellType.Data;

/// <summary>
/// Simple C# LINQ statements over a small, fixed set of collections, typed at a C# REPL prompt.
/// Each group of operators lives in its own partial file.
/// </summary>
public static partial class LinqLibrary
{
    /// <summary>The prompt of the C# REPL the statements are typed in.</summary>
    public const string Prompt = "csharp>";

    /// <summary>
    /// The collections every statement works on, element by element, shown above the test and the quiz.
    /// It is real C#: with Person(Id, Name, Age, City) and Order(Id, PersonId, Total, Date) records, every statement and operation compiles against it.
    /// </summary>
    public const string Data = """
        int[] numbers = [4, 8, 15, 16, 23, 42, 7, 3];
        int[] other = [3, 7, 10, 15, 99];
        string[] words = ["apple", "kiwi", "banana", "fig", "cherry", "Kiwi"];
        string[] lines = ["# report", "# 2026", "name,age", "Ada,36"];
        object[] items = [1, "one", 2.5, "two", 'x'];

        List<Person> people =
        [
            new(1, "Ada", 36, "Oslo"),
            new(2, "Linus", 17, "Helsinki"),
            new(3, "Grace", 45, "Oslo"),
            new(4, "Alan", 41, "London"),
            new(7, "Edsger", 29, "Helsinki"),
        ];

        List<Order> orders =
        [
            new(1, 1, 120.50m, new(2026, 3, 2)),
            new(2, 3, 80.00m, new(2026, 3, 9)),
            new(3, 1, 45.25m, new(2025, 12, 24)),
            new(4, 4, 210.00m, new(2026, 4, 1)),
            new(5, 7, 99.90m, new(2026, 4, 1)),
        ];
        """;

    private static readonly Lazy<IReadOnlyList<(string Group, CommandEntry Command)>> Grouped =
        new(() => Sources().SelectMany(s => s.Commands.Select(c => (s.Group, c))).ToList());

    private static readonly Lazy<IReadOnlyList<CommandEntry>> AllCommands =
        new(() => Grouped.Value.Select(g => g.Command).ToList());

    /// <summary>Every statement, in learning order.</summary>
    public static IReadOnlyList<CommandEntry> Essentials => AllCommands.Value;

    /// <summary>The statements up to a difficulty, for time and commands tests.</summary>
    public static IReadOnlyList<CommandEntry> Filter(Difficulty maxDifficulty) =>
        Essentials.Where(c => c.Difficulty <= maxDifficulty).ToList();

    /// <summary>The group of operators a statement belongs to, e.g. "filtering" or "grouping".</summary>
    public static string GroupOf(CommandEntry command) =>
        Grouped.Value.FirstOrDefault(g => g.Command == command).Group ?? "linq";

    private static IEnumerable<(string Group, CommandEntry[] Commands)> Sources()
    {
        yield return ("filtering", Filtering);
        yield return ("projection", Projection);
        yield return ("ordering", Ordering);
        yield return ("partitioning", Partitioning);
        yield return ("aggregation", Aggregation);
        yield return ("elements", Elements);
        yield return ("quantifiers", Quantifiers);
        yield return ("sets", Sets);
        yield return ("grouping", Grouping);
        yield return ("joining", Joining);
        yield return ("conversion", Conversion);
        yield return ("query syntax", QuerySyntax);
    }

    // The categories are Linux ones; LINQ statements all count as text processing.
    private static CommandEntry L(string text, string description, Difficulty difficulty) =>
        new(text, description, CommandCategory.Text, difficulty, Prompt);
}

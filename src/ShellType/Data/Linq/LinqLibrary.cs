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

    /// <summary>The collections every statement works on, shown above the test.</summary>
    public const string Data =
        "int[] numbers, other · string[] words, lines · object[] items · List<Person> people (Id, Name, Age, City) · List<Order> orders (Id, PersonId, Total, Date)";

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
    }

    // The categories are Linux ones; LINQ statements all count as text processing.
    private static CommandEntry L(string text, string description, Difficulty difficulty) =>
        new(text, description, CommandCategory.Text, difficulty, Prompt);
}

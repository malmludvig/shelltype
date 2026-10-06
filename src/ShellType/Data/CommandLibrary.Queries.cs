using ShellType.Models;

namespace ShellType.Data;

public static partial class CommandLibrary
{
    public static IEnumerable<CommandEntry> ByCategory(CommandCategory category) =>
        All.Where(c => c.Category == category);

    /// <summary>
    /// Commands matching the given categories whose difficulty is at most <paramref name="maxDifficulty"/>.
    /// An empty category set means "every category".
    /// </summary>
    public static IReadOnlyList<CommandEntry> Filter(
        IReadOnlyCollection<CommandCategory> categories,
        Difficulty maxDifficulty) =>
        All.Where(c => (categories.Count == 0 || categories.Contains(c.Category))
                       && c.Difficulty <= maxDifficulty)
           .ToList();

    public static CommandEntry? Find(string text) =>
        All.FirstOrDefault(c => c.Text == text);

    public static IReadOnlyDictionary<CommandCategory, int> CountByCategory() =>
        All.GroupBy(c => c.Category).ToDictionary(g => g.Key, g => g.Count());
}

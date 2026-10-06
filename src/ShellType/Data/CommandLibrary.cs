using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

/// <summary>
/// The built-in catalogue of commands. Each category lives in its own partial file.
/// </summary>
public static partial class CommandLibrary
{
    private static readonly Lazy<IReadOnlyList<CommandEntry>> AllCommands =
        new(() => Sources().SelectMany(s => s).ToList());

    public static IReadOnlyList<CommandEntry> All => AllCommands.Value;

    private static IEnumerable<CommandEntry[]> Sources()
    {
        yield return Navigation;
        yield return Files;
        yield return Text;
        yield return Search;
        yield return Processes;
        yield return Networking;
    }

    private static CommandEntry[] Build(
        CommandCategory category,
        params (string Text, string Description, Difficulty Difficulty)[] items) =>
        items.Select(i => new CommandEntry(i.Text, i.Description, category, i.Difficulty)).ToArray();
}

using ShellType.Models;

namespace ShellType.Data;

/// <summary>
/// The git cli, from setting up a repository to rewriting history, typed at a shell prompt inside a repository.
/// Each group of commands lives in its own partial file.
/// </summary>
public static partial class GitLibrary
{
    /// <summary>The prompt of a shell inside a git repository, with the branch shown like most git prompts do.</summary>
    public const string Prompt = "~/repo (main)$";

    private static readonly Lazy<IReadOnlyList<(string Group, CommandEntry Command)>> Grouped =
        new(() => Sources().SelectMany(s => s.Commands.Select(c => (s.Group, c))).ToList());

    private static readonly Lazy<IReadOnlyList<CommandEntry>> AllCommands =
        new(() => Grouped.Value.Select(g => g.Command).ToList());

    /// <summary>Every command, in learning order.</summary>
    public static IReadOnlyList<CommandEntry> Essentials => AllCommands.Value;

    /// <summary>The commands up to a difficulty, for time and commands tests.</summary>
    public static IReadOnlyList<CommandEntry> Filter(Difficulty maxDifficulty) =>
        Essentials.Where(c => c.Difficulty <= maxDifficulty).ToList();

    /// <summary>The commands in the given categories up to a difficulty. An empty category set means every category.</summary>
    public static IReadOnlyList<CommandEntry> Filter(IReadOnlyCollection<GitCategory> categories, Difficulty maxDifficulty) =>
        Grouped.Value
            .Where(g => (categories.Count == 0 || categories.Contains(CategoryOf(g.Group))) && g.Command.Difficulty <= maxDifficulty)
            .Select(g => g.Command)
            .ToList();

    /// <summary>The category a command belongs to, e.g. <see cref="GitCategory.Branching"/> for <c>git switch -c</c>.</summary>
    public static GitCategory CategoryOf(CommandEntry command) => CategoryOf(GroupOf(command));

    /// <summary>The name a category is shown with, e.g. "remotes &amp; undo".</summary>
    public static string Label(GitCategory category) => category switch
    {
        GitCategory.Basics => "basics",
        GitCategory.Branching => "branching",
        _ => "remotes & undo",
    };

    private static GitCategory CategoryOf(string group) => group switch
    {
        "branching" or "merging" => GitCategory.Branching,
        "remotes" or "undoing" or "advanced" => GitCategory.RemotesAndUndo,
        _ => GitCategory.Basics,
    };

    /// <summary>The group a command belongs to, e.g. "branching" or "undoing".</summary>
    public static string GroupOf(CommandEntry command) =>
        Grouped.Value.FirstOrDefault(g => g.Command == command).Group ?? "git";

    private static IEnumerable<(string Group, CommandEntry[] Commands)> Sources()
    {
        yield return ("setup", Setup);
        yield return ("staging", Staging);
        yield return ("committing", Committing);
        yield return ("history", History);
        yield return ("branching", Branching);
        yield return ("merging", Merging);
        yield return ("remotes", Remotes);
        yield return ("undoing", Undoing);
        yield return ("stashing", Stashing);
        yield return ("tags", Tags);
        yield return ("advanced", Advanced);
    }

    private static CommandEntry G(string text, string description, Difficulty difficulty) =>
        new(text, description, CommandCategory.Git, difficulty, Prompt);
}

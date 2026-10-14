using ShellType.Models;

namespace ShellType.Data;

/// <summary>
/// The chapters on the theory page. Each chapter lives in its own partial file.
/// </summary>
public static partial class TheoryLibrary
{
    /// <summary>The parts of the theory page, in reading order.</summary>
    public static readonly string[] Groups = ["files", "users & permissions", "processes", "system", "the shell", "networking"];

    private static readonly Lazy<IReadOnlyList<TheoryTopic>> AllTopics = new(() => Topics().ToList());

    public static IReadOnlyList<TheoryTopic> All => AllTopics.Value;

    public static TheoryTopic? Find(string slug) => All.FirstOrDefault(t => t.Slug == slug);

    private static IEnumerable<TheoryTopic> Topics()
    {
        yield return FilesystemHierarchy;
        yield return EverythingIsAFile;
        yield return Paths;
        yield return InodesAndLinks;
        yield return DisksAndMounting;
        yield return UsersAndGroups;
        yield return Permissions;
        yield return SpecialPermissions;
        yield return Processes;
        yield return SignalsAndJobs;
    }
}

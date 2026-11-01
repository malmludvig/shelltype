namespace ShellType.Models;

/// <summary>
/// Broad groups of git commands that can be toggled on and off in a git test.
/// Each one covers a few of the git library's groups.
/// </summary>
public enum GitCategory
{
    /// <summary>Setting up, staging, committing, reading history, stashing and tagging.</summary>
    Basics,

    /// <summary>Creating and switching branches, merging, rebasing and cherry-picking.</summary>
    Branching,

    /// <summary>Fetching, pulling and pushing, undoing mistakes and tracking down bugs.</summary>
    RemotesAndUndo,
}

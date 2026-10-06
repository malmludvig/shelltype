using ShellType.Models;

namespace ShellType.Game;

/// <summary>
/// Replays a fixed list of commands (for "repeat test"), then falls back to
/// another source if the player outlasts the list in a timed test.
/// </summary>
public sealed class ReplaySource(IReadOnlyList<CommandEntry> commands, ICommandSource fallback) : ICommandSource
{
    private int _index;

    public CommandEntry Next() =>
        _index < commands.Count ? commands[_index++] : fallback.Next();
}

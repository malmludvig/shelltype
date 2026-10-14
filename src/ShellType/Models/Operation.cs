namespace ShellType.Models;

/// <summary>How many steps an operation takes.</summary>
public enum OperationLength
{
    /// <summary>2 to 4 steps.</summary>
    Short = 1,

    /// <summary>5 to 7 steps.</summary>
    Medium = 2,

    /// <summary>8 to 10 steps.</summary>
    Long = 3,
}

/// <summary>
/// A real-world task done as a sequence of commands, like freeing a busy port
/// or opening a port in the firewall. Each step's description says why it is done.
/// </summary>
public sealed record Operation(string Name, string Goal, IReadOnlyList<CommandEntry> Steps)
{
    public OperationLength Length => Steps.Count switch
    {
        <= 4 => OperationLength.Short,
        <= 7 => OperationLength.Medium,
        _ => OperationLength.Long,
    };
}

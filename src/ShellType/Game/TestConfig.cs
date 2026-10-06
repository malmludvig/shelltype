using ShellType.Models;

namespace ShellType.Game;

public enum TestMode
{
    Time,
    Commands,
}

/// <summary>
/// Everything that defines a single typing test.
/// </summary>
public sealed record TestConfig
{
    public static readonly int[] TimeOptions = [15, 30, 60, 120];
    public static readonly int[] CommandOptions = [5, 10, 25, 50];

    public TestMode Mode { get; init; } = TestMode.Time;
    public int TimeSeconds { get; init; } = 30;
    public int CommandCount { get; init; } = 10;
    public Difficulty MaxDifficulty { get; init; } = Difficulty.Medium;

    /// <summary>Categories to draw commands from. Empty means all of them.</summary>
    public IReadOnlySet<CommandCategory> Categories { get; init; } = new HashSet<CommandCategory>();

    public int ModeValue => Mode == TestMode.Time ? TimeSeconds : CommandCount;

    public string ModeLabel => Mode == TestMode.Time ? $"time {TimeSeconds}" : $"commands {CommandCount}";
}

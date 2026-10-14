using ShellType.Models;

namespace ShellType.Game;

public enum TestMode
{
    Time,
    Commands,
    Operations,
}

/// <summary>
/// Everything that defines a single typing test.
/// </summary>
public sealed record TestConfig
{
    public static readonly int[] TimeOptions = [15, 30, 60, 120];
    public static readonly int[] CommandOptions = [5, 10, 25, 50];

    public Platform Platform { get; init; } = Platform.Linux;
    public TestMode Mode { get; init; } = TestMode.Time;
    public int TimeSeconds { get; init; } = 30;
    public int CommandCount { get; init; } = 10;
    public Difficulty MaxDifficulty { get; init; } = Difficulty.Medium;
    public OperationLength OperationLength { get; init; } = OperationLength.Short;

    /// <summary>Categories to draw commands from. Empty means all of them.</summary>
    public IReadOnlySet<CommandCategory> Categories { get; init; } = new HashSet<CommandCategory>();

    /// <summary>Wrong characters are rejected; you cannot move on until you type the right one.</summary>
    public bool StopOnError { get; init; }

    /// <summary>Backspace is disabled, so every mistake sticks.</summary>
    public bool ConfidenceMode { get; init; }

    /// <summary>Commands and operations end after a set number of commands; time mode never runs out of them.</summary>
    public bool IsFixedLength => Mode != TestMode.Time;

    public int ModeValue => Mode switch
    {
        TestMode.Time => TimeSeconds,
        TestMode.Commands => CommandCount,
        _ => (int)OperationLength,
    };

    public string ModeLabel => TestModes.Label(Mode, ModeValue, Platform);
}

public static class TestModes
{
    public static string Label(TestMode mode, int value, Platform platform = Platform.Linux)
    {
        var label = mode switch
        {
            TestMode.Time => $"time {value}",
            TestMode.Commands => $"commands {value}",
            _ => $"operations {((OperationLength)value).ToString().ToLowerInvariant()}",
        };

        // Linux is the default, so only Cisco tests get a prefix.
        return platform == Platform.Cisco ? $"cisco {label}" : label;
    }

    /// <summary>Operations are hand-written sequences, so the difficulty setting does not apply to them.</summary>
    public static string DifficultyLabel(TestMode mode, Difficulty difficulty) =>
        mode == TestMode.Operations ? "" : difficulty.ToString().ToLowerInvariant();
}

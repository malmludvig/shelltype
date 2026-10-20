using ShellType.Game;
using ShellType.Models;

namespace ShellType.Services;

public enum CaretStyle
{
    Line,
    Block,
    Underline,
    Off,
}

/// <summary>
/// Preferences that persist between visits. Also remembers the last test setup.
/// </summary>
public sealed class UserSettings
{
    public string Theme { get; set; } = "shelltype";
    public CaretStyle Caret { get; set; } = CaretStyle.Line;
    public bool SmoothCaret { get; set; } = true;
    public double FontSize { get; set; } = 1.6;
    public bool ShowLiveWpm { get; set; } = true;
    public bool ShowDescriptions { get; set; } = true;
    public bool ShowUpcoming { get; set; } = true;
    public bool KeySounds { get; set; }
    public bool StopOnError { get; set; }
    public bool ConfidenceMode { get; set; }

    public Platform Platform { get; set; } = Platform.Linux;
    public TestMode Mode { get; set; } = TestMode.Time;
    public int TimeSeconds { get; set; } = 30;
    public int CommandCount { get; set; } = 10;
    public Difficulty MaxDifficulty { get; set; } = Difficulty.Medium;
    public OperationLength OperationLength { get; set; } = OperationLength.Short;
    public List<CommandCategory> Categories { get; set; } = [];

    public TestConfig ToTestConfig() => new()
    {
        Platform = Platform,
        Mode = EffectiveMode,
        TimeSeconds = TimeSeconds,
        CommandCount = CommandCount,
        MaxDifficulty = MaxDifficulty,
        OperationLength = OperationLength,
        Categories = Categories.ToHashSet(),
        StopOnError = StopOnError,
        ConfidenceMode = ConfidenceMode,
    };

    /// <summary>The essentials list only exists for Cisco and Claude Code, so on Linux it falls back to a commands test.</summary>
    public TestMode EffectiveMode =>
        Mode == TestMode.Essentials && Platform == Platform.Linux ? TestMode.Commands : Mode;
}

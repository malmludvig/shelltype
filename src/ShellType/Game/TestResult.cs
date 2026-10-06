using ShellType.Models;

namespace ShellType.Game;

/// <summary>Speed and errors during one second of a test, used for the results chart.</summary>
public sealed record SecondSample(int Second, double Wpm, double Raw, int Errors);

/// <summary>How the player did on a single command.</summary>
public sealed record CommandResult(string Text, string Typed, bool Correct, int Mistakes, double? Wpm, bool Finished = true);

/// <summary>Everything we know about a finished test.</summary>
public sealed record TestResult
{
    public required DateTimeOffset CompletedAt { get; init; }
    public required TestMode Mode { get; init; }
    public required int ModeValue { get; init; }
    public required Difficulty Difficulty { get; init; }
    public required double Wpm { get; init; }
    public required double RawWpm { get; init; }
    public required double Accuracy { get; init; }
    public required double Consistency { get; init; }
    public required TimeSpan Duration { get; init; }
    public required int CorrectChars { get; init; }
    public required int IncorrectChars { get; init; }
    public required int ExtraChars { get; init; }
    public required int MissedChars { get; init; }
    public required IReadOnlyList<SecondSample> Samples { get; init; }
    public required IReadOnlyList<CommandResult> Commands { get; init; }

    public string ModeLabel => Mode == TestMode.Time ? $"time {ModeValue}" : $"commands {ModeValue}";

    /// <summary>A test this short or with this little typing does not count toward records.</summary>
    public bool IsValid => Duration.TotalSeconds >= 3 && Commands.Count > 0;
}

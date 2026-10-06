using System.Text;
using ShellType.Models;

namespace ShellType.Game;

public enum CharState
{
    Pending,
    Correct,
    Incorrect,
    Extra,
}

/// <summary>
/// The player's attempt at typing one command.
/// </summary>
public sealed class CommandAttempt(CommandEntry command)
{
    /// <summary>How many characters past the end of the target the player may type.</summary>
    public const int MaxExtra = 10;

    private readonly StringBuilder _typed = new();

    public CommandEntry Command { get; } = command;
    public string Target => Command.Text;
    public string Typed => _typed.ToString();
    public int TypedLength => _typed.Length;
    public bool IsSubmitted { get; private set; }
    public bool IsCorrect => Typed == Target;

    /// <summary>Wrong keystrokes made on this command, even if later corrected.</summary>
    public int Mistakes { get; private set; }

    public TimeSpan? FirstKeyAt { get; private set; }
    public TimeSpan? SubmittedAt { get; private set; }

    /// <summary>Number of character cells to render: target plus any extra typed chars.</summary>
    public int DisplayLength => Math.Max(Target.Length, _typed.Length);

    public bool WouldBeCorrect(char c) => _typed.Length < Target.Length && Target[_typed.Length] == c;

    public char DisplayCharAt(int index) => index < Target.Length ? Target[index] : _typed[index];

    public CharState StateAt(int index)
    {
        if (index >= _typed.Length)
        {
            return CharState.Pending;
        }

        if (index >= Target.Length)
        {
            return CharState.Extra;
        }

        return _typed[index] == Target[index] ? CharState.Correct : CharState.Incorrect;
    }

    public int CorrectChars => CountStates(CharState.Correct);
    public int IncorrectChars => CountStates(CharState.Incorrect);
    public int ExtraChars => Math.Max(0, _typed.Length - Target.Length);
    public int MissedChars => IsSubmitted ? Math.Max(0, Target.Length - _typed.Length) : 0;

    /// <summary>True when everything typed so far matches the start of the target.</summary>
    public bool IsOnTrack => Target.StartsWith(Typed, StringComparison.Ordinal);

    /// <summary>Typing speed for this single command, or null if it was not completed correctly.</summary>
    public double? Wpm
    {
        get
        {
            if (!IsSubmitted || !IsCorrect || FirstKeyAt is null || SubmittedAt is null)
            {
                return null;
            }

            return StatsCalculator.Wpm(Target.Length + 1, SubmittedAt.Value - FirstKeyAt.Value);
        }
    }

    internal bool Append(char c, TimeSpan at)
    {
        if (_typed.Length >= Target.Length + MaxExtra)
        {
            return false;
        }

        var correct = WouldBeCorrect(c);
        FirstKeyAt ??= at;
        _typed.Append(c);
        if (!correct)
        {
            Mistakes++;
        }

        return true;
    }

    internal void RegisterRejected(TimeSpan at)
    {
        FirstKeyAt ??= at;
        Mistakes++;
    }

    internal void Backspace()
    {
        if (_typed.Length > 0)
        {
            _typed.Length--;
        }
    }

    internal void Submit(TimeSpan at)
    {
        FirstKeyAt ??= at;
        SubmittedAt = at;
        IsSubmitted = true;
    }

    private int CountStates(CharState state)
    {
        var count = 0;
        for (var i = 0; i < _typed.Length; i++)
        {
            if (StateAt(i) == state)
            {
                count++;
            }
        }

        return count;
    }
}

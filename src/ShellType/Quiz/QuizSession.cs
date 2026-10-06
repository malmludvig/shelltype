namespace ShellType.Quiz;

public enum QuizPhase
{
    /// <summary>Typing a filter and picking an option.</summary>
    Answering,

    /// <summary>Showing whether the answer was right, waiting for Enter.</summary>
    Revealed,

    Finished,
}

/// <summary>An option that survives the current filter, with the characters that matched.</summary>
public sealed record VisibleOption(int Index, string Text, IReadOnlyList<int> Highlights);

/// <summary>What happened on one question, for the results screen.</summary>
public sealed record QuizAnswer(QuizQuestion Question, int ChosenIndex, int Points, TimeSpan Time)
{
    public bool Correct => ChosenIndex == Question.AnswerIndex;
}

/// <summary>
/// Runs a quiz. Options are picked fzf-style: type to fuzzy-filter, move the selection,
/// press Enter. Fast answers earn a bonus and a streak of right answers builds a combo.
/// </summary>
public sealed class QuizSession
{
    public const int BasePoints = 100;
    public const int MaxSpeedBonus = 100;
    public const int MaxComboLevel = 4;

    private readonly TimeProvider _time;
    private readonly List<QuizAnswer> _answers = [];
    private long _questionStarted;

    public QuizSession(IReadOnlyList<QuizQuestion> questions, TimeProvider? time = null)
    {
        if (questions.Count == 0)
        {
            throw new ArgumentException("A quiz needs at least one question.", nameof(questions));
        }

        Questions = questions;
        _time = time ?? TimeProvider.System;
        StartQuestion();
    }

    public IReadOnlyList<QuizQuestion> Questions { get; }
    public int Index { get; private set; }
    public QuizQuestion Current => Questions[Index];
    public QuizPhase Phase { get; private set; }
    public string Query { get; private set; } = "";
    public IReadOnlyList<VisibleOption> Visible { get; private set; } = [];

    /// <summary>Index into <see cref="Visible"/> of the highlighted option.</summary>
    public int Selected { get; private set; }

    public int Score { get; private set; }
    public int Streak { get; private set; }
    public int BestStreak { get; private set; }
    public IReadOnlyList<QuizAnswer> Answers => _answers;
    public QuizAnswer? LastAnswer => _answers.Count > 0 ? _answers[^1] : null;
    public int CorrectCount => _answers.Count(a => a.Correct);

    /// <summary>Score multiplier for the next right answer: x1, x1.5, x2, x2.5, x3.</summary>
    public double Multiplier => 1 + Math.Min(Streak, MaxComboLevel) * 0.5;

    public void Type(char c)
    {
        if (Phase == QuizPhase.Answering)
        {
            SetQuery(Query + c);
        }
    }

    public void Backspace()
    {
        if (Phase == QuizPhase.Answering && Query.Length > 0)
        {
            SetQuery(Query[..^1]);
        }
    }

    public void DeleteWord()
    {
        if (Phase != QuizPhase.Answering)
        {
            return;
        }

        var trimmed = Query.TrimEnd();
        var space = trimmed.LastIndexOf(' ');
        SetQuery(space < 0 ? "" : trimmed[..(space + 1)]);
    }

    public void ClearQuery()
    {
        if (Phase == QuizPhase.Answering)
        {
            SetQuery("");
        }
    }

    /// <summary>Moves the highlight up (negative) or down (positive), wrapping around.</summary>
    public void Move(int delta)
    {
        if (Phase == QuizPhase.Answering && Visible.Count > 0)
        {
            Selected = ((Selected + delta) % Visible.Count + Visible.Count) % Visible.Count;
        }
    }

    /// <summary>Enter: answers the highlighted option, or moves on after the reveal.</summary>
    public void Submit()
    {
        switch (Phase)
        {
            case QuizPhase.Answering when Visible.Count > 0:
                Answer(Visible[Selected].Index);
                break;
            case QuizPhase.Revealed:
                Next();
                break;
        }
    }

    private void Answer(int chosen)
    {
        var elapsed = _time.GetElapsedTime(_questionStarted);
        var points = 0;
        if (chosen == Current.AnswerIndex)
        {
            var speedBonus = Math.Max(0, MaxSpeedBonus - (int)(elapsed.TotalSeconds * 10));
            points = (int)Math.Round((BasePoints + speedBonus) * Multiplier);
            Score += points;
            Streak++;
            BestStreak = Math.Max(BestStreak, Streak);
        }
        else
        {
            Streak = 0;
        }

        _answers.Add(new QuizAnswer(Current, chosen, points, elapsed));
        Phase = QuizPhase.Revealed;
    }

    private void Next()
    {
        if (Index == Questions.Count - 1)
        {
            Phase = QuizPhase.Finished;
            return;
        }

        Index++;
        StartQuestion();
    }

    private void StartQuestion()
    {
        Phase = QuizPhase.Answering;
        _questionStarted = _time.GetTimestamp();
        SetQuery("");
    }

    private void SetQuery(string query)
    {
        Query = query;
        Visible = Current.Options
            .Select((text, index) => (text, index, match: FuzzyMatcher.Match(query, text)))
            .Where(o => o.match is not null)
            .OrderByDescending(o => o.match!.Score)
            .ThenBy(o => o.index)
            .Select(o => new VisibleOption(o.index, o.text, o.match!.Positions))
            .ToList();
        Selected = 0;
    }
}

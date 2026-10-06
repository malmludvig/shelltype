namespace ShellType.Game;

/// <summary>
/// Pure functions for typing statistics. A "word" is five characters, as is
/// standard for WPM measurements.
/// </summary>
public static class StatsCalculator
{
    public const double CharsPerWord = 5.0;

    public static double Wpm(int chars, TimeSpan duration) =>
        duration <= TimeSpan.Zero ? 0 : chars / CharsPerWord / duration.TotalMinutes;

    public static double Accuracy(int correct, int total) =>
        total == 0 ? 100 : 100.0 * correct / total;

    /// <summary>
    /// How steady the typing speed was, from 0 to 100. Uses the coefficient of
    /// variation squashed through a tanh-like curve so that small wobbles barely
    /// matter and wild swings approach zero.
    /// </summary>
    public static double Consistency(IReadOnlyList<double> values)
    {
        if (values.Count < 2)
        {
            return 100;
        }

        var mean = values.Average();
        if (mean <= 0)
        {
            return 0;
        }

        var variance = values.Sum(v => (v - mean) * (v - mean)) / values.Count;
        var cov = Math.Sqrt(variance) / mean;
        var squashed = Math.Tanh(cov + Math.Pow(cov, 3) / 3 + Math.Pow(cov, 5) / 5);
        return Math.Clamp(100 * (1 - squashed), 0, 100);
    }

    /// <summary>Buckets keystrokes into one sample per second of the test.</summary>
    public static IReadOnlyList<SecondSample> Samples(IReadOnlyList<Keystroke> keystrokes, TimeSpan duration)
    {
        var samples = new List<SecondSample>();
        var totalSeconds = duration.TotalSeconds;
        var seconds = (int)Math.Ceiling(totalSeconds);
        var correctSoFar = 0;
        var index = 0;

        for (var s = 1; s <= seconds; s++)
        {
            var end = Math.Min(s, totalSeconds);
            var windowLength = end - (s - 1);

            // A sliver of a final second gives absurd per-second numbers; drop it.
            if (windowLength < 0.5 && s > 1)
            {
                break;
            }

            var inWindow = 0;
            var errors = 0;
            while (index < keystrokes.Count && keystrokes[index].At.TotalSeconds <= end)
            {
                inWindow++;
                if (keystrokes[index].Correct)
                {
                    correctSoFar++;
                }
                else
                {
                    errors++;
                }

                index++;
            }

            var wpm = Wpm(correctSoFar, TimeSpan.FromSeconds(end));
            var raw = Wpm(inWindow, TimeSpan.FromSeconds(windowLength));
            samples.Add(new SecondSample(s, wpm, raw, errors));
        }

        return samples;
    }

    /// <summary>Net WPM so far, for the live counter while typing.</summary>
    public static double LiveWpm(TypingSession session) =>
        session.State == SessionState.Ready ? 0 : Wpm(CountChars(session.Attempts).WpmChars, session.Elapsed);

    /// <summary>
    /// WPM chars are whole commands typed right (+1 for Enter) plus a clean partial
    /// command; raw chars are everything typed.
    /// </summary>
    private static (int WpmChars, int RawChars) CountChars(IEnumerable<CommandAttempt> attempts)
    {
        var wpmChars = 0;
        var rawChars = 0;
        foreach (var attempt in attempts)
        {
            rawChars += attempt.TypedLength + (attempt.IsSubmitted ? 1 : 0);
            if (attempt.IsSubmitted && attempt.IsCorrect)
            {
                wpmChars += attempt.Target.Length + 1;
            }
            else if (!attempt.IsSubmitted && attempt.IsOnTrack)
            {
                wpmChars += attempt.TypedLength;
            }
        }

        return (wpmChars, rawChars);
    }

    public static TestResult Compute(TypingSession session)
    {
        var duration = session.Elapsed;
        var typed = session.Attempts.Where(a => a.TypedLength > 0 || a.IsSubmitted).ToList();
        var (wpmChars, rawChars) = CountChars(typed);
        var keystrokes = session.Keystrokes;
        var samples = Samples(keystrokes, duration);

        return new TestResult
        {
            CompletedAt = session.CompletedAt ?? DateTimeOffset.UtcNow,
            Mode = session.Config.Mode,
            ModeValue = session.Config.ModeValue,
            Difficulty = session.Config.MaxDifficulty,
            Duration = duration,
            Wpm = Wpm(wpmChars, duration),
            RawWpm = Wpm(rawChars, duration),
            Accuracy = Accuracy(keystrokes.Count(k => k.Correct), keystrokes.Count),
            Consistency = Consistency(samples.Select(s => s.Raw).ToList()),
            CorrectChars = typed.Sum(a => a.CorrectChars),
            IncorrectChars = typed.Sum(a => a.IncorrectChars),
            ExtraChars = typed.Sum(a => a.ExtraChars),
            MissedChars = typed.Sum(a => a.MissedChars),
            Samples = samples,
            Commands = typed
                .Select(a => new CommandResult(a.Target, a.Typed, a.IsSubmitted && a.IsCorrect, a.Mistakes, a.Wpm))
                .ToList(),
        };
    }
}

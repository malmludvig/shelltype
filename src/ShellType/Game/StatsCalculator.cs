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
}

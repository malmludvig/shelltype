using System.Text.Json.Serialization;
using ShellType.Game;
using ShellType.Models;

namespace ShellType.Services;

/// <summary>A compact record of a finished test, small enough to keep hundreds of in localStorage.</summary>
public sealed record HistoryEntry(
    DateTimeOffset At,
    TestMode Mode,
    int ModeValue,
    Difficulty Difficulty,
    double Wpm,
    double Raw,
    double Accuracy,
    double Consistency,
    double Seconds)
{
    [JsonIgnore]
    public string ModeLabel => Mode == TestMode.Time ? $"time {ModeValue}" : $"commands {ModeValue}";
}

/// <summary>Running totals for one command across every test.</summary>
public sealed class CommandStat
{
    public int Attempts { get; set; }
    public int Failed { get; set; }
    public int Mistakes { get; set; }
    public double BestWpm { get; set; }

    /// <summary>Average mistakes per attempt, with failed submissions weighing extra.</summary>
    [JsonIgnore]
    public double Weakness => Attempts == 0 ? 0 : (Mistakes + 2.0 * Failed) / Attempts;
}

/// <summary>
/// Stores test history and per-command statistics, and answers questions like
/// "is this a personal best?" and "which commands do I struggle with?".
/// </summary>
public sealed class HistoryService(BrowserStorage storage)
{
    public const int MaxEntries = 500;
    private const string HistoryKey = "shelltype.history";
    private const string CommandsKey = "shelltype.commands";

    private List<HistoryEntry>? _history;
    private Dictionary<string, CommandStat>? _commands;

    public async Task<IReadOnlyList<HistoryEntry>> GetHistoryAsync()
    {
        _history ??= await storage.GetAsync<List<HistoryEntry>>(HistoryKey) ?? [];
        return _history;
    }

    public async Task<IReadOnlyDictionary<string, CommandStat>> GetCommandStatsAsync()
    {
        _commands ??= await storage.GetAsync<Dictionary<string, CommandStat>>(CommandsKey) ?? [];
        return _commands;
    }

    /// <summary>Best WPM for this exact mode, value and difficulty, if any.</summary>
    public async Task<double?> GetPersonalBestAsync(TestMode mode, int modeValue, Difficulty difficulty)
    {
        var history = await GetHistoryAsync();
        var matching = history.Where(h => h.Mode == mode && h.ModeValue == modeValue && h.Difficulty == difficulty).ToList();
        return matching.Count == 0 ? null : matching.Max(h => h.Wpm);
    }

    /// <summary>Saves a result. Returns true if it beat the previous personal best.</summary>
    public async Task<bool> RecordAsync(TestResult result)
    {
        if (!result.IsValid)
        {
            return false;
        }

        var previousBest = await GetPersonalBestAsync(result.Mode, result.ModeValue, result.Difficulty);

        var history = (List<HistoryEntry>)await GetHistoryAsync();
        history.Add(new HistoryEntry(
            result.CompletedAt,
            result.Mode,
            result.ModeValue,
            result.Difficulty,
            Math.Round(result.Wpm, 2),
            Math.Round(result.RawWpm, 2),
            Math.Round(result.Accuracy, 2),
            Math.Round(result.Consistency, 2),
            Math.Round(result.Duration.TotalSeconds, 2)));
        if (history.Count > MaxEntries)
        {
            history.RemoveRange(0, history.Count - MaxEntries);
        }

        var commands = (Dictionary<string, CommandStat>)await GetCommandStatsAsync();
        foreach (var command in result.Commands.Where(c => c.Finished))
        {
            if (!commands.TryGetValue(command.Text, out var stat))
            {
                stat = new CommandStat();
                commands[command.Text] = stat;
            }

            stat.Attempts++;
            stat.Mistakes += command.Mistakes;
            if (!command.Correct)
            {
                stat.Failed++;
            }

            if (command.Wpm is { } wpm && wpm > stat.BestWpm)
            {
                stat.BestWpm = Math.Round(wpm, 2);
            }
        }

        await storage.SetAsync(HistoryKey, history);
        await storage.SetAsync(CommandsKey, commands);

        return previousBest is null || result.Wpm > previousBest;
    }

    /// <summary>The commands you get wrong most often, worst first.</summary>
    public async Task<IReadOnlyList<(string Text, CommandStat Stat)>> GetWeakestAsync(int count)
    {
        var commands = await GetCommandStatsAsync();
        return commands
            .Where(kv => kv.Value.Weakness > 0)
            .OrderByDescending(kv => kv.Value.Weakness)
            .ThenByDescending(kv => kv.Value.Attempts)
            .Take(count)
            .Select(kv => (kv.Key, kv.Value))
            .ToList();
    }

    public async Task ClearAsync()
    {
        _history = [];
        _commands = [];
        await storage.RemoveAsync(HistoryKey);
        await storage.RemoveAsync(CommandsKey);
    }
}

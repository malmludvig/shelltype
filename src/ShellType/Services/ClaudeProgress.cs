using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShellType.Services;

/// <summary>
/// What you have learned about Claude Code: the entries you know, the flashcard boxes,
/// the quiz questions you missed and how many days in a row you have practiced.
/// Entries are identified by their key, like <c>/compact</c>.
/// </summary>
public sealed class ClaudeProgress
{
    /// <summary>Flashcards go up a box when you know them and back to box 1 when you do not; higher boxes come up less often.</summary>
    public const int MaxBox = 5;

    public List<string> Known { get; set; } = [];
    public Dictionary<string, int> Boxes { get; set; } = [];
    public List<string> Missed { get; set; } = [];

    /// <summary>The last day you practiced, as yyyy-MM-dd.</summary>
    public string? LastDay { get; set; }

    public int DayStreak { get; set; }

    public int BoxOf(string key) => Boxes.TryGetValue(key, out var box) ? box : 1;

    /// <summary>Leitner: a known card moves up a box, a missed one goes back to box 1.</summary>
    public void Rate(string key, bool knew) => Boxes[key] = knew ? Math.Min(MaxBox, BoxOf(key) + 1) : 1;

    public bool IsKnown(string key) => Known.Contains(key);

    public void SetKnown(string key, bool known)
    {
        Known.Remove(key);
        if (known)
        {
            Known.Add(key);
        }
    }

    /// <summary>Remembers a quiz answer: a miss is kept for the "missed" quiz until you get it right.</summary>
    public void RecordAnswer(string key, bool correct)
    {
        Missed.Remove(key);
        if (!correct)
        {
            Missed.Add(key);
        }
    }

    /// <summary>The first practice of a new day extends the streak if you practiced yesterday, otherwise starts it over.</summary>
    public void MarkPracticed(DateOnly today)
    {
        var todayKey = Format(today);
        if (LastDay == todayKey)
        {
            return;
        }

        DayStreak = LastDay == Format(today.AddDays(-1)) ? DayStreak + 1 : 1;
        LastDay = todayKey;
    }

    /// <summary>The streak still counts today if you practiced today or yesterday.</summary>
    public int CurrentStreak(DateOnly today) =>
        LastDay == Format(today) || LastDay == Format(today.AddDays(-1)) ? DayStreak : 0;

    private static string Format(DateOnly day) => day.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
}

/// <summary>Loads and saves <see cref="ClaudeProgress"/>, and moves it between browsers as JSON.</summary>
public sealed class ClaudeProgressService(BrowserStorage storage, TimeProvider time)
{
    private const string Key = "shelltype.claude";

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private ClaudeProgress? _progress;

    public DateOnly Today => DateOnly.FromDateTime(time.GetLocalNow().DateTime);

    public async Task<ClaudeProgress> GetAsync() =>
        _progress ??= await storage.GetAsync<ClaudeProgress>(Key) ?? new ClaudeProgress();

    public async Task UpdateAsync(Action<ClaudeProgress> change)
    {
        var progress = await GetAsync();
        change(progress);
        await storage.SetAsync(Key, progress);
    }

    /// <summary>Applies a change that counts as practice, so it also extends the day streak.</summary>
    public Task PracticeAsync(Action<ClaudeProgress> change) =>
        UpdateAsync(p =>
        {
            change(p);
            p.MarkPracticed(Today);
        });

    public async Task<string> ExportAsync() => JsonSerializer.Serialize(await GetAsync(), Json);

    /// <summary>Replaces the progress with an export. Returns false, and changes nothing, if the text is not one.</summary>
    public async Task<bool> ImportAsync(string text)
    {
        ClaudeProgress? imported;
        try
        {
            imported = JsonSerializer.Deserialize<ClaudeProgress>(text, Json);
        }
        catch (JsonException)
        {
            return false;
        }

        if (imported is null)
        {
            return false;
        }

        _progress = imported;
        await storage.SetAsync(Key, imported);
        return true;
    }

    public async Task ResetAsync()
    {
        _progress = new ClaudeProgress();
        await storage.RemoveAsync(Key);
    }
}

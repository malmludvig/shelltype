using System.Globalization;
using Microsoft.JSInterop;

namespace ShellType.Services;

/// <summary>
/// Owns the user's settings: loads them once, saves on change and pushes
/// visual settings (theme, font size) to the document.
/// </summary>
public sealed class SettingsService(BrowserStorage storage, IJSRuntime js)
{
    private const string Key = "shelltype.settings";
    private bool _loaded;

    public event Action? Changed;

    public UserSettings Current { get; private set; } = new();

    public async Task LoadAsync()
    {
        if (_loaded)
        {
            return;
        }

        Current = await storage.GetAsync<UserSettings>(Key) ?? new UserSettings();
        _loaded = true;
        await ApplyAsync();
    }

    public async Task UpdateAsync(Action<UserSettings> change)
    {
        change(Current);
        await storage.SetAsync(Key, Current);
        await ApplyAsync();
        Changed?.Invoke();
    }

    public async Task ResetAsync()
    {
        Current = new UserSettings();
        await storage.RemoveAsync(Key);
        await ApplyAsync();
        Changed?.Invoke();
    }

    private async Task ApplyAsync()
    {
        await js.InvokeVoidAsync("shellType.ui.setTheme", Current.Theme);
        await js.InvokeVoidAsync(
            "shellType.ui.setCssVar",
            "--type-size",
            Current.FontSize.ToString("0.##", CultureInfo.InvariantCulture) + "rem");
    }
}

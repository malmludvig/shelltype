using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.JSInterop;

namespace ShellType.Services;

/// <summary>
/// JSON-over-localStorage. Missing or corrupt values come back as null rather than throwing.
/// </summary>
public sealed class BrowserStorage(IJSRuntime js)
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    };

    public async Task<T?> GetAsync<T>(string key)
    {
        var raw = await js.InvokeAsync<string?>("shellType.storage.get", key);
        if (string.IsNullOrEmpty(raw))
        {
            return default;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(raw, Json);
        }
        catch (JsonException)
        {
            return default;
        }
    }

    public ValueTask SetAsync<T>(string key, T value) =>
        js.InvokeVoidAsync("shellType.storage.set", key, JsonSerializer.Serialize(value, Json));

    public ValueTask RemoveAsync(string key) =>
        js.InvokeVoidAsync("shellType.storage.remove", key);
}

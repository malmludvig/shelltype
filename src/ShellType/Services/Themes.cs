namespace ShellType.Services;

/// <summary>A theme defined in themes.css, with a few colors for the preview swatch.</summary>
public sealed record ThemeInfo(string Id, string Background, string Main, string Text);

public static class Themes
{
    public static readonly IReadOnlyList<ThemeInfo> All =
    [
        new("shelltype", "#16181d", "#7ee787", "#d6dde6"),
        new("serika", "#323437", "#e2b714", "#d1d0c5"),
        new("dracula", "#282a36", "#bd93f9", "#f8f8f2"),
        new("nord", "#2e3440", "#88c0d0", "#d8dee9"),
        new("gruvbox", "#282828", "#fabd2f", "#ebdbb2"),
        new("matrix", "#000000", "#15ff00", "#d1ffcd"),
        new("solarized", "#fdf6e3", "#268bd2", "#586e75"),
        new("paper", "#eeeeee", "#444444", "#444444"),
    ];
}

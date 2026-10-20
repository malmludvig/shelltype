using ShellType.Models;

namespace ShellType.Data;

public static partial class ClaudeLibrary
{
    private static readonly ClaudeCommand[] Shortcuts =
    [
        Key("Shift+Tab", "Cycle the permission mode: default, acceptEdits, plan (and auto)",
            note: "Windows without VT mode: Alt+M", keywords: "mode permissions plan"),
        Key("Esc", "Interrupt Claude mid-task; what is already done is kept", keywords: "stop interrupt cancel"),
        Key("Esc Esc", "On an empty prompt, open the rewind menu; with text, clear it and keep it as a draft", keywords: "rewind undo"),
        Key("Ctrl+C", "Cancel the current operation or clear the input; twice on an empty prompt quits", keywords: "stop cancel quit"),
        Key("Ctrl+D", "Quit the session (press twice)", keywords: "exit"),
        Key("Ctrl+R", "Search backwards through your command history", keywords: "history search"),
        Key("Ctrl+O", "Show or hide the transcript with detailed tool use", keywords: "verbose details"),
        Key("Ctrl+T", "Show or hide Claude's task list (todos)", keywords: "todo"),
        Key("Ctrl+B", "Send a running task to the background"),
        Key("Ctrl+G", "Open the prompt in your usual text editor", keywords: "editor vim long prompt"),
        Key("Ctrl+S", "Stash the prompt; press it again on an empty prompt to bring it back", keywords: "save draft"),
        Key("Ctrl+L", "Redraw the screen"),
        Key("Ctrl+V", "Paste an image from the clipboard", note: "Windows/WSL: Alt+V", keywords: "image screenshot picture"),
        Key("Ctrl+J", "New line in the prompt, works in every terminal"),
        Key("\\ + Enter", "New line in the prompt, the quick way that works everywhere"),
        Key("Shift+Enter", "New line in the prompt, built in to Windows Terminal, iTerm2, Ghostty and others"),
        Key("Alt+P", "Switch model without clearing the prompt", note: "macOS: Option+P"),
        Key("Alt+T", "Turn extended thinking on or off", note: "macOS: Option+T"),
        Key("Alt+O", "Turn fast mode on or off", note: "macOS: Option+O"),
        Key("Up / Down", "Scroll through command history, also from earlier sessions in the same project", keywords: "arrows history"),
        Key("Tab", "Accept the autocomplete suggestion"),
        Key("Ctrl+_", "Undo the last edit in the input"),
    ];

    private static readonly ClaudeCommand[] Prefixes =
    [
        Cmd(ClaudeKind.Prefix, "/", "At the start of the prompt: run a command or a skill", null, null, null, "commands"),
        Cmd(ClaudeKind.Prefix, "!", "At the start of the prompt: shell mode, run the command and add its output to the conversation",
            null, "Example: ! git status", "! git status", "terminal shell bash run command"),
        Cmd(ClaudeKind.Prefix, "@", "Mention a file or folder with autocomplete, like @src/app.js",
            null, null, "explain @src/app.js", "file reference attach"),
        Cmd(ClaudeKind.Prefix, "?", "On an empty prompt: show or hide the shortcuts panel", null, null, null, "help keys"),
    ];

    private static ClaudeCommand Key(string name, string description, string? note = null, string keywords = "") =>
        Cmd(ClaudeKind.Shortcut, name, description, null, note, null, keywords);
}

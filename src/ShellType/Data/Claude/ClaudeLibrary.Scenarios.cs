using ShellType.Models;

namespace ShellType.Data;

public static partial class ClaudeLibrary
{
    /// <summary>Everyday situations and the command that solves each, for the scenario quiz.</summary>
    public static IReadOnlyList<ClaudeScenario> Scenarios { get; } =
    [
        new("The context is filling up, but you want to keep going with the same task.", "/compact",
            "It summarizes the conversation, so you free up room without losing the thread."),
        new("You want Claude to think a big change through without touching any files yet.", "/plan",
            "In plan mode Claude reads and plans but changes nothing until you approve."),
        new("Claude is heading the wrong way in the middle of a reply.", "Esc",
            "Esc interrupts at once but keeps what has already been done."),
        new("Claude edited the wrong files and you want the code back the way it was.", "/rewind",
            "It rolls the conversation and/or the code back to an earlier point."),
        new("You are starting a brand new task that has nothing to do with the last one.", "/clear",
            "An empty context gives better answers than one full of old work."),
        new("Claude should always remember that the project uses pnpm, not npm.", "CLAUDE.md",
            "CLAUDE.md is read at the start of every session."),
        new("You want to run git status yourself and let Claude see the result.", "!",
            "Shell mode runs the command directly and adds its output to the conversation."),
        new("You want to run Claude in a CI script and only get the answer back.", "claude -p \"query\"",
            "Print mode runs one query, prints the answer and exits."),
        new("You closed the terminal and want to continue yesterday's conversation in the same folder.", "claude -c",
            "-c continues the latest conversation in the current directory."),
        new("Claude should be able to read from your database or GitHub through an external tool.", "MCP",
            "MCP servers connect Claude to external tools and data."),
        new("A lint script should run automatically every time Claude has edited a file.", "Hooks",
            "A PostToolUse hook runs after every tool call, without Claude having to remember it."),
        new("Claude asks for permission on every file edit and you trust the edits.", "acceptEdits",
            "acceptEdits approves file edits automatically but still asks about commands."),
        new("You want to see everything Claude changed before you commit.", "/diff",
            "It shows the changes in the working directory."),
        new("You want to switch to a faster model for a simple task.", "/model",
            "It switches the model and keeps the choice as the default."),
        new("You wonder how much of your usage limit the session has used.", "/usage",
            "It shows cost, limits and stats."),
        new("You have a screenshot of an error message that Claude should look at.", "Ctrl+V",
            "It pastes the image from the clipboard straight into the prompt."),
        new("You want to point Claude at a file without pasting its content.", "@",
            "@ with autocomplete references the file."),
        new("You want to find bugs in your change before you push.", "/code-review",
            "It reviews the diff for correctness bugs."),
        new("Something seems broken with the installation or the settings.", "/doctor",
            "The health check finds common problems and suggests fixes."),
        new("You want to write a long prompt in your usual text editor.", "Ctrl+G",
            "It opens the prompt in your editor."),
        new("The same check should run every five minutes while the session is open.", "/loop",
            "It runs a prompt over and over at an interval, like /loop 5m."),
        new("You have a quick side question but do not want to fill up the context.", "/btw",
            "The side question is not added to the conversation history."),
        new("You want to switch to plan mode quickly without typing a command.", "Shift+Tab",
            "Shift+Tab cycles through the permission modes."),
        new("Claude should be able to read a folder outside the project during this session.", "/add-dir",
            "It adds an extra working directory."),
    ];
}

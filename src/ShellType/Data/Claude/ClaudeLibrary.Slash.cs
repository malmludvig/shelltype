using ShellType.Models;

namespace ShellType.Data;

public static partial class ClaudeLibrary
{
    private static readonly ClaudeCommand[] Basics =
    [
        Slash("/help", "Show help and every available command"),
        Slash("/clear", "Start a new conversation with an empty context; the old one is kept and can be resumed",
            aliases: "/reset, /new", keywords: "reset new chat start over empty"),
        Slash("/compact [instructions]", "Free up context by summarizing the conversation so far, optionally with what to focus on",
            example: "/compact keep only the database decisions", keywords: "full context memory summarize tokens"),
        Slash("/context", "Show how much of the context window is used, as a colored grid with tips to trim it",
            keywords: "tokens memory full"),
        Slash("/init", "Create a CLAUDE.md that describes the project for Claude",
            keywords: "start project document setup"),
        Slash("/memory", "Edit CLAUDE.md files and turn auto memory on, off or view it",
            keywords: "remember memory instructions"),
        Slash("/model [model]", "Switch the AI model and keep it as the default for new sessions",
            example: "/model opus", keywords: "change model opus sonnet haiku fable"),
        Slash("/effort [level]", "Set how hard Claude thinks: low, medium, high, xhigh, max or auto",
            example: "/effort high", keywords: "thinking reasoning"),
        Slash("/fast [on|off]", "Turn fast mode on or off: the same model, faster output",
            example: "/fast on", keywords: "speed quick"),
        Slash("/config", "Open the settings, or set a value directly, like /config model=sonnet",
            aliases: "/settings", example: "/config model=sonnet", keywords: "settings preferences options"),
        Slash("/status", "Open the settings on the status tab: version, model, account and connection",
            keywords: "version account"),
        Slash("/usage", "Show the session's cost, your usage limits and stats",
            aliases: "/cost, /stats", keywords: "price cost money limit billing"),
        Slash("/exit", "Quit Claude Code", aliases: "/quit", keywords: "close leave"),
        Slash("/resume [session]", "Resume an earlier conversation by id or name, or pick one from a list",
            aliases: "/continue", example: "/resume vlan-lab", keywords: "continue old conversation"),
        Slash("/rename [name]", "Rename the session; with no name one is generated for you",
            example: "/rename vlan-lab", keywords: "name session"),
        Slash("/rewind", "Roll the conversation and/or the code back to an earlier point",
            aliases: "/checkpoint, /undo", keywords: "undo revert restore go back"),
        Slash("/branch [name]", "Branch the conversation from here to try another direction",
            example: "/branch try-redis", keywords: "fork alternative"),
        Slash("/fork [prompt]", "Copy the conversation into a new background session and keep working here",
            example: "/fork write the migration script", keywords: "parallel copy"),
        Slash("/btw [question]", "Ask a side question that is not added to the conversation history",
            example: "/btw what does the -z flag do in tar?", keywords: "aside quick question"),
        Slash("/plan [description]", "Go straight into plan mode: Claude plans but changes nothing",
            example: "/plan add rate limiting to the login endpoint", keywords: "plan think first design"),
        Slash("/diff", "Review the changes in the working directory, including what Claude changed",
            keywords: "changes git review"),
        Slash("/copy [N]", "Copy the last reply, or the Nth latest, to the clipboard",
            example: "/copy 2", keywords: "clipboard"),
        Slash("/export [file]", "Export the conversation as text, to a file or through a dialog",
            example: "/export session.txt", keywords: "save transcript"),
        Slash("/recap", "Write a one-line summary of the session", keywords: "summary"),
        Slash("/goal [condition]", "Set a goal Claude keeps working toward over several turns until the condition holds",
            example: "/goal all tests pass", keywords: "target until"),
        Slash("/add-dir <path>", "Add an extra working directory that Claude may read and change",
            example: "/add-dir ../shared-lib", keywords: "folder directory access"),
        Slash("/cd <path>", "Move the session to a new working directory but keep the conversation",
            example: "/cd ~/code/api", keywords: "change directory"),
    ];

    private static readonly ClaudeCommand[] Setup =
    [
        Slash("/permissions", "Manage the rules for which tools are allowed, asked about or denied",
            aliases: "/allowed-tools", keywords: "allow deny permissions rights"),
        Slash("/hooks", "Show the hook configurations: scripts that run on events", keywords: "automate events"),
        Slash("/mcp", "Manage connections to MCP servers and their logins", keywords: "servers tools integrations"),
        Slash("/agents", "Explains how to create and manage subagents in .claude/agents/", keywords: "subagents"),
        Slash("/skills", "List the available skills, sort them by token cost and set their visibility"),
        Slash("/plugin", "Manage plugins: list, install, enable and disable them", keywords: "extensions"),
        Slash("/reload-skills", "Load skills and commands that were added during the session"),
        Slash("/output-style [style]", "List the output styles or switch to one, like concise",
            example: "/output-style concise"),
        Slash("/statusline", "Set up the status line at the bottom"),
        Slash("/theme", "Switch the color theme: light, dark, colorblind friendly, ANSI and more", keywords: "colors dark light"),
        Slash("/keybindings", "Open the file with your own keyboard shortcuts", keywords: "keys shortcuts"),
        Slash("/terminal-setup", "Install Shift+Enter for a new line in terminals like VS Code"),
        Slash("/ide", "Manage IDE integrations and show their status", keywords: "vs code jetbrains editor"),
        Slash("/doctor", "Check the installation, settings and CLAUDE.md, and suggest fixes",
            aliases: "/checkup", keywords: "troubleshoot broken not working diagnose"),
        Slash("/sandbox", "Turn sandbox mode on or off, where it is supported"),
        Slash("/login", "Log in to your Anthropic account"),
        Slash("/logout", "Log out of your Anthropic account"),
        Slash("/release-notes", "Show the changelog for each version", keywords: "changelog new"),
        Slash("/powerup", "Learn Claude Code features through short interactive lessons", keywords: "tutorial learn"),
        Slash("/feedback [report]", "Send feedback about Claude Code"),
        Slash("/bug [report]", "Report a bug or share the conversation", aliases: "/share",
            example: "/bug the diff view freezes on large files"),
        Slash("/autocompact [auto|tokens]", "Set when the context is summarized automatically, like 500k",
            example: "/autocompact 500k"),
        Slash("/color [color]", "Change the color of the prompt line, like blue or green", example: "/color green"),
        Slash("/tui [default|fullscreen]", "Switch the terminal interface and restart with the conversation kept",
            example: "/tui fullscreen"),
        Slash("/focus", "Focus view: show only the last prompt, a tool summary and the reply (fullscreen mode)"),
        Slash("/voice [hold|tap|off]", "Turn voice dictation on or off", example: "/voice hold", keywords: "speech dictate"),
        Slash("/chrome", "Set up Claude in Chrome to drive the browser", keywords: "browser"),
        Slash("/insights", "Make an HTML report that analyzes your recent sessions"),
        Slash("/import [codex|gemini|cursor]", "Bring in configuration from other AI tools", example: "/import cursor"),
        Slash("/team-onboarding", "Write an onboarding guide for the team from the last 30 days of use"),
        Slash("/install-slack-app", "Install the Claude app for Slack"),
        Slash("/mobile", "Show a QR code to download the Claude app to your phone", aliases: "/ios, /android"),
        Slash("/upgrade", "Open the page to change your subscription plan"),
        Slash("/usage-credits", "Set up or request extra usage credits"),
        Slash("/stop", "Stop the background session you are attached to; the transcript and worktree are kept"),
    ];

    private static readonly ClaudeCommand[] Background =
    [
        Slash("/tasks", "Show and manage the background work in the session", aliases: "/bashes"),
        Slash("/background [prompt]", "Detach the session so it keeps running as a background agent",
            aliases: "/bg", example: "/background run the full test suite"),
        Slash("/subtask <task>", "Start a forked subagent that inherits the conversation and reports back",
            example: "/subtask find every caller of parseDate"),
        Slash("/schedule [description]", "Create and manage routines that run in the cloud on a schedule",
            aliases: "/routines", example: "/schedule every monday summarize open issues", keywords: "cron recurring"),
        Slash("/remote-control", "Make the session controllable from claude.ai", aliases: "/rc"),
        Slash("/teleport", "Pull a cloud session into this terminal", aliases: "/tp"),
        Slash("/desktop", "Continue the session in the Claude Code desktop app", aliases: "/app"),
        Slash("/install-github-app", "Install the Claude GitHub app for a repository"),
        Slash("/security-review", "Analyze the changes on the current branch for security holes", keywords: "vulnerabilities"),
    ];

    private static readonly ClaudeCommand[] Skills =
    [
        Skill("/code-review [level]", "Review your diff, a PR or a branch for bugs, at low to max, or ultra for a cloud review",
            aliases: "/review", example: "/code-review high --fix", keywords: "bugs review check"),
        Skill("/simplify", "Review changed code for simplifications and cleanups, then apply the fixes", keywords: "clean refactor"),
        Skill("/loop [interval] [prompt]", "Run a prompt over and over, like /loop 5m /foo",
            example: "/loop 5m check if the build is done", keywords: "repeat interval poll"),
        Skill("/batch <instruction>", "Make big parallel changes: split into 5-30 units with a subagent each",
            example: "/batch rename userId to accountId everywhere"),
        Skill("/debug [description]", "Turn on debug logging and troubleshoot by reading the session log",
            example: "/debug why do my hooks not run"),
        Skill("/update-config", "Describe a settings change and Claude edits the right settings.json"),
        Skill("/fewer-permission-prompts", "Find common read-only commands in your history and add them to an allowlist"),
        Skill("/run", "Start the project's app to see that a change works"),
    ];

    private static ClaudeCommand Slash(string name, string description, string? aliases = null, string? example = null, string keywords = "") =>
        Cmd(ClaudeKind.Slash, name, description, aliases, null, example, keywords);

    private static ClaudeCommand Skill(string name, string description, string? aliases = null, string? example = null, string keywords = "") =>
        Cmd(ClaudeKind.Skill, name, description, aliases, null, example, keywords);
}

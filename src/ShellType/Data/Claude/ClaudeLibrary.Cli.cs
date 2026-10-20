using ShellType.Models;

namespace ShellType.Data;

public static partial class ClaudeLibrary
{
    private static readonly ClaudeCommand[] Cli =
    [
        Cmd(ClaudeKind.Cli, "claude", "Start an interactive session", null, null, null, "start open"),
        Cmd(ClaudeKind.Cli, "claude \"query\"", "Start an interactive session with a first prompt",
            null, null, "claude \"explain this repo\"", ""),
        Cmd(ClaudeKind.Cli, "claude -p \"query\"", "Run a query without the interactive ui, print the answer and exit (good in scripts)",
            "--print", "Long form: --print", "claude -p \"summarize README.md\"", "script automate ci headless"),
        Cmd(ClaudeKind.Cli, "cat file | claude -p \"query\"", "Pipe content in and process it",
            null, null, "cat error.log | claude -p \"what went wrong?\"", "pipe stdin log"),
        Cmd(ClaudeKind.Cli, "claude -c", "Continue the latest conversation in the current directory",
            "--continue", "Long form: --continue", null, "continue yesterday"),
        Cmd(ClaudeKind.Cli, "claude -r \"session\"", "Resume a session by id or name, or open a picker",
            "--resume", "Long form: --resume", "claude -r vlan-lab", "resume"),
        Cmd(ClaudeKind.Cli, "claude update", "Update to the latest version", null, null, null, "upgrade"),
        Cmd(ClaudeKind.Cli, "claude doctor", "Print diagnostics about the installation and settings without starting a session",
            null, null, null, "troubleshoot"),
        Cmd(ClaudeKind.Cli, "claude mcp", "Configure MCP servers, like claude mcp add",
            null, null, "claude mcp add --transport http github https://api.githubcopilot.com/mcp/", "servers"),
        Cmd(ClaudeKind.Cli, "claude plugin", "Manage plugins from the terminal", null, null, "claude plugin list", ""),
        Cmd(ClaudeKind.Cli, "claude auth login", "Log in; with --console for API billing", null, null, null, "login"),
        Cmd(ClaudeKind.Cli, "claude auth status", "Show the login status as JSON", null, null, null, ""),
        Cmd(ClaudeKind.Cli, "claude agents", "Open the view of background sessions", null, null, null, "background"),
        Cmd(ClaudeKind.Cli, "claude attach <id>", "Attach to a background session", null, null, "claude attach 4f2a9c", ""),
        Cmd(ClaudeKind.Cli, "claude setup-token", "Create a long-lived OAuth token for CI and scripts", null, null, null, "ci token"),
    ];

    private static readonly ClaudeCommand[] Flags =
    [
        Flag("--model <model>", "Pick the model for the session: sonnet, opus, haiku, fable or a full name", "claude --model sonnet"),
        Flag("--permission-mode <mode>", "Start in a given permission mode, like plan or acceptEdits",
            "claude --permission-mode plan \"map out the refactor\""),
        Flag("--add-dir <paths>", "Give Claude access to more directories", "claude --add-dir ../apps ../lib"),
        Flag("--allowedTools", "Tools that may run without asking, like \"Bash(git log *)\" \"Read\"",
            "claude --allowedTools \"Bash(git log *)\" \"Read\""),
        Flag("--disallowedTools", "Tools that are denied or removed entirely", "claude --disallowedTools \"Bash(rm *)\""),
        Flag("--dangerously-skip-permissions", "Skip every permission prompt, the same as bypassPermissions; use with great care",
            "claude --dangerously-skip-permissions"),
        Flag("--append-system-prompt <text>", "Add text to the end of the default system prompt",
            "claude --append-system-prompt \"answer in swedish\""),
        Flag("--system-prompt <text>", "Replace the whole system prompt", "claude --system-prompt \"you are a code reviewer\""),
        Flag("--output-format <format>", "Output format in print mode: text, json or stream-json",
            "claude -p \"list the files\" --output-format json"),
        Flag("--max-turns <n>", "Limit the number of agent turns (print mode)", "claude -p \"fix the lint errors\" --max-turns 5"),
        Flag("--mcp-config <file>", "Load MCP servers from a JSON file", "claude --mcp-config mcp.json"),
        Flag("--worktree, -w [name]", "Start in an isolated git worktree", "claude -w feature-auth"),
        Flag("--effort <level>", "Set the thinking level for the session", "claude --effort max"),
        Flag("--name, -n <name>", "Name the session so you can use it with --resume later", "claude -n checkout-flow"),
        Flag("--verbose", "Verbose logging that shows everything turn by turn", "claude --verbose"),
        Flag("--debug", "Turn on debug mode, optionally filtered, like --debug=mcp", "claude --debug=mcp"),
        Flag("--safe-mode", "Start without customizations (CLAUDE.md, skills, hooks, MCP...); good for troubleshooting",
            "claude --safe-mode"),
        Flag("--version, -v", "Show the version number", "claude --version"),
    ];

    private static readonly ClaudeCommand[] Modes =
    [
        Mode("default", "The default mode (\"manual\"): Claude asks before edits and commands", "claude --permission-mode default"),
        Mode("acceptEdits", "File edits are approved automatically, but commands still ask", "claude --permission-mode acceptEdits"),
        Mode("plan", "Claude reads and plans but changes nothing until you approve the plan", "claude --permission-mode plan"),
        Mode("auto", "Claude works on its own with built-in safety rules instead of asking about everything",
            "claude --permission-mode auto"),
        Mode("dontAsk", "Never asks: everything that is not explicitly allowed is denied", "claude --permission-mode dontAsk"),
        Mode("bypassPermissions", "Skips every permission check; only for isolated environments",
            "claude --permission-mode bypassPermissions"),
    ];

    private static readonly ClaudeCommand[] Concepts =
    [
        Concept("CLAUDE.md", "The project's memory file, read at the start of every session: conventions, commands, architecture",
            "memory instructions rules"),
        Concept("~/.claude/CLAUDE.md", "Your personal memory file that applies to every project"),
        Concept(".claude/settings.json", "The project's shared settings (permissions, hooks, env), checked in to git"),
        Concept(".claude/settings.local.json", "Your personal project settings, not checked in"),
        Concept("~/.claude/settings.json", "User settings that apply to every project"),
        Concept(".claude/commands/name.md", "A custom slash command: the file deploy.md becomes /deploy"),
        Concept(".claude/skills/name/SKILL.md", "A skill: packaged instructions Claude loads when the task fits, or through /name"),
        Concept(".claude/agents/", "The folder for your own subagents, each with its own prompt, tools and model"),
        Concept("Hooks", "Scripts that run automatically on events like PreToolUse and PostToolUse, set up in settings.json",
            "automate event script"),
        Concept("MCP", "Model Context Protocol: connects Claude to external tools and data such as databases, GitHub and browsers",
            "integrations servers"),
        Concept("Subagent", "A separate Claude instance with its own context that does a subtask and reports back"),
        Concept("Context window", "Everything Claude can see right now; when it fills up, /compact can summarize to make room",
            "tokens memory"),
    ];

    private static ClaudeCommand Flag(string name, string description, string example) =>
        Cmd(ClaudeKind.Flag, name, description, null, null, example, "");

    private static ClaudeCommand Mode(string name, string description, string example) =>
        Cmd(ClaudeKind.Mode, name, description, null, null, example, "permissions");

    private static ClaudeCommand Concept(string name, string description, string keywords = "") =>
        Cmd(ClaudeKind.Concept, name, description, null, null, null, keywords);
}

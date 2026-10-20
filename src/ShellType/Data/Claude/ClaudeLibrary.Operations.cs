using ShellType.Models;

namespace ShellType.Data;

public static partial class ClaudeLibrary
{
    private const string Prompt = ClaudeCommand.ClaudePrompt;
    private const string Shell = CommandEntry.ShellPrompt;

    /// <summary>Everyday Claude Code workflows as sequences of commands, typed in order in operations mode.</summary>
    public static IReadOnlyList<Operation> Operations { get; } =
    [
        // ---- short: 2 to 4 steps ----
        Op("start a fresh task", "the last task is done. start the next one with a clean context.",
            (Prompt, "/clear", "Old context makes answers worse; start with an empty one"),
            (Prompt, "/plan add rate limiting to the login endpoint", "Let Claude read and plan before it touches anything")),

        Op("free up context", "the context is almost full but the task is not done.",
            (Prompt, "/context", "See how full the context window is and what fills it"),
            (Prompt, "/compact keep the api design decisions", "Summarize the conversation, keeping what matters"),
            (Prompt, "/context", "Check how much room you won back")),

        Op("pick up yesterday's work", "you closed the terminal last night. continue where you left off.",
            (Shell, "cd ~/code/shop", "Go to the project; conversations are saved per directory"),
            (Shell, "claude -c", "Continue the latest conversation in this directory"),
            (Prompt, "/recap", "Get a one-line reminder of where the session stands")),

        Op("undo a bad edit", "claude changed the wrong files. get the code back.",
            (Prompt, "/diff", "See exactly what Claude changed"),
            (Prompt, "/rewind", "Roll the code back to before the edit"),
            (Prompt, "/diff", "Confirm the unwanted changes are gone")),

        Op("summarize a log in a script", "a nightly job failed. let claude explain the log without opening a session.",
            (Shell, "cat error.log | claude -p \"what went wrong?\"", "Pipe the log in and print the answer"),
            (Shell, "claude -p \"list the failing tests\" --output-format json", "Ask for json so a script can read the answer")),

        Op("go fast on a small task", "the next change is tiny. use a quick model and less thinking.",
            (Prompt, "/model haiku", "A smaller model is quick and cheap for simple work"),
            (Prompt, "/effort low", "Simple tasks do not need deep thinking"),
            (Prompt, "/fast on", "Faster output with the same model")),

        Op("check what it costs", "you wonder how much this session has used.",
            (Prompt, "/usage", "See the session's cost and your limits"),
            (Prompt, "/context", "See how much of the context window is used")),

        // ---- medium: 5 to 7 steps ----
        Op("set up a new project", "claude is new to this repo. teach it the project and set sane defaults.",
            (Shell, "cd ~/code/new-app", "Start Claude from the project root"),
            (Shell, "claude", "Start an interactive session"),
            (Prompt, "/init", "Let Claude write a CLAUDE.md that describes the project"),
            (Prompt, "/memory", "Read CLAUDE.md through and add your own rules"),
            (Prompt, "/permissions", "Allow the safe commands you run all the time"),
            (Prompt, "/config model=sonnet", "Pick the default model for the project")),

        Op("review before you push", "the feature works. make sure it is good before it leaves your machine.",
            (Prompt, "/diff", "Read through everything that changed"),
            (Prompt, "/code-review high", "Hunt for bugs in the diff"),
            (Prompt, "/simplify", "Clean up what can be simpler"),
            (Prompt, "/security-review", "Look for security holes in the branch"),
            (Prompt, "! git status", "Run git yourself and let Claude see the result"),
            (Prompt, "! git push", "Push once the review is clean")),

        Op("work on two things at once", "a bug report arrived while you are building a feature. do both.",
            (Shell, "claude -w feature-auth", "Start the feature in its own git worktree"),
            (Prompt, "/rename feature-auth", "Name the session so it is easy to find again"),
            (Prompt, "/background keep building the login form", "Let the feature continue as a background agent"),
            (Shell, "claude -w fix-crash", "Open a second isolated worktree for the bug"),
            (Prompt, "/tasks", "Keep an eye on the background work"),
            (Shell, "claude agents", "See every background session from the terminal")),

        Op("connect github", "claude should read issues and pull requests on its own.",
            (Shell, "claude mcp add --transport http github https://api.githubcopilot.com/mcp/", "Register the GitHub MCP server"),
            (Shell, "claude", "Start a session so the server is loaded"),
            (Prompt, "/mcp", "Log in to the server and check that it is connected"),
            (Prompt, "/permissions", "Decide which GitHub tools may run without asking"),
            (Prompt, "list my open pull requests", "Try it out with a plain prompt")),

        Op("troubleshoot a broken setup", "claude behaves oddly after you changed some settings.",
            (Shell, "claude doctor", "Print diagnostics without starting a session"),
            (Shell, "claude --safe-mode", "Start without CLAUDE.md, skills, hooks and MCP to rule them out"),
            (Prompt, "/exit", "Leave the safe session once you know what is to blame"),
            (Shell, "claude --debug=hooks", "Start again with debug logging for hooks"),
            (Prompt, "/hooks", "Look over the hook configuration"),
            (Prompt, "/doctor", "Run the health check from inside the session")),

        // ---- long: 8 to 10 steps ----
        Op("ship a feature end to end", "build guest checkout, from plan to commit.",
            (Shell, "claude -n checkout-flow", "Start a named session you can resume later"),
            (Prompt, "/plan add a guest checkout flow", "Plan the change before anything is written"),
            (Prompt, "/model opus", "Use the strongest model for the build"),
            (Prompt, "/effort high", "Let Claude think hard about the design"),
            (Prompt, "implement the plan", "Approve the plan and let Claude build it"),
            (Prompt, "! npm test", "Run the tests yourself and share the result"),
            (Prompt, "/diff", "Review the changes"),
            (Prompt, "/code-review high --fix", "Find bugs and apply the fixes"),
            (Prompt, "! git commit -am \"add guest checkout\"", "Commit the work"),
            (Prompt, "/recap", "Get a one-line summary for the pull request")),

        Op("automate your workflow", "stop repeating yourself: formatting, permissions and checks should just happen.",
            (Prompt, "/update-config run prettier after every edit", "Let Claude add a PostToolUse hook to settings.json"),
            (Prompt, "/hooks", "Check that the hook is in place"),
            (Prompt, "/fewer-permission-prompts", "Allow the read-only commands you approve all the time"),
            (Prompt, "/permissions", "Look over the new allowlist"),
            (Prompt, "/skills", "See which skills are available"),
            (Prompt, "/reload-skills", "Load a skill you just added to .claude/skills/"),
            (Prompt, "/statusline", "Show model and context use at the bottom"),
            (Prompt, "/doctor", "Make sure the new config is valid")),

        Op("run claude in ci", "a pipeline should fix lint errors without a human at the keyboard.",
            (Shell, "claude setup-token", "Create a long-lived token for the pipeline"),
            (Shell, "claude auth status", "Check that the login works"),
            (Shell, "claude --version", "Log which version the job runs"),
            (Shell, "claude -p \"fix the lint errors\" --max-turns 5", "Run headless and cap the number of turns"),
            (Shell, "claude -p \"fix the lint errors\" --allowedTools \"Edit\"", "Allow file edits without prompts"),
            (Shell, "claude -p \"summarize the changes\" --output-format json", "Get a summary the job can parse"),
            (Shell, "claude -p \"review the diff\" --output-format stream-json", "Stream the review as it is written"),
            (Shell, "claude -p \"write release notes\" --append-system-prompt \"be brief\"", "Steer the tone of the output")),

        Op("keep working from anywhere", "you are leaving your desk but the work should go on.",
            (Prompt, "/rename api-migration", "Name the session so you recognize it later"),
            (Prompt, "/remote-control", "Make the session controllable from claude.ai"),
            (Prompt, "/mobile", "Get the app on your phone"),
            (Prompt, "/schedule every morning run the migration checks", "Let a cloud routine run the daily checks"),
            (Prompt, "/background finish the migration", "Detach the session and let it keep going"),
            (Shell, "claude agents", "Check on it later from any terminal"),
            (Shell, "claude attach 4f2a9c", "Attach to the session again"),
            (Prompt, "/teleport", "Or pull a cloud session into this terminal")),
    ];

    private static Operation Op(string name, string goal, params (string Prompt, string Text, string Why)[] steps) =>
        new(name, goal, steps.Select(s => new CommandEntry(s.Text, s.Why, CommandCategory.System, Difficulty.Medium, s.Prompt)).ToList());
}

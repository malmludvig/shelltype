namespace ShellType.Models;

/// <summary>
/// A single command line the player has to type, plus a short explanation
/// of what it does so the game teaches as well as trains.
/// The prompt is shown before it, e.g. <c>~$</c> in a shell or <c>R1(config)#</c> on a Cisco router.
/// </summary>
public sealed record CommandEntry(
    string Text,
    string Description,
    CommandCategory Category,
    Difficulty Difficulty,
    string Prompt = CommandEntry.ShellPrompt)
{
    public const string ShellPrompt = "~$";

    /// <summary>The program being invoked, e.g. <c>grep</c> for <c>grep -rn foo .</c>.</summary>
    public string Program
    {
        get
        {
            var text = Text.StartsWith("sudo ", StringComparison.Ordinal) ? Text[5..] : Text;
            var space = text.IndexOf(' ');
            return space < 0 ? text : text[..space];
        }
    }
}

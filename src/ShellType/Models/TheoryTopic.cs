namespace ShellType.Models;

/// <summary>
/// One chapter on the theory page: a Linux concept explained in about a page of text.
/// </summary>
/// <param name="Slug">URL name, e.g. <c>filesystem-hierarchy</c>.</param>
/// <param name="Group">Which part of the theory page it belongs to, e.g. <c>files</c>.</param>
/// <param name="Summary">One sentence shown in the topic list.</param>
/// <param name="Body">The text, in the small Markdown subset that <c>TheoryMarkdown</c> understands.</param>
public sealed record TheoryTopic(string Slug, string Title, string Group, string Summary, string Body)
{
    public int WordCount => Body.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;

    /// <summary>Reading time in whole minutes, at about 200 words a minute.</summary>
    public int Minutes => Math.Max(1, (int)Math.Round(WordCount / 200.0));
}

using System.Text;

namespace ShellType.Theory;

public enum InlineKind
{
    Text,
    Code,
    Bold,
}

/// <summary>A run of text inside a paragraph, heading or list item.</summary>
public sealed record Inline(InlineKind Kind, string Text);

public abstract record TheoryBlock;

public sealed record HeadingBlock(int Level, IReadOnlyList<Inline> Text) : TheoryBlock;

public sealed record ParagraphBlock(IReadOnlyList<Inline> Text) : TheoryBlock;

public sealed record CodeBlock(string Code) : TheoryBlock;

public sealed record ListBlock(IReadOnlyList<IReadOnlyList<Inline>> Items) : TheoryBlock;

/// <summary>
/// Parses the small Markdown subset the theory chapters are written in:
/// <c>## headings</c>, paragraphs, <c>- lists</c>, fenced code blocks,
/// <c>`inline code`</c> and <c>**bold**</c>. The result is rendered as plain
/// Razor elements, so no HTML from the text ever reaches the page.
/// </summary>
public static class TheoryMarkdown
{
    public static IReadOnlyList<TheoryBlock> Parse(string markdown)
    {
        var blocks = new List<TheoryBlock>();
        var lines = markdown.Replace("\r\n", "\n").Split('\n');
        var paragraph = new List<string>();
        var list = new List<IReadOnlyList<Inline>>();

        void FlushParagraph()
        {
            if (paragraph.Count > 0)
            {
                blocks.Add(new ParagraphBlock(ParseInline(string.Join(' ', paragraph))));
                paragraph.Clear();
            }
        }

        void FlushList()
        {
            if (list.Count > 0)
            {
                blocks.Add(new ListBlock(list.ToList()));
                list.Clear();
            }
        }

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].TrimEnd();
            var trimmed = line.TrimStart();

            if (trimmed.StartsWith("```", StringComparison.Ordinal))
            {
                FlushParagraph();
                FlushList();
                var code = new StringBuilder();
                for (i++; i < lines.Length && !lines[i].TrimStart().StartsWith("```", StringComparison.Ordinal); i++)
                {
                    code.Append(lines[i].TrimEnd()).Append('\n');
                }

                blocks.Add(new CodeBlock(code.ToString().TrimEnd('\n')));
            }
            else if (trimmed.Length == 0)
            {
                FlushParagraph();
                FlushList();
            }
            else if (trimmed.StartsWith('#'))
            {
                FlushParagraph();
                FlushList();
                var level = trimmed.TakeWhile(c => c == '#').Count();
                blocks.Add(new HeadingBlock(level, ParseInline(trimmed[level..].Trim())));
            }
            else if (trimmed.StartsWith("- ", StringComparison.Ordinal))
            {
                FlushParagraph();
                list.Add(ParseInline(trimmed[2..]));
            }
            else
            {
                FlushList();
                paragraph.Add(trimmed);
            }
        }

        FlushParagraph();
        FlushList();
        return blocks;
    }

    public static IReadOnlyList<Inline> ParseInline(string text)
    {
        var result = new List<Inline>();
        var plain = new StringBuilder();
        var i = 0;

        void FlushPlain()
        {
            if (plain.Length > 0)
            {
                result.Add(new Inline(InlineKind.Text, plain.ToString()));
                plain.Clear();
            }
        }

        while (i < text.Length)
        {
            if (text[i] == '`')
            {
                var end = text.IndexOf('`', i + 1);
                if (end > i)
                {
                    FlushPlain();
                    result.Add(new Inline(InlineKind.Code, text[(i + 1)..end]));
                    i = end + 1;
                    continue;
                }
            }
            else if (text[i] == '*' && i + 1 < text.Length && text[i + 1] == '*')
            {
                var end = text.IndexOf("**", i + 2, StringComparison.Ordinal);
                if (end > i)
                {
                    FlushPlain();
                    result.Add(new Inline(InlineKind.Bold, text[(i + 2)..end]));
                    i = end + 2;
                    continue;
                }
            }

            plain.Append(text[i]);
            i++;
        }

        FlushPlain();
        return result;
    }
}

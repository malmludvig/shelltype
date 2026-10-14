using ShellType.Theory;

namespace ShellType.Tests;

public class TheoryMarkdownTests
{
    [Fact]
    public void Parses_headings_paragraphs_lists_and_code()
    {
        var blocks = TheoryMarkdown.Parse("""
            ## Title

            First line
            continues here.

            - one
            - two

            ```
            ls -l
              indented
            ```
            """);

        Assert.Collection(blocks,
            b => Assert.Equal(2, Assert.IsType<HeadingBlock>(b).Level),
            b => Assert.Equal("First line continues here.", Assert.Single(Assert.IsType<ParagraphBlock>(b).Text).Text),
            b => Assert.Equal(2, Assert.IsType<ListBlock>(b).Items.Count),
            b => Assert.Equal("ls -l\n  indented", Assert.IsType<CodeBlock>(b).Code));
    }

    [Fact]
    public void Parses_numbered_lists()
    {
        var blocks = TheoryMarkdown.Parse("1. first\n2. second\n- bullet");

        Assert.Collection(blocks,
            b => Assert.Equal(2, Assert.IsType<ListBlock>(b) is { Ordered: true } list ? list.Items.Count : 0),
            b => Assert.False(Assert.IsType<ListBlock>(b).Ordered));
    }

    [Fact]
    public void Parses_inline_code_and_bold()
    {
        var inlines = TheoryMarkdown.ParseInline("run `ls -l` to see **permissions** here");

        Assert.Equal(
            [
                new Inline(InlineKind.Text, "run "),
                new Inline(InlineKind.Code, "ls -l"),
                new Inline(InlineKind.Text, " to see "),
                new Inline(InlineKind.Bold, "permissions"),
                new Inline(InlineKind.Text, " here"),
            ],
            inlines);
    }

    [Fact]
    public void Unclosed_markers_stay_as_text()
    {
        var inlines = TheoryMarkdown.ParseInline("a ` b ** c");

        Assert.Equal("a ` b ** c", Assert.Single(inlines).Text);
    }
}

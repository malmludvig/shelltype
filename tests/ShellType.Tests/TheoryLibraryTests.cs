using System.Text.RegularExpressions;
using ShellType.Data;
using ShellType.Theory;

namespace ShellType.Tests;

public class TheoryLibraryTests
{
    [Fact]
    public void Every_chapter_is_at_least_a_page_long()
    {
        Assert.All(TheoryLibrary.All, t => Assert.True(t.WordCount >= 500, $"{t.Slug} has only {t.WordCount} words"));
    }

    [Fact]
    public void Slugs_are_unique_and_url_friendly()
    {
        Assert.Equal(TheoryLibrary.All.Count, TheoryLibrary.All.Select(t => t.Slug).Distinct().Count());
        Assert.All(TheoryLibrary.All, t => Assert.Matches(new Regex("^[a-z0-9]+(-[a-z0-9]+)*$"), t.Slug));
    }

    [Fact]
    public void Every_chapter_belongs_to_a_known_group_and_has_headings()
    {
        Assert.All(TheoryLibrary.All, t =>
        {
            Assert.Contains(t.Group, TheoryLibrary.Groups);
            Assert.Contains(TheoryMarkdown.Parse(t.Body), b => b is HeadingBlock);
        });
    }
}

using System.Text.RegularExpressions;
using ShellType.Data;
using ShellType.Theory;

namespace ShellType.Tests;

public class TheoryLibraryTests
{
    [Fact]
    public void There_are_30_chapters_in_reading_order()
    {
        Assert.Equal(30, TheoryLibrary.All.Count);

        // The pager walks All in order, so it must match the order of the groups on the page.
        var groupOrder = TheoryLibrary.All.Select(t => Array.IndexOf(TheoryLibrary.Groups, t.Group)).ToList();
        Assert.Equal(groupOrder.Order(), groupOrder);
    }

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

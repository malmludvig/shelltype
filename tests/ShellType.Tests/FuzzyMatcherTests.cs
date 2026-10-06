using ShellType.Quiz;

namespace ShellType.Tests;

public class FuzzyMatcherTests
{
    [Fact]
    public void Empty_query_matches_everything()
    {
        Assert.NotNull(FuzzyMatcher.Match("", "anything"));
    }

    [Theory]
    [InlineData("chmod", "chmod +x deploy.sh")]
    [InlineData("cx", "chmod +x deploy.sh")]
    [InlineData("EXTR", "Extract a gzip-compressed tarball")]
    [InlineData("git p", "git push -u origin feature")]
    public void Matches_characters_in_order(string query, string text)
    {
        Assert.NotNull(FuzzyMatcher.Match(query, text));
    }

    [Theory]
    [InlineData("xc", "chmod +x")]
    [InlineData("zz", "tar -xzf")]
    [InlineData("sudo", "chmod +x deploy.sh")]
    public void Rejects_when_characters_are_missing_or_out_of_order(string query, string text)
    {
        Assert.Null(FuzzyMatcher.Match(query, text));
    }

    [Fact]
    public void Reports_matched_positions()
    {
        var match = FuzzyMatcher.Match("cd", "chmod");

        Assert.Equal([0, 4], match!.Positions);
    }

    [Fact]
    public void Consecutive_word_start_matches_rank_higher()
    {
        var tight = FuzzyMatcher.Match("ext", "Extract a tarball")!;
        var scattered = FuzzyMatcher.Match("ext", "Show the next line")!;

        Assert.True(tight.Score > scattered.Score);
    }

    [Fact]
    public void Finds_the_best_alignment_not_just_the_first()
    {
        // Greedy leftmost would use the "t" in "extract"; the better match is "tar" in "tarball".
        var match = FuzzyMatcher.Match("tar", "Extract a tarball")!;

        Assert.Equal([10, 11, 12], match.Positions);
    }
}

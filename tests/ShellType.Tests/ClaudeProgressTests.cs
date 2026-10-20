using ShellType.Data;
using ShellType.Game;
using ShellType.Models;
using ShellType.Services;

namespace ShellType.Tests;

public class ClaudeProgressTests
{
    private static readonly DateOnly Monday = new(2026, 10, 19);

    [Fact]
    public void Leitner_moves_up_one_box_and_back_to_one_on_a_miss()
    {
        var progress = new ClaudeProgress();
        Assert.Equal(1, progress.BoxOf("/clear"));

        for (var i = 0; i < 10; i++)
        {
            progress.Rate("/clear", knew: true);
        }

        Assert.Equal(ClaudeProgress.MaxBox, progress.BoxOf("/clear"));

        progress.Rate("/clear", knew: false);
        Assert.Equal(1, progress.BoxOf("/clear"));
    }

    [Fact]
    public void Day_streak_grows_on_consecutive_days_and_restarts_after_a_gap()
    {
        var progress = new ClaudeProgress();

        progress.MarkPracticed(Monday);
        progress.MarkPracticed(Monday);
        Assert.Equal(1, progress.CurrentStreak(Monday));

        progress.MarkPracticed(Monday.AddDays(1));
        Assert.Equal(2, progress.CurrentStreak(Monday.AddDays(1)));
        Assert.Equal(2, progress.CurrentStreak(Monday.AddDays(2))); // still alive the day after
        Assert.Equal(0, progress.CurrentStreak(Monday.AddDays(3)));

        progress.MarkPracticed(Monday.AddDays(4));
        Assert.Equal(1, progress.CurrentStreak(Monday.AddDays(4)));
    }

    [Fact]
    public void Missed_answers_are_kept_until_answered_right()
    {
        var progress = new ClaudeProgress();

        progress.RecordAnswer("/plan", correct: false);
        progress.RecordAnswer("/plan", correct: false);
        Assert.Equal(["/plan"], progress.Missed);

        progress.RecordAnswer("/plan", correct: true);
        Assert.Empty(progress.Missed);
    }

    [Fact]
    public void Known_can_be_toggled()
    {
        var progress = new ClaudeProgress();

        progress.SetKnown("Esc", true);
        progress.SetKnown("Esc", true);
        Assert.Equal(["Esc"], progress.Known);

        progress.SetKnown("Esc", false);
        Assert.False(progress.IsKnown("Esc"));
    }
}

public class FlashcardDeckTests
{
    [Fact]
    public void Cards_in_low_boxes_come_first()
    {
        var progress = new ClaudeProgress();
        var cards = ClaudeLibrary.All.Where(c => c.Kind == ClaudeKind.Mode).ToList();
        progress.Boxes["plan"] = 3;
        progress.Boxes["auto"] = 2;

        var deck = new FlashcardDeck(cards, progress, new Random(1));

        Assert.Equal("auto", deck.Cards[^2].Key);
        Assert.Equal("plan", deck.Cards[^1].Key);
    }

    [Fact]
    public void A_missed_card_comes_back_a_few_cards_later()
    {
        var progress = new ClaudeProgress();
        var deck = new FlashcardDeck(ClaudeLibrary.All.Take(10), progress, new Random(3));
        var first = deck.Current;

        deck.Flip();
        Assert.True(deck.Flipped);
        deck.Rate(progress, knew: false);

        Assert.False(deck.Flipped);
        Assert.Equal(1, deck.Index);
        Assert.Equal(11, deck.Cards.Count);
        Assert.Same(first, deck.Cards[1 + FlashcardDeck.RetryGap]);
        Assert.Equal(1, progress.BoxOf(first.Key));
    }

    [Fact]
    public void Knowing_a_card_moves_it_up_and_moves_on()
    {
        var progress = new ClaudeProgress();
        var deck = new FlashcardDeck(ClaudeLibrary.All.Take(3), progress, new Random(3));
        var first = deck.Current;

        deck.Rate(progress, knew: true);

        Assert.Equal(2, progress.BoxOf(first.Key));
        Assert.Equal(3, deck.Cards.Count);
        deck.Previous();
        Assert.Same(first, deck.Current);
        deck.Previous();
        Assert.Equal(2, deck.Index); // wraps around
    }
}

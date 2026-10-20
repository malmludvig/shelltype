using ShellType.Models;
using ShellType.Services;

namespace ShellType.Game;

/// <summary>
/// A shuffled deck of Claude Code flashcards, lowest Leitner box first, so the cards you know
/// least come up first and the ones you know well come up less often.
/// </summary>
public sealed class FlashcardDeck
{
    /// <summary>A card you did not know comes back this many cards later.</summary>
    public const int RetryGap = 4;

    private readonly List<ClaudeCommand> _cards;

    public FlashcardDeck(IEnumerable<ClaudeCommand> cards, ClaudeProgress progress, Random? random = null)
    {
        random ??= Random.Shared;
        _cards = cards.OrderBy(_ => random.Next()).OrderBy(c => progress.BoxOf(c.Key)).ToList();
    }

    public IReadOnlyList<ClaudeCommand> Cards => _cards;
    public int Index { get; private set; }
    public bool IsEmpty => _cards.Count == 0;
    public ClaudeCommand Current => _cards[Index];

    /// <summary>Showing the back: what the card does.</summary>
    public bool Flipped { get; private set; }

    public void Flip() => Flipped = !Flipped;

    public void Next() => MoveTo(Index + 1);

    public void Previous() => MoveTo(Index - 1);

    /// <summary>Moves the card between Leitner boxes and goes on to the next; a missed card is shown again soon.</summary>
    public void Rate(ClaudeProgress progress, bool knew)
    {
        if (IsEmpty)
        {
            return;
        }

        var card = Current;
        progress.Rate(card.Key, knew);
        if (!knew)
        {
            _cards.Insert(Math.Min(Index + 1 + RetryGap, _cards.Count), card);
        }

        Next();
    }

    private void MoveTo(int index)
    {
        if (IsEmpty)
        {
            return;
        }

        Index = (index % _cards.Count + _cards.Count) % _cards.Count;
        Flipped = false;
    }
}

namespace ShellType.Quiz;

/// <summary>Where a query matched inside a text, and how good the match is.</summary>
public sealed record FuzzyMatch(int Score, IReadOnlyList<int> Positions);

/// <summary>
/// fzf-style fuzzy matching: the query's characters must appear in order (case-insensitive),
/// not necessarily next to each other. Consecutive characters and matches at the start of
/// a word score higher, so "ext" ranks "Extract a tarball" above "Show the next line".
/// </summary>
public static class FuzzyMatcher
{
    private const int MatchScore = 10;
    private const int ConsecutiveBonus = 15;
    private const int WordStartBonus = 10;
    private const int MaxGapPenalty = 5;

    public static FuzzyMatch? Match(string query, string text)
    {
        if (query.Length == 0)
        {
            return new FuzzyMatch(0, []);
        }

        // Greedy matching from the leftmost occurrence can miss a better alignment,
        // so try every possible starting position of the first character and keep the best.
        FuzzyMatch? best = null;
        for (var start = 0; start < text.Length; start++)
        {
            if (!Same(text[start], query[0]))
            {
                continue;
            }

            var candidate = MatchFrom(query, text, start);
            if (candidate is null)
            {
                // If it fails from here it fails from every later start too.
                break;
            }

            if (best is null || candidate.Score > best.Score)
            {
                best = candidate;
            }
        }

        return best;
    }

    private static FuzzyMatch? MatchFrom(string query, string text, int start)
    {
        var positions = new List<int>(query.Length);
        var score = 0;
        var last = -2;
        var ti = start;

        foreach (var qc in query)
        {
            var found = -1;
            for (; ti < text.Length; ti++)
            {
                if (Same(text[ti], qc))
                {
                    found = ti++;
                    break;
                }
            }

            if (found < 0)
            {
                return null;
            }

            score += MatchScore;
            if (found == last + 1)
            {
                score += ConsecutiveBonus;
            }
            else if (last >= 0)
            {
                score -= Math.Min(found - last - 1, MaxGapPenalty);
            }

            if (found == 0 || !char.IsLetterOrDigit(text[found - 1]))
            {
                score += WordStartBonus;
            }

            positions.Add(found);
            last = found;
        }

        return new FuzzyMatch(score, positions);
    }

    private static bool Same(char a, char b) => char.ToLowerInvariant(a) == char.ToLowerInvariant(b);
}

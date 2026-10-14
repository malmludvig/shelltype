using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic Regex = new(
        "regex",
        "Regular expressions",
        "the shell",
        "The pattern language behind grep, sed and awk: anchors, character classes, repetition, groups and the basic versus extended flavors.",
        """
        ## Patterns for text

        A **regular expression** (regex) is a pattern that describes a set of strings. Where a glob like `*.log` matches file names, a regex matches text inside files and streams. `grep`, `sed`, `awk`, `less`, `vim`, `find -regex` and nearly every programming language use them. Learning the core of regex pays off for the rest of your career.

        ## The building blocks

        Most characters simply match themselves: the regex `error` matches the text "error" anywhere in a line. The power comes from a few special characters, the **metacharacters**:

        - `.` matches **any single character**. `c.t` matches cat, cut and c9t.
        - `[...]` a **character class**: one character from the set. `[aeiou]` a vowel, `[0-9]` a digit, `[A-Za-z]` a letter. `[^0-9]` means anything **except** a digit.
        - `^` anchors to the **start** of the line. `^root` matches lines that begin with root.
        - `$` anchors to the **end** of the line. `\.conf$` matches lines ending in .conf.
        - `\` escapes a metacharacter so it matches literally. `\.` is a real dot, `\$` a real dollar sign.

        ## Repetition

        These apply to the item just before them:

        - `*` zero or more times. `ab*c` matches ac, abc, abbbc.
        - `+` one or more times. `[0-9]+` is a number of any length.
        - `?` zero or one time, making something optional. `colou?r` matches color and colour.
        - `{n}`, `{n,}`, `{n,m}` exact counts. `[0-9]{3}` is exactly three digits; `[a-z]{2,8}` two to eight letters.

        A very common combination is `.*`: any number of any characters. `^Failed.*from` matches lines that start with "Failed" and later contain "from".

        ## Groups and alternatives

        - `(...)` groups items so a repetition applies to all of them: `(ab)+` matches ab, abab.
        - `|` means **or**: `error|warning|fatal` matches any of the three. `(GET|POST) /api` matches either method.

        Groups also **capture** what they matched. In `sed`, you can reuse captures in the replacement as `\1`, `\2`:

        ```
        echo "2026-10-14" | sed -E 's/([0-9]{4})-([0-9]{2})-([0-9]{2})/\3.\2.\1/'
        14.10.2026
        ```

        ## Basic and extended: the backslash trap

        Here is the part that confuses everyone. Traditional Unix tools speak **basic regular expressions (BRE)**, where `+`, `?`, `|`, `{}` and `()` are **plain characters** unless you put a backslash in front of them. **Extended regular expressions (ERE)** treat them as special without backslashes.

        - `grep` and `sed` use BRE by default. `grep -E` (or `egrep`) and `sed -E` switch to ERE.
        - `awk` always uses ERE.

        ```
        grep "error\|warning" app.log     # BRE: | needs a backslash
        grep -E "error|warning" app.log   # ERE: much easier to read
        ```

        When a pattern with `+` or `|` "does not work" in grep, a missing `-E` is almost always the reason. Just use `-E` by default.

        `grep -P` gives **Perl-compatible** regex (PCRE), which adds handy shortcuts like `\d` for a digit, `\s` for whitespace and `\w` for a word character. Without `-P`, `\d` does not work; use `[0-9]` or the POSIX classes `[[:digit:]]`, `[[:space:]]` and `[[:alpha:]]` instead, which work in every tool.

        ## Quote your patterns

        Regex metacharacters overlap with shell metacharacters: `*`, `?`, `[`, `$`, `|`, `(`. Always put patterns in **single quotes** so the shell passes them through untouched: `grep -E '^[0-9]+$' file`. Without quotes the shell may expand `*` into file names or treat `|` as a pipe.

        ## Practical examples

        ```
        grep -E '^[^#]' sshd_config                  # lines that are not comments
        grep -Ev '^[[:space:]]*(#|$)' nginx.conf     # skip comments and blank lines
        grep -Eo '[0-9]{1,3}(\.[0-9]{1,3}){3}' log   # extract IPv4 addresses
        grep -E ' (4|5)[0-9]{2} ' access.log         # HTTP 4xx and 5xx responses
        sed -E 's/[[:space:]]+$//' file.txt          # strip trailing whitespace
        awk '$7 ~ /^\/api\// {print $7}' access.log  # requests under /api/
        ```

        ## Habits that help

        - Start small and grow the pattern, testing it on real data with `grep --color` so you can see exactly what matched.
        - Anchor when you can. `^PermitRootLogin` is much more precise than `PermitRootLogin`, which also matches comments.
        - Remember that regex is **greedy**: `.*` grabs as much as it can, which matters in `sed` replacements.
        - Do not try to parse everything with regex. JSON wants `jq`, HTML wants a real parser, and email addresses are famously messier than any short pattern. For lines in logs and config files, though, regex is exactly the right tool.
        """);
}

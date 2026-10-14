using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic ExpansionAndQuoting = new(
        "expansion-and-quoting",
        "Expansion and quoting",
        "the shell",
        "How the shell rewrites your command line before running it, and how single quotes, double quotes and backslashes control that.",
        """
        ## The shell rewrites your command

        Before the shell runs anything it **expands** the command line: it replaces variables, patterns and embedded commands with their results. This is powerful, and it is also the cause of a huge share of shell bugs, especially around spaces in file names. Quoting is how you tell the shell what to expand and what to leave alone.

        ## The expansions, in order

        Bash performs these steps in a fixed order:

        1. **Brace expansion**: `file{1,2,3}.txt` becomes `file1.txt file2.txt file3.txt`, and `{1..5}` becomes `1 2 3 4 5`. Pure text generation; the files do not need to exist.
        2. **Tilde expansion**: `~` becomes your home directory, `~bob` Bob's.
        3. **Parameter expansion**: `$name` or `${name}` becomes the variable's value. There are useful variants: `${name:-default}` uses a default if unset, `${file%.txt}` strips a suffix, `${#name}` gives the length.
        4. **Command substitution**: `$(command)` is replaced by the command's output. `echo "Today is $(date +%A)"`. The old backtick form does the same but nests badly.
        5. **Arithmetic expansion**: `$((3 * 4 + 1))` becomes `13`.
        6. **Word splitting**: the results of unquoted expansions are split into separate words on spaces, tabs and newlines.
        7. **Pathname expansion (globbing)**: words containing `*`, `?` or `[...]` are replaced by matching file names.
        8. **Quote removal**: finally the quote characters themselves are removed.

        ## Why word splitting bites

        Steps 6 and 7 are where trouble hides. Imagine a file called `my report.txt`:

        ```
        file="my report.txt"
        rm $file          # runs: rm my report.txt  -> two arguments, two wrong files!
        rm "$file"        # runs: rm 'my report.txt' -> one argument, correct
        ```

        The unquoted `$file` was split at the space into two words. The rule that prevents nearly all of these bugs is simple: **always put double quotes around variable and command expansions**, `"$file"`, `"$(pwd)"`, unless you specifically want splitting. Tools like **shellcheck** will remind you.

        ## Three ways to quote

        **Single quotes** `'...'` turn off **everything**. What is inside is passed exactly as written: no variables, no commands, no globs, no backslash escapes. Use them for literal text full of special characters, like `awk` programs and regular expressions:

        ```
        echo 'Cost: $5 * 2'           # Cost: $5 * 2
        awk '{print $1}' access.log   # awk sees $1, the shell does not touch it
        ```

        You cannot put a single quote inside single quotes; close, add an escaped one, and reopen: `'It'\''s'`.

        **Double quotes** `"..."` prevent word splitting and globbing but **still allow** `$variable`, `$(command)` and `$((math))` expansions, plus backslash escapes for `$`, backticks, `"` and `\`. This is what you want around variables almost always:

        ```
        echo "Hello $USER, you are in $(pwd)"
        echo "Price: \$5"
        ```

        A **backslash** `\` escapes the single next character outside quotes: `rm my\ report.txt`, `echo \*`. At the end of a line it continues the command on the next line.

        ## Globbing details

        Globs match file names in the current directory (or the given path). Some behaviors worth knowing:

        - If nothing matches, bash passes the pattern **unchanged**: `ls *.xyz` gives "No such file or directory: *.xyz".
        - `*` does not match names starting with a dot, so hidden files are skipped.
        - Quoting a glob disables it: `find . -name "*.log"` must be quoted so that **find** receives the pattern, not the shell's expansion of it.

        That last point is a classic. Without quotes, if the current directory happens to contain `a.log`, the shell turns the command into `find . -name a.log` and you silently get the wrong result.

        ## Seeing what the shell does

        When in doubt, put `echo` in front of a command to see the expanded result without running it, or turn on tracing:

        ```
        echo rm *.tmp          # shows exactly which files would be removed
        set -x                 # print every command after expansion
        cp config{,.bak}
        + cp config config.bak
        set +x
        ```

        Quoting looks fussy at first, but it follows from one idea: the shell rewrites your line, and quotes tell it which parts are not its business.
        """);
}

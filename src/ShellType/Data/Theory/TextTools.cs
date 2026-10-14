using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic TextTools = new(
        "text-tools",
        "The Unix philosophy and text tools",
        "the shell",
        "Why Linux is full of small tools that work on lines of text, and how grep, cut, sort, uniq, sed and awk fit together.",
        """
        ## Small tools, joined by text

        In the 1970s the creators of Unix settled on a philosophy that still shapes Linux:

        - Write programs that **do one thing and do it well**.
        - Write programs that **work together**.
        - Use **text streams** as the universal interface, because every program can read and write text.

        So instead of one giant program with a menu for every task, you get dozens of small filters, each taking lines of text in and putting lines out. Joined with pipes, they solve problems their authors never imagined. Configuration files, logs, command output and CSV files are all text, so the same tools work on all of them.

        ## Looking at text

        - `cat` prints files; `less` pages through them (`/` to search, `q` to quit, `G` to jump to the end).
        - `head -n 20` and `tail -n 20` show the start and end; `tail -f` follows a growing file live.
        - `wc -l` counts lines, which answers surprisingly many questions ("how many errors today?").

        ## Finding lines: grep

        `grep` prints lines that match a pattern. It is probably the most used text tool of all.

        ```
        grep error app.log             # lines containing "error"
        grep -i error app.log          # ignore case
        grep -v DEBUG app.log          # lines NOT matching
        grep -c " 500 " access.log     # count matching lines
        grep -rn "TODO" src/           # search recursively, with line numbers
        grep -E "timeout|refused" app.log   # extended regex: either word
        grep -A 3 -B 1 Exception app.log    # show context after and before
        ```

        ## Cutting columns: cut

        `cut` extracts fields from each line, given a delimiter:

        ```
        cut -d: -f1 /etc/passwd        # user names (field 1, split on :)
        cut -d, -f2,4 data.csv         # columns 2 and 4 of a simple CSV
        ```

        ## Sorting and counting: sort and uniq

        `sort` orders lines (`-n` numerically, `-r` reversed, `-h` human sizes like 2K and 1G, `-k2` by the second field). `uniq` collapses **adjacent** duplicate lines, which is why it almost always follows `sort`. With `-c` it counts them:

        ```
        cut -d' ' -f1 access.log | sort | uniq -c | sort -rn | head
        ```

        That pipeline answers "which IP addresses made the most requests?" in one line. The `sort | uniq -c | sort -rn` pattern is worth memorizing; it is the shell's "group by and count".

        ## Changing characters: tr

        `tr` translates or deletes single characters: `tr 'a-z' 'A-Z'` uppercases, `tr -d '\r'` removes Windows line endings, `tr -s ' '` squeezes repeated spaces.

        ## Editing streams: sed

        `sed` is a **stream editor**. Its most common use by far is search and replace:

        ```
        sed 's/http:/https:/g' links.txt          # replace on every line, print the result
        sed -i 's/^#Port 22/Port 2222/' sshd_config   # edit the file in place
        sed -n '10,20p' big.log                   # print only lines 10 to 20
        sed '/^#/d' config.ini                    # delete comment lines
        ```

        `s/old/new/g` means substitute, with `g` for every match on the line rather than only the first. `-i` changes the file itself; make a backup first or use `-i.bak`.

        ## Working with columns: awk

        `awk` is a small programming language built around the idea that each line is a **record** split into **fields** `$1`, `$2` and so on, separated by whitespace by default (`-F` sets another separator). An awk program is a list of `pattern { action }` rules:

        ```
        awk '{print $1}' access.log                      # first field of every line
        awk '$9 == 500 {print $7}' access.log            # URLs that returned 500
        awk -F: '$3 >= 1000 {print $1}' /etc/passwd      # normal users
        awk '{sum += $5} END {print sum}' sizes.txt      # add up a column
        df -h | awk 'NR > 1 && $5+0 > 80 {print $6, $5}' # filesystems over 80% full
        ```

        `NR` is the line number, `NF` the number of fields and `END` runs after the last line. When `cut` is not flexible enough, awk usually is.

        ## Other useful filters

        - `tee` saves a copy while passing data on.
        - `xargs` turns lines into arguments for another command.
        - `diff` compares files line by line; `comm` compares sorted lists.
        - `column -t` lines up text into a readable table.
        - `jq` does for JSON what awk does for columns.

        ## Building a pipeline

        Build pipelines one step at a time: start with `cat file | head`, add one filter, look at the output, add the next. Each stage is simple, and you can check it in isolation. That incremental, exploratory style is the real power of the Unix philosophy: you are not writing a program, you are having a conversation with your data.
        """);
}

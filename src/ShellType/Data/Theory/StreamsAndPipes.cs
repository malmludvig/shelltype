using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic StreamsAndPipes = new(
        "streams-and-pipes",
        "Streams, redirection and pipes",
        "the shell",
        "stdin, stdout and stderr, sending them to files with > and 2>, and chaining programs together with |.",
        """
        ## Three standard streams

        Every process starts with three open channels, the **standard streams**, each with a file descriptor number:

        - **stdin (0)**, standard input: where the program reads input from. By default, your keyboard.
        - **stdout (1)**, standard output: where normal output goes. By default, the terminal.
        - **stderr (2)**, standard error: where error messages and diagnostics go. Also the terminal by default.

        Programs do not care where these streams lead. That is the key to the whole design: the shell can reconnect them to files, to other programs or to nothing, and the program works the same.

        Why have a separate stderr at all? So that errors stay visible even when the normal output is sent somewhere else, and so that a file or pipe gets clean data without error messages mixed in.

        ## Redirecting output

        ```
        ls > files.txt          # stdout to a file (overwrite)
        ls >> files.txt         # stdout to a file (append)
        ls /nope 2> errors.txt  # stderr to a file
        make > build.log 2>&1   # both stdout and stderr to the same file
        make &> build.log       # bash shorthand for the same
        command > /dev/null     # throw away output
        command 2> /dev/null    # throw away errors only
        ```

        The order in `> file 2>&1` matters. Read it left to right: first stdout goes to the file, then "send 2 to wherever 1 points now", which is the file. Writing `2>&1 > file` sends stderr to the terminal (where stdout pointed at that moment) and only stdout to the file.

        Be careful with `>`: it **truncates** the file before the command even runs. `sort data.txt > data.txt` leaves you with an empty file, because the shell emptied it before `sort` read it. Write to a new file, or use `sponge` or `sort -o`. Setting `set -o noclobber` makes the shell refuse to overwrite existing files with `>`.

        ## Redirecting input

        ```
        wc -l < access.log           # stdin from a file
        mysql mydb < dump.sql
        cat <<EOF > config.ini       # a here-document: several lines of stdin
        [server]
        port = 8080
        EOF
        grep error <<< "$logline"    # a here-string: one string as stdin
        ```

        ## Pipes

        The **pipe** `|` connects the stdout of one command to the stdin of the next. Both run at the same time; data flows through as it is produced.

        ```
        ps aux | grep nginx
        cat access.log | cut -d' ' -f1 | sort | uniq -c | sort -rn | head
        ```

        That second line is a small program built from five tools, each doing one thing: extract the first field (the IP address), sort them so duplicates are adjacent, count each unique one, sort by count from highest, and keep the top ten. This is the **Unix philosophy** in action: small tools that do one job well and work together through text streams.

        Pipes only carry **stdout**. Error messages still go to the terminal, which is usually what you want. To send stderr through the pipe too, use `2>&1 |` or bash's `|&`.

        ## tee: both ways at once

        `tee` copies its input to a file **and** passes it on, like a T-junction in a pipe. It is handy for watching output while saving it, and for writing to files that need root:

        ```
        ./build.sh | tee build.log
        echo "127.0.0.1 myapp.local" | sudo tee -a /etc/hosts
        ```

        The second example exists because `sudo echo ... >> /etc/hosts` fails: the redirection is done by **your** shell, which is not root, before sudo even starts. With `tee`, the program that opens the file is the one running as root.

        ## xargs: from lines to arguments

        Some commands do not read stdin but take arguments, like `rm`. `xargs` builds argument lists from its input:

        ```
        find . -name "*.tmp" | xargs rm
        find . -name "*.tmp" -print0 | xargs -0 rm   # safe with spaces in names
        grep -rl oldName src/ | xargs sed -i 's/oldName/newName/g'
        ```

        ## Exit status in pipelines

        A pipeline's exit status is that of the **last** command. `false | true` succeeds. In scripts, `set -o pipefail` makes the pipeline fail if any part fails, which catches errors that would otherwise be hidden in the middle.

        Once streams click, you start seeing every command as a filter that takes text in and puts text out, and building answers by snapping filters together becomes second nature.
        """);
}

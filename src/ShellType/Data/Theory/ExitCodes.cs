using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic ExitCodes = new(
        "exit-codes",
        "Exit codes and chaining commands",
        "the shell",
        "How every command reports success or failure, what the numbers mean, and how &&, || and ; build on that.",
        """
        ## Every command answers yes or no

        When a program finishes, it hands a small number back to its parent: the **exit status** or **exit code**. The convention is simple and universal on Unix systems:

        - **0** means **success**.
        - **anything from 1 to 255** means **failure** of some kind.

        That is the opposite of many programming languages, where 0 is false. In the shell, 0 is "true" because there is only one way to succeed but many ways to fail, and the number can say which.

        The shell stores the exit status of the last command in the special variable `$?`:

        ```
        ls /etc > /dev/null
        echo $?          # 0
        ls /nope
        echo $?          # 2
        grep root /etc/passwd > /dev/null; echo $?   # 0: found
        grep nobodyhere /etc/passwd; echo $?          # 1: not found
        ```

        Note that `$?` is overwritten by every command, including `echo`, so read it immediately or save it: `status=$?`.

        ## What the numbers mean

        Each program documents its own codes (see the "EXIT STATUS" section of its man page), but some patterns are common:

        - `1` a general error. For `grep`, specifically "no match", while `2` means a real error like a missing file.
        - `2` misuse: wrong arguments or options.
        - `126` the file was found but is not executable (permission problem).
        - `127` **command not found**.
        - `128 + N` the process was killed by signal N. `130` is 128 + 2, killed by Ctrl+C (SIGINT). `137` is 128 + 9, killed by SIGKILL, very often by the OOM killer. `143` is 128 + 15, SIGTERM.

        When a container exits with 137 or a CI step fails with 127, these numbers tell you immediately where to look.

        ## Chaining commands

        The shell gives you operators that use exit codes to decide what runs next:

        - `a ; b` runs `a`, then `b`, **no matter what**.
        - `a && b` runs `b` only if `a` **succeeded**.
        - `a || b` runs `b` only if `a` **failed**.

        ```
        sudo apt update && sudo apt upgrade -y     # only upgrade if the update worked
        mkdir -p build && cd build && cmake ..      # stop at the first failing step
        ping -c1 db.internal || echo "database host is down"
        cd /some/dir || exit 1                      # a common safety line in scripts
        ```

        `&&` is the one you will use most, because it stops a sequence at the first failure. The difference matters a lot: `cd /tmp/build; rm -rf *` is dangerous, because if the `cd` fails, `rm` runs in whatever directory you happened to be in. `cd /tmp/build && rm -rf *` is safe.

        Combining `&&` and `||` as an if-else, `test && echo yes || echo no`, mostly works but has a trap: if the middle command fails, the `||` part runs too. For real logic use `if`.

        ## if is about exit codes too

        The shell's `if` does not test true/false values like in other languages. It runs a command and checks its **exit code**:

        ```
        if grep -q "^alice:" /etc/passwd; then
            echo "alice exists"
        fi

        if ! systemctl is-active --quiet nginx; then
            echo "nginx is down"
        fi
        ```

        The `[ ... ]` you often see in `if` lines is actually a **command** too, called `test`. `[ -f file ]` exits 0 if the file exists. Bash's `[[ ... ]]` is a safer built-in version with fewer quoting pitfalls:

        ```
        [[ -d /var/log ]]          # is a directory
        [[ -z "$name" ]]           # string is empty
        [[ "$a" == "$b" ]]         # strings are equal
        [[ $count -gt 10 ]]        # number greater than
        ```

        ## Setting exit codes yourself

        Scripts end with the exit status of their last command, or whatever you pass to `exit`:

        ```
        if [[ ! -f config.yml ]]; then
            echo "config.yml is missing" >&2   # errors go to stderr
            exit 1
        fi
        ```

        Returning meaningful codes makes your scripts work properly with `&&`, `if`, cron, systemd (which marks a service as failed on a non-zero exit) and CI systems (which fail a build on a non-zero step).

        ## Stopping on errors

        By default a script keeps going after a command fails, which can turn one small error into a mess. Many scripts start with:

        ```
        set -euo pipefail
        ```

        `-e` exits at the first failing command, `-u` treats unset variables as errors, and `pipefail` makes a pipeline fail if any part of it fails. They are not perfect, but they catch a lot of mistakes early.

        Exit codes are the quiet glue of Linux: every tool speaks them, and almost every way of combining tools listens to them.
        """);
}

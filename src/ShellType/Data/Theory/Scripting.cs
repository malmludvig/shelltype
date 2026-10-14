using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic Scripting = new(
        "scripting",
        "Shell scripting basics",
        "the shell",
        "Turning commands into a reusable script: the shebang, arguments, variables, conditions, loops, functions and good habits.",
        """
        ## Why script

        Anything you type more than twice is a candidate for a script. A shell script is just a text file full of the same commands you would type by hand, plus some logic. It documents how a task is done and makes it repeatable, whether that is a backup, a deploy or setting up a new machine.

        ## The first lines

        ```
        #!/usr/bin/env bash
        set -euo pipefail

        echo "Backing up $HOME"
        ```

        The first line is the **shebang**. When you run the file as a program, the kernel reads `#!` and uses the rest of the line as the interpreter. `/usr/bin/env bash` finds bash wherever it is in `PATH`. Use `#!/bin/sh` only if you deliberately write portable POSIX shell without bash features.

        The second line makes the script stop on errors, as explained in the exit codes chapter. Then make it executable and run it:

        ```
        chmod +x backup.sh
        ./backup.sh
        ```

        ## Variables and arguments

        ```
        backup_dir="/backups/$(date +%F)"
        mkdir -p "$backup_dir"
        ```

        Arguments given to the script are in `$1`, `$2` and so on. `$0` is the script's name, `$#` the number of arguments and `"$@"` all of them, each correctly quoted. Give arguments readable names right away and provide defaults:

        ```
        source_dir="${1:?usage: backup.sh SOURCE [DEST]}"   # required, with a message
        dest="${2:-/backups}"                                # optional, with a default
        ```

        Quote every variable: `"$source_dir"`, not `$source_dir`. Unquoted variables break on spaces, as the quoting chapter shows.

        ## Conditions

        ```
        if [[ ! -d "$source_dir" ]]; then
            echo "error: $source_dir is not a directory" >&2
            exit 1
        elif [[ -z "$(ls -A "$source_dir")" ]]; then
            echo "nothing to back up"
            exit 0
        else
            echo "backing up $source_dir"
        fi
        ```

        Common tests: `-f` file exists, `-d` directory exists, `-x` is executable, `-z` string is empty, `-n` not empty, `==` and `!=` for strings, `-eq`, `-lt`, `-gt` for numbers. For several fixed choices, `case` is cleaner:

        ```
        case "$1" in
            start) start_server ;;
            stop)  stop_server ;;
            *)     echo "usage: $0 start|stop" >&2; exit 2 ;;
        esac
        ```

        ## Loops

        ```
        for file in /var/log/*.log; do
            echo "$file: $(wc -l < "$file") lines"
        done

        for i in {1..5}; do
            echo "attempt $i"
        done

        while read -r line; do
            echo "host: $line"
        done < hosts.txt

        until curl -sf http://localhost:8080/health; do
            sleep 2          # wait until the service answers
        done
        ```

        Loop over globs or `"$@"`, never over the output of `ls`, which breaks on unusual names. `while read -r` is the standard way to process a file line by line.

        ## Functions

        ```
        log() {
            echo "[$(date +%T)] $*" >&2
        }

        backup() {
            local src="$1" dest="$2"
            log "copying $src to $dest"
            rsync -a "$src/" "$dest/"
        }

        backup "$source_dir" "$dest"
        ```

        Functions take arguments just like scripts (`$1`, `$@`) and return an exit status. Use `local` for their variables so they do not leak into the rest of the script.

        ## Cleaning up

        `trap` runs code when the script exits, however it exits, which is perfect for temporary files:

        ```
        tmp="$(mktemp -d)"
        trap 'rm -rf "$tmp"' EXIT
        ```

        ## Debugging

        - `bash -n script.sh` checks the syntax without running it.
        - `bash -x script.sh` (or `set -x` inside) prints every command as it runs, after expansion.
        - **shellcheck** finds quoting bugs, typos and common mistakes before they bite. It is worth installing and running on every script.

        ## When to stop

        Shell scripts are excellent glue for running commands, moving files and checking conditions. Once a script needs complex data structures, a lot of string processing or careful error handling, a language like Python is usually the better tool. A good rule of thumb: if it grows past a page or two and you keep fighting the quoting, it is time to switch.
        """);
}

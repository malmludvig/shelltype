using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic Paths = new(
        "paths",
        "Paths and the working directory",
        "files",
        "Absolute and relative paths, ., .., ~ and how the shell knows where you are.",
        """
        ## What a path is

        A **path** is the address of a file in the directory tree. It is a list of directory names separated by slashes, ending in the name you want. `/home/alice/notes/todo.txt` means: start at the root `/`, go into `home`, then `alice`, then `notes`, and there find `todo.txt`.

        Linux paths are **case sensitive**: `Notes`, `notes` and `NOTES` are three different names. They may contain spaces and almost any character except `/` and the zero byte, but spaces and special characters must be quoted in the shell, which is why most people avoid them.

        ## Absolute and relative paths

        There are two kinds of paths, and telling them apart is simple: look at the first character.

        - An **absolute path** starts with `/`. It always means the same file, no matter where you are: `/etc/hosts`.
        - A **relative path** does not start with `/`. It is interpreted starting from your **current working directory**: `notes/todo.txt`.

        Every process has a current working directory. In the shell, `pwd` prints it and `cd` changes it. If your working directory is `/home/alice`, then `notes/todo.txt` and `/home/alice/notes/todo.txt` refer to the same file.

        ```
        pwd                    # /home/alice
        cd notes               # relative: now in /home/alice/notes
        cd /var/log            # absolute: now in /var/log
        ```

        ## The special names: . and ..

        Every directory contains two hidden entries:

        - `.` means **this directory**. `./deploy.sh` runs the script in the current directory, and `cp /etc/hosts .` copies a file to where you are.
        - `..` means **the parent directory**, one level up. `cd ..` goes up, and `../../config` goes up twice and then into `config`.

        Why do you have to type `./deploy.sh` instead of just `deploy.sh`? Because the shell only looks for commands in the directories listed in `PATH`, and the current directory is deliberately not one of them. Otherwise a malicious file called `ls` in a shared folder could run when you meant the real `ls`.

        ## Home and the tilde

        Your **home directory** is where your own files live, usually `/home/yourname`. The shell expands `~` to it, so `~/notes` is the same as `/home/alice/notes`. `~bob` means Bob's home directory. Running `cd` with no argument takes you home, and `cd -` jumps back to the previous directory you were in, which is very handy when switching between two places.

        ## Hidden files

        Names that start with a dot, like `.bashrc` or `.ssh`, are **hidden**: `ls` skips them unless you add `-a`. There is nothing secret about them; the convention just keeps configuration files out of the way. Your home directory is usually full of them.

        ## Globs: many paths at once

        The shell can expand patterns into lists of paths before a command runs. These are called **globs**:

        - `*` matches any number of characters: `*.log` is every name ending in `.log`.
        - `?` matches exactly one character: `file?.txt` matches `file1.txt` but not `file10.txt`.
        - `[abc]` matches one of the listed characters: `report[12].pdf`.
        - `{a,b}` (brace expansion) generates both words: `cp config{,.bak}` becomes `cp config config.bak`.

        The important thing is that the **shell** does this, not the command. `rm *.tmp` never sees the star; it receives the list of matching names. That is also why `echo *` prints the files in the current directory.

        ## Resolving paths

        Paths can contain symbolic links, `.` and `..`, so two different strings may point to the same file. `realpath` resolves a path into its single canonical absolute form, and `readlink -f` does the same for links:

        ```
        realpath ../notes/./todo.txt     # /home/alice/notes/todo.txt
        basename /var/log/syslog          # syslog
        dirname /var/log/syslog           # /var/log
        ```

        Getting comfortable with paths pays off everywhere: in `cd`, in `cp` and `mv`, in scripts, and in understanding error messages like "No such file or directory", which almost always means a typo or a relative path used from the wrong place.
        """);
}

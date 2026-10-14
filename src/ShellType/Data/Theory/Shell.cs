using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic Shell = new(
        "shell",
        "What the shell does",
        "the shell",
        "Terminal versus shell, how a command line is read and run, builtins versus programs, and the startup files.",
        """
        ## Terminal and shell are two things

        People say "terminal", "console", "command line" and "shell" as if they were the same, but there are two separate programs involved.

        The **terminal** (or terminal emulator, like GNOME Terminal, Windows Terminal or iTerm) is a window that shows text and sends your keystrokes onward. It knows nothing about commands.

        The **shell** is the program running **inside** the terminal that reads what you type, interprets it and runs things. The most common shell on Linux is **bash**; others are **zsh** (the default on macOS), **fish** and the minimal **sh** (often **dash**) used for system scripts. When you ssh into a server you get a shell there too, with the terminal on your own machine.

        ## What happens when you press Enter

        A command line looks simple, but the shell does a lot of work before any program starts. For `ls -l $HOME/*.txt > list.txt`, roughly:

        1. **Read** the line and split it into words, respecting quotes.
        2. **Expand**: replace `$HOME` with its value, `*.txt` with the matching file names, `~` with your home directory, `$(...)` with a command's output.
        3. **Set up redirections**: here, open `list.txt` for writing and connect it to the program's standard output.
        4. **Find the command**: is `ls` an alias, a shell function, a builtin, or a program somewhere in `PATH`?
        5. **Run** it: for a program, fork a child process, exec the program with the expanded arguments, and wait for it to finish.
        6. **Record** the exit status in `$?` and show the prompt again.

        The program only sees the final result. `ls` never sees `$HOME` or `*.txt`; it receives a list of actual paths. This is why many surprises in the shell are not the program's fault but the shell's expansion, which the next chapter explains.

        ## Finding commands

        When you type a name, the shell checks in this order:

        - **Aliases**: shortcuts you defined, like `alias ll='ls -lh'`.
        - **Functions**: small shell programs defined in your session or startup files.
        - **Builtins**: commands implemented inside the shell itself, like `cd`, `echo`, `export`, `source`, `alias`, `history`, `jobs` and `exit`.
        - **Programs** found by searching each directory listed in the `PATH` variable, left to right.

        `type` tells you which one a name is:

        ```
        type cd        # cd is a shell builtin
        type ll        # ll is aliased to `ls -lh'
        type grep      # grep is /usr/bin/grep
        which -a python3   # every python3 in PATH, in order
        ```

        Some commands **must** be builtins. `cd` changes the shell's own working directory, and a child process cannot change its parent's directory. The same goes for `export` and `source`, which change the shell's own variables.

        After the shell finds a program it remembers the location in a hash table. If you install a new version elsewhere in `PATH` and the old one still runs, `hash -r` clears that memory.

        ## Interactive comforts

        Shells offer a lot for the human at the keyboard:

        - **Tab completion** of commands, file names and often options.
        - **History**: Up and Down arrows, `history` to list, **Ctrl+R** to search backwards, `!!` for the previous command (`sudo !!` is a favorite) and `!$` for its last argument.
        - **Line editing**: Ctrl+A and Ctrl+E jump to the start and end, Ctrl+W deletes a word, Ctrl+U deletes to the start of the line, Ctrl+L clears the screen.
        - **Job control** with `&`, Ctrl+Z, `fg` and `bg`.

        ## Startup files

        When bash starts, it reads configuration files, and which ones depends on how it was started:

        - A **login shell** (ssh login, console login) reads `/etc/profile` and then the first of `~/.bash_profile`, `~/.bash_login` or `~/.profile`.
        - An **interactive non-login shell** (a new terminal window on a desktop) reads `~/.bashrc`.
        - A **script** reads neither, unless told to.

        Most distributions make `~/.profile` source `~/.bashrc`, so in practice **put aliases, functions and prompt settings in `~/.bashrc`** and environment variables meant for all programs in `~/.profile`. After editing, `source ~/.bashrc` reloads it without opening a new terminal.

        Understanding that the shell is just a program that turns your text into process launches is the foundation for everything else in this section: quoting, pipes, variables and scripts all build on it.
        """);
}

using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic Environment = new(
        "environment",
        "Environment variables and PATH",
        "the shell",
        "Shell variables versus environment variables, export, how PATH finds programs, and where to set things permanently.",
        """
        ## Variables in the shell

        The shell can store named values:

        ```
        name="Alice"          # no spaces around =
        echo "Hello $name"
        ```

        A variable set like this is a **shell variable**: it exists only inside the current shell. Programs you start from that shell do not see it. That surprises people the first time, so it is worth understanding why.

        ## The environment

        Every process has an **environment**: a list of `NAME=value` strings it received from its parent when it was started. When the shell runs a program, it passes along only the variables that are marked for **export**. Those are the **environment variables**.

        ```
        greeting="hi"
        bash -c 'echo $greeting'      # prints an empty line: not exported
        export greeting
        bash -c 'echo $greeting'      # hi
        export EDITOR=vim             # set and export in one step
        ```

        The environment is passed **down**, from parent to child, as a copy. A child can change its own copy, but it can never change its parent's. That is why running a script that sets variables does not affect your shell afterwards, and why you use `source script.sh` (or `. script.sh`) when you want a script to set variables in your current shell: `source` runs it inside the current shell instead of a child.

        You can also set a variable for a single command only, by putting it in front:

        ```
        LANG=C sort names.txt
        DEBUG=1 ./server
        ```

        ## Looking at the environment

        ```
        env                    # every environment variable
        printenv HOME          # one variable
        echo "$PATH"
        set | less             # all shell variables and functions too
        unset greeting         # remove a variable
        ```

        ## Common environment variables

        - `HOME` your home directory. `USER` your user name. `SHELL` your login shell.
        - `PATH` where to look for programs (see below).
        - `PWD` the current directory, `OLDPWD` the previous one.
        - `LANG` and the `LC_*` variables: language, character encoding, number and date formats.
        - `EDITOR` and `VISUAL`: the editor programs like git and crontab open.
        - `TERM` the terminal type, used by full-screen programs.
        - `TZ` the time zone, if set.

        Applications use environment variables for configuration all the time: `DATABASE_URL`, `NODE_ENV`, `JAVA_HOME`, `http_proxy`. Containers and cloud platforms rely on them heavily because they are an easy, language-independent way to pass settings in.

        ## PATH

        `PATH` is a list of directories separated by colons:

        ```
        echo "$PATH"
        /usr/local/bin:/usr/bin:/bin:/usr/local/sbin:/usr/sbin:/home/alice/.local/bin
        ```

        When you type a command name without a slash, the shell looks for an executable file with that name in each directory, **from left to right**, and runs the first one it finds. If none matches you get `command not found`. Order matters: putting `/usr/local/bin` first lets locally installed versions win over the system's.

        To add a directory, append (or prepend) it, keeping the old value:

        ```
        export PATH="$PATH:/opt/node/bin"
        export PATH="$HOME/.local/bin:$PATH"
        ```

        Never write `export PATH=/opt/node/bin` alone; that replaces the whole list, and suddenly even `ls` is "not found" in that shell.

        The current directory is deliberately **not** in PATH. To run a program from where you are, write `./program`. `which program` and `type program` show which file a name resolves to, which is the first thing to check when the "wrong version" of something runs.

        ## Making it permanent

        Variables set in a terminal disappear when you close it. To keep them, add the lines to a startup file:

        - `~/.bashrc` for things used in interactive shells: aliases, prompt, functions.
        - `~/.profile` (or `~/.bash_profile`) for environment variables of your login session.
        - `/etc/environment` or files in `/etc/profile.d/` for all users.

        Then `source ~/.bashrc` or log in again.

        ## Services do not read your startup files

        A very common trap: something works in your terminal but fails under cron or as a systemd service. Those start with a minimal environment and never read `~/.bashrc`, so your PATH additions and exported variables are missing. Set what they need explicitly: `Environment=` or `EnvironmentFile=` in the systemd unit, variables at the top of the crontab, or full paths to programs. You can check what a running process really received in `/proc/PID/environ`.
        """);
}

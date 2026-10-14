using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic SignalsAndJobs = new(
        "signals-and-jobs",
        "Signals and job control",
        "processes",
        "What kill really sends, SIGTERM versus SIGKILL, Ctrl+C and Ctrl+Z, and running jobs in the background.",
        """
        ## Signals: tapping a process on the shoulder

        A **signal** is a small notification the kernel delivers to a process. It carries no data except its number, and it means something like "please stop", "you lost your terminal" or "reload your config". Signals interrupt whatever the process is doing; the process then either runs a **handler** it set up for that signal, ignores it, or gets the default action, which for most signals is to terminate.

        The command for sending signals is, confusingly, called `kill`. It sends **SIGTERM** by default, but it can send any signal:

        ```
        kill 4821            # SIGTERM: please exit
        kill -9 4821         # SIGKILL: die now
        kill -HUP 4821       # SIGHUP: often means reload
        kill -l              # list every signal
        ```

        ## The signals you need

        - **SIGTERM (15)**: "please terminate". The polite default. A well-written program catches it, finishes what it is doing, saves state, closes files and exits cleanly.
        - **SIGKILL (9)**: "terminate now". It cannot be caught, blocked or ignored; the kernel just removes the process. Nothing gets cleaned up: temporary files stay, data may be half written, child processes may be orphaned. Use it only when SIGTERM did not work.
        - **SIGINT (2)**: sent when you press **Ctrl+C** in a terminal. Usually ends the program.
        - **SIGHUP (1)**: originally "the terminal hung up". Programs started in a terminal get it when you close the terminal or log out. Many daemons use it as a signal to **reload their configuration** without restarting.
        - **SIGSTOP (19)** and **SIGTSTP (20)**: pause a process. SIGTSTP is what **Ctrl+Z** sends. SIGSTOP, like SIGKILL, cannot be ignored.
        - **SIGCONT (18)**: continue a paused process.
        - **SIGCHLD (17)**: sent to a parent when a child exits, so it can collect the exit status.
        - **SIGSEGV (11)**: the process accessed memory it should not have, a "segmentation fault". Usually a bug.

        ## The right way to stop something

        Try in order, and wait a few seconds between steps:

        1. `kill PID` (SIGTERM) and give it time to shut down.
        2. Check with `ps -p PID` whether it is gone.
        3. Only then `kill -9 PID`.

        If even SIGKILL does not work, the process is probably in state **D**, stuck waiting for disk or network storage. No signal can get through until that I/O returns; the fix is to solve the storage problem.

        `pkill` and `killall` send signals by **name** instead of PID: `pkill -f backup.sh` matches the whole command line. Be careful, they hit every matching process.

        ## Job control in the shell

        The shell lets you run several programs from one terminal and juggle them. Each pipeline you start is a **job**.

        - Add `&` to run a command in the **background**: `./long-task.sh &`. You get your prompt back immediately.
        - Press **Ctrl+Z** to **stop** the foreground job.
        - `jobs` lists your jobs with numbers like `[1]`.
        - `fg %1` brings job 1 back to the **foreground**; `bg %1` lets a stopped job continue in the background.

        ```
        $ make build          # oops, takes long
        ^Z
        [1]+  Stopped         make build
        $ bg %1               # keep building in the background
        $ jobs
        [1]+  Running         make build &
        ```

        ## Surviving logout

        Background jobs are still children of your shell. When you log out or your ssh connection drops, the shell sends them SIGHUP and they usually die. There are several ways to keep work running:

        - `nohup ./task.sh &` ignores SIGHUP and writes output to `nohup.out`.
        - `disown %1` removes a running job from the shell's job table so it is not sent SIGHUP.
        - `tmux` or `screen` give you a terminal session that lives on the server; you can detach, log out, and attach again later to find everything still running. This is the most comfortable option for long interactive work.
        - For anything that should run permanently, make it a **systemd service** instead.

        ## Signals in your own scripts

        Shell scripts can catch signals with `trap`, which is great for cleaning up:

        ```
        tmp=$(mktemp)
        trap 'rm -f "$tmp"' EXIT     # always delete the temp file
        trap 'echo interrupted; exit 1' INT TERM
        ```

        Understanding signals turns "the program will not die" from a mystery into a short checklist: which signal did you send, does the program handle it, and is it stuck in the kernel?
        """);
}

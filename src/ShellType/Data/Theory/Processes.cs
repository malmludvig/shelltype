using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic Processes = new(
        "processes",
        "How processes work",
        "processes",
        "PIDs, parents and children, fork and exec, process states, and how to read ps and top.",
        """
        ## Programs and processes

        A **program** is a file on disk, like `/usr/bin/python3`. A **process** is a program that is **running**: the code loaded into memory plus everything the kernel tracks about it, such as its memory, open files, current directory, user, environment variables and CPU time. Start Python three times and you get one program but three processes.

        Every process has a number, the **process ID (PID)**. PIDs are handed out in increasing order and reused after they wrap around. You use them to inspect and control processes: `kill 4821`, `ps -p 4821`, `ls /proc/4821`.

        ## Every process has a parent

        Processes do not appear out of nowhere. Each one is created by another process, its **parent**, and remembers the parent's PID as its **PPID**. When you type `ls` in bash, bash is the parent and `ls` is the child. Follow the parents upward and you always end at **PID 1**, the first process the kernel starts at boot. On most systems today that is **systemd**. `pstree -p` draws the whole family tree.

        ## fork and exec

        Linux creates processes in two steps, and knowing them explains a lot of shell behavior:

        - **fork** makes an almost exact copy of the current process. The copy, the child, gets a new PID but inherits the parent's memory, open files, environment and working directory.
        - **exec** replaces the code running in a process with a new program, keeping the same PID and open files.

        When bash runs `ls`, it forks a copy of itself, and the copy execs `/usr/bin/ls`. That is why a child inherits the parent's environment variables and open file descriptors, and why `cd` has to be built into the shell: a child process changing its own working directory cannot change the parent's.

        ## Process states

        A process is not always running. `ps` and `top` show a state letter:

        - **R** running, or ready and waiting for a CPU.
        - **S** sleeping, waiting for something like input, a timer or a network packet. Most processes spend most of their time here.
        - **D** uninterruptible sleep, usually waiting for disk or network storage. Lots of processes in D often mean slow or hanging I/O, and they cannot be killed until the I/O returns.
        - **T** stopped, for example after you pressed Ctrl+Z.
        - **Z** zombie: the process has finished, but its parent has not yet collected its exit status.

        Zombies use no CPU or memory, only an entry in the process table. They disappear when the parent calls `wait`. If the parent is buggy and never does, killing or restarting the **parent** clears them; when a parent dies, its children are adopted by PID 1, which cleans them up.

        ## Looking at processes

        ```
        ps aux                         # every process, BSD style
        ps -ef                         # every process, with PPID
        ps -eo pid,ppid,user,%cpu,%mem,stat,cmd --sort=-%cpu | head
        pgrep -a nginx                 # PIDs and command lines matching a name
        top                            # live view; press P for cpu, M for memory
        ```

        In `ps aux`, **VSZ** is virtual memory the process has reserved and **RSS** is the physical memory it actually uses right now. RSS is usually the number you care about.

        ## Load average

        `uptime` and `top` show three numbers like `load average: 0.42, 0.55, 0.61`. They are the average number of processes that were running or waiting to run (including state D) over the last 1, 5 and 15 minutes. Compare them with the number of CPU cores (`nproc`): a load of 4 on a 4-core machine means it is fully busy; a load of 12 means work is queuing up.

        ## Priorities

        The scheduler decides which ready process gets a CPU next. You can influence it with the **nice** value, from -20 (greedy) to 19 (polite). `nice -n 10 ./backup.sh` starts a job politely, and `renice` changes a running one. Only root may raise priority. For disk-heavy work there is `ionice`.

        ## Daemons and services

        Many processes run in the background with no terminal, waiting for work: web servers, databases, the ssh server. They are called **daemons**, and their names often end in `d`: `sshd`, `crond`, `systemd-journald`. On modern systems systemd starts and supervises them as **services**, which the systemd chapter covers.

        Everything in Linux that "does something" is a process. When a machine is slow, when a port is busy, when a file cannot be deleted, the question is almost always: **which process, running as which user, is doing what?** `ps`, `top`, `pgrep`, `lsof` and `/proc` are how you find out.
        """);
}

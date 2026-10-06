using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

public static partial class CommandLibrary
{
    private static readonly CommandEntry[] Processes = Build(CommandCategory.Processes,
        ("ps aux", "List every running process", Easy),
        ("ps -ef | grep nginx", "Find nginx processes", Medium),
        ("top", "Interactive view of running processes", Easy),
        ("htop", "Friendlier interactive process viewer", Easy),
        ("kill 1234", "Ask process 1234 to terminate", Easy),
        ("kill -9 1234", "Forcefully kill process 1234", Easy),
        ("pkill -f gunicorn", "Kill processes whose command line matches gunicorn", Medium),
        ("killall node", "Kill every process named node", Easy),
        ("pgrep -a python", "List python processes with their command lines", Medium),
        ("jobs", "List background jobs in this shell", Easy),
        ("fg %1", "Bring job 1 to the foreground", Easy),
        ("bg", "Resume a stopped job in the background", Easy),
        ("nohup ./server.sh &", "Run a script that survives logout, in the background", Medium),
        ("nice -n 10 make", "Run make with lower CPU priority", Medium),
        ("renice +5 -p 4321", "Lower the priority of a running process", Medium),
        ("lsof -i :8080", "See which process is listening on port 8080", Medium),
        ("watch -n 2 'ps aux | head'", "Re-run a command every two seconds", Hard),
        ("time ./build.sh", "Measure how long a command takes", Easy),
        ("strace -p 1234", "Trace system calls made by a process", Medium),
        ("ps -eo pid,comm,%mem --sort=-%mem | head", "Show the processes using the most memory", Hard));
}

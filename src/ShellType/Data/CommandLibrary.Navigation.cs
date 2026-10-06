using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

public static partial class CommandLibrary
{
    private static readonly CommandEntry[] Navigation = Build(CommandCategory.Navigation,
        ("pwd", "Print the current working directory", Easy),
        ("cd ~", "Go to your home directory", Easy),
        ("cd ..", "Move up one directory", Easy),
        ("cd -", "Jump back to the previous directory", Easy),
        ("cd /var/log", "Change into the system log directory", Easy),
        ("ls", "List files in the current directory", Easy),
        ("ls -la", "List all files, including hidden ones, in long format", Easy),
        ("ls -lh", "Long listing with human-readable file sizes", Easy),
        ("ls -lt", "Long listing sorted by modification time, newest first", Easy),
        ("ls -R src", "List the src directory recursively", Medium),
        ("ls -1 | wc -l", "Count the entries in the current directory", Medium),
        ("tree -L 2", "Show the directory tree two levels deep", Medium),
        ("pushd /etc", "Push /etc onto the directory stack and cd into it", Medium),
        ("popd", "Return to the directory on top of the stack", Easy),
        ("dirs -v", "Show the directory stack with indexes", Medium),
        ("cd \"$(git rev-parse --show-toplevel)\"", "Jump to the root of the current git repository", Hard),
        ("ls -d */", "List only directories", Medium),
        ("ls -lS --color=auto", "Long listing sorted by size, with colors", Hard));
}

using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

public static partial class CommandLibrary
{
    private static readonly CommandEntry[] Search = Build(CommandCategory.Search,
        ("grep error app.log", "Find lines that contain error", Easy),
        ("grep -i warning app.log", "Case-insensitive search", Easy),
        ("grep -rn TODO src/", "Recursively search with line numbers", Medium),
        ("grep -v '^#' config.ini", "Show lines that are not comments", Medium),
        ("grep -E 'fail|error' syslog", "Search with an extended regular expression", Medium),
        ("grep -c 404 access.log", "Count matching lines", Easy),
        ("find . -name '*.cs'", "Find all C# files below the current directory", Medium),
        ("find /tmp -type f -mtime +7 -delete", "Delete files in /tmp older than a week", Hard),
        ("find . -type d -name node_modules -prune", "Find node_modules directories without descending into them", Hard),
        ("find . -size +100M", "Find files larger than 100 MB", Medium),
        ("find . -name '*.log' -exec gzip {} \\;", "Compress every log file", Hard),
        ("locate nginx.conf", "Look up a file in the locate database", Easy),
        ("which python3", "Show which executable runs for a command", Easy),
        ("whereis ls", "Find the binary, source and man page for a command", Easy),
        ("type cd", "Show how the shell interprets a name", Easy),
        ("rg -i 'connection refused'", "Fast recursive search with ripgrep", Medium),
        ("find . -name '*.txt' | xargs wc -l", "Count lines across all text files", Hard),
        ("history | grep ssh", "Search your shell history", Medium));
}

using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

public static partial class CommandLibrary
{
    private static readonly CommandEntry[] Files = Build(CommandCategory.Files,
        ("touch notes.txt", "Create an empty file or update its timestamp", Easy),
        ("mkdir projects", "Create a directory", Easy),
        ("mkdir -p src/app/components", "Create nested directories in one go", Medium),
        ("cp file.txt backup.txt", "Copy a file", Easy),
        ("cp -r src/ dist/", "Copy a directory recursively", Medium),
        ("cp -a /etc/nginx ~/nginx.bak", "Archive-copy a directory, preserving attributes", Hard),
        ("mv old.txt new.txt", "Rename a file", Easy),
        ("mv *.log logs/", "Move every .log file into the logs directory", Medium),
        ("rm temp.txt", "Delete a file", Easy),
        ("rm -rf build/", "Forcefully delete a directory and its contents", Medium),
        ("rmdir empty_dir", "Remove an empty directory", Easy),
        ("ln -s /opt/app/bin/app /usr/local/bin/app", "Create a symbolic link", Hard),
        ("cat README.md", "Print a file to the terminal", Easy),
        ("less /var/log/syslog", "Page through a large file", Easy),
        ("head -n 20 data.csv", "Show the first 20 lines of a file", Medium),
        ("tail -f /var/log/nginx/access.log", "Follow a log file as it grows", Medium),
        ("file image.png", "Detect what kind of file something is", Easy),
        ("stat config.yaml", "Show detailed metadata about a file", Easy),
        ("du -sh *", "Show the size of each item in the current directory", Medium),
        ("df -h", "Show free disk space on mounted filesystems", Easy),
        ("rsync -avz src/ user@host:/srv/app/", "Sync a directory to a remote host", Hard),
        ("touch {a,b,c}.txt", "Create three files using brace expansion", Medium),
        ("diff -u old.conf new.conf", "Show a unified diff between two files", Medium));
}

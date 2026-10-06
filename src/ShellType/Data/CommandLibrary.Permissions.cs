using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

public static partial class CommandLibrary
{
    private static readonly CommandEntry[] Permissions = Build(CommandCategory.Permissions,
        ("chmod +x deploy.sh", "Make a script executable", Easy),
        ("chmod 644 index.html", "Owner can read and write, everyone else can read", Easy),
        ("chmod 755 /usr/local/bin/tool", "Owner can do everything, others can read and execute", Medium),
        ("chmod -R u+rwX,go-w shared/", "Recursively tighten permissions on a directory", Hard),
        ("chown www-data:www-data /var/www", "Change the owner and group of a directory", Medium),
        ("sudo chown -R $USER:$USER ~/.npm", "Take ownership of your npm cache", Hard),
        ("chgrp developers project/", "Change the group of a directory", Easy),
        ("whoami", "Print your username", Easy),
        ("id", "Show your user and group IDs", Easy),
        ("groups", "List the groups you belong to", Easy),
        ("sudo -i", "Start a root login shell", Easy),
        ("su - postgres", "Switch to the postgres user", Easy),
        ("sudo useradd -m -s /bin/bash alice", "Create a user with a home directory and bash shell", Hard),
        ("sudo usermod -aG docker $USER", "Add yourself to the docker group", Medium),
        ("sudo passwd alice", "Set a password for alice", Easy),
        ("umask 022", "Set the default permission mask for new files", Easy),
        ("ls -l /etc/shadow", "Inspect the permissions of the shadow password file", Easy),
        ("sudo visudo", "Safely edit the sudoers file", Easy));
}

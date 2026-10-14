using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic UsersAndGroups = new(
        "users-and-groups",
        "Users, groups, root and sudo",
        "users & permissions",
        "Who you are to the system: uids, groups, /etc/passwd, the root account and how sudo lends you its power.",
        """
        ## Linux is multi-user

        Linux was built for many people sharing one machine, and that design is still everywhere, even on a laptop with a single person. Every process runs **as some user**, and every file is **owned by some user**. The system uses that to decide who may read, change or run what.

        To the kernel, a user is not a name but a number: the **user ID (uid)**. The name `alice` is just a friendly label that maps to, say, uid 1000. Likewise every **group** has a **group ID (gid)**. The `id` command shows both:

        ```
        id
        uid=1000(alice) gid=1000(alice) groups=1000(alice),27(sudo),998(docker)
        ```

        ## Where users are stored

        Local accounts live in a few plain text files in `/etc`:

        - `/etc/passwd`: one line per user with name, uid, primary gid, a description, the home directory and the login shell. Despite its name it no longer contains passwords.
        - `/etc/shadow`: the password **hashes** and expiry rules. Only root can read it.
        - `/etc/group`: the groups and which users are members.

        ```
        alice:x:1000:1000:Alice:/home/alice:/bin/bash
        www-data:x:33:33:www-data:/var/www:/usr/sbin/nologin
        ```

        The second example shows a **system user**. Services like web servers and databases run as their own unprivileged users (`www-data`, `postgres`, `nobody`) with no password and a shell like `/usr/sbin/nologin`, so nobody can log in as them. If such a service is hacked, the attacker only gets that user's limited rights, not the whole machine.

        Use `getent passwd alice` rather than grepping the file, because on company networks users may also come from LDAP or Active Directory, and `getent` asks all configured sources.

        ## Groups

        Groups let you give a set of users the same access. Every user has one **primary group** (often with the same name as the user) and any number of **supplementary groups**. Membership in special groups grants abilities: `sudo` or `wheel` may use sudo, `docker` may talk to the Docker daemon, `adm` may read many logs.

        ```
        sudo usermod -aG docker alice    # add alice to docker; -a means append, do not forget it
        groups alice
        ```

        Group changes apply to **new** logins. An existing session keeps its old groups until you log out and in again, or start a shell with `newgrp docker`. Forgetting `-a` in `usermod -G` replaces all of a user's groups, a classic way to lose sudo rights.

        ## Root

        The user with uid **0** is **root**, the superuser. The kernel skips almost all permission checks for root: it can read any file, kill any process, change any setting and bind to privileged ports below 1024. That power is why you should not work as root day to day. One typo in `rm -rf` as root can destroy a system; the same typo as a normal user mostly hurts only your own files.

        ## sudo

        `sudo` lets permitted users run a single command as root (or as another user) and then drops back to normal rights:

        ```
        sudo apt update                 # one command as root
        sudo -u postgres psql           # run as the postgres user
        sudo -i                         # an interactive root shell, use sparingly
        ```

        sudo asks for **your own** password, not root's, and remembers it for a few minutes. Who may do what is configured in `/etc/sudoers` and files in `/etc/sudoers.d/`. Always edit them with `visudo`, which checks the syntax before saving; a broken sudoers file can lock everyone out of sudo. Every sudo call is logged, which gives a trail of who did what.

        The older `su` switches to another user entirely and asks for **that** user's password. `su -` becomes root with root's own environment. Many systems disable root's password and rely on sudo only.

        ## Managing accounts

        ```
        sudo adduser bob                 # friendly: creates home, asks for a password
        sudo useradd -m -s /bin/bash bob # low level, used in scripts
        sudo passwd bob                  # set or change a password
        sudo userdel -r bob              # delete the user and the home directory
        who                              # who is logged in now
        last                             # login history
        ```

        The principle behind all of it is **least privilege**: every user and every service gets exactly the rights it needs and no more. Linux's user and group model is the basic tool for that.
        """);
}

using ShellType.Models;

namespace ShellType.Data;

public static partial class TheoryLibrary
{
    private static readonly TheoryTopic Packages = new(
        "packages",
        "Package management",
        "system",
        "How software is installed on Linux: packages, repositories, dependencies, apt and dnf, and why you rarely download installers.",
        """
        ## Software comes in packages

        On Windows and macOS you often download an installer from a website. On Linux the normal way to install software is through a **package manager**. A **package** is an archive containing a program's files, plus metadata: its name and version, a description, which other packages it **depends** on, and scripts to run during installation. Your distribution builds and tests thousands of packages and publishes them in **repositories**, online collections your system downloads from.

        This model has big advantages. One command installs software along with everything it needs. One command updates **everything** on the system, programs and libraries alike, including security fixes. Every file is tracked, so removal is clean. And packages are cryptographically **signed**, so your system can verify they really came from the distribution.

        ## Two big families

        Most distributions belong to one of two families with different package formats:

        - **Debian family** (Debian, Ubuntu, Mint, Raspberry Pi OS): `.deb` packages. The low-level tool is `dpkg`; the everyday tool is `apt`.
        - **Red Hat family** (Fedora, RHEL, Rocky, AlmaLinux, CentOS): `.rpm` packages. The low-level tool is `rpm`; the everyday tool is `dnf` (formerly `yum`).

        Others have their own: Arch uses `pacman`, openSUSE `zypper`, Alpine `apk`. The ideas are the same everywhere.

        ## apt in practice

        ```
        sudo apt update              # refresh the list of available packages
        sudo apt upgrade             # install newer versions of everything
        sudo apt install nginx       # install a package and its dependencies
        sudo apt remove nginx        # remove it (keeps config files)
        sudo apt purge nginx         # remove it including config files
        sudo apt autoremove          # remove dependencies nothing needs anymore
        apt search postgres          # search package names and descriptions
        apt show nginx               # details about a package
        apt list --installed         # what is installed
        ```

        A common beginner confusion: `apt update` does **not** update any software. It only downloads the latest **list** of what is available. `apt upgrade` is what installs new versions. You normally run them together.

        ## dnf in practice

        ```
        sudo dnf check-update
        sudo dnf upgrade
        sudo dnf install nginx
        sudo dnf remove nginx
        dnf search postgres
        dnf info nginx
        dnf history                   # past transactions, which can be undone
        ```

        ## Dependencies

        Most programs rely on shared **libraries** and other programs. Instead of every package bundling its own copy, Linux packages declare dependencies and share one copy. Install a tool that needs `libssl`, and the package manager pulls in `libssl` automatically. Update `libssl` once to fix a security bug, and every program using it is fixed. The flip side is that occasionally dependencies conflict or an interrupted install leaves things half done; `sudo dpkg --configure -a` and `sudo apt --fix-broken install` repair most such states on Debian systems.

        ## Repositories

        The list of repositories lives in `/etc/apt/sources.list` and `/etc/apt/sources.list.d/` on Debian systems, and in `/etc/yum.repos.d/` on Red Hat systems. Software vendors sometimes offer their own repositories, for example for Docker, PostgreSQL or Node.js, which you add along with their signing key. Only add repositories you trust: a repository can install anything as root.

        ## Asking questions about installed software

        ```
        dpkg -l | grep nginx          # is it installed, which version?
        dpkg -L nginx                 # which files did it install?
        dpkg -S /usr/sbin/nginx       # which package owns this file?
        rpm -qa | grep nginx          # the same questions on rpm systems
        rpm -ql nginx
        rpm -qf /usr/sbin/nginx
        ```

        ## Other ways to install software

        - **Snap** and **Flatpak** bundle an app with its dependencies and run it in a sandbox. Good for desktop apps and newer versions than the distribution offers.
        - **Language package managers** like `pip`, `npm` and `cargo` install libraries for one programming language. Prefer virtual environments or per-project installs so they do not fight with system packages.
        - **Containers** (Docker, Podman) ship a whole userland with the application.
        - **Building from source** with `./configure && make && sudo make install` puts files in `/usr/local`, out of the package manager's sight. It works, but you are then responsible for updates yourself.

        Whenever possible, install from your distribution's repositories first. That keeps the whole system updated with a single command, which is the best security habit there is.
        """);
}

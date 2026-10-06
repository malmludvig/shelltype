using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

public static partial class CommandLibrary
{
    private static readonly CommandEntry[] Packages = Build(CommandCategory.Packages,
        ("sudo apt update", "Refresh the package index on Debian or Ubuntu", Easy),
        ("sudo apt upgrade -y", "Upgrade all installed packages", Easy),
        ("sudo apt install git curl", "Install packages with apt", Easy),
        ("sudo apt remove --purge nginx", "Remove a package and its configuration", Medium),
        ("apt search ripgrep", "Search the package index", Easy),
        ("dpkg -l | grep python", "List installed packages matching python", Medium),
        ("sudo dnf install htop", "Install a package on Fedora", Easy),
        ("sudo pacman -Syu", "Sync and upgrade everything on Arch Linux", Easy),
        ("sudo pacman -S neovim", "Install a package on Arch Linux", Easy),
        ("sudo snap install code --classic", "Install a snap with classic confinement", Medium),
        ("flatpak install flathub org.gimp.GIMP", "Install an app from Flathub", Hard),
        ("pip install --user requests", "Install a Python package for your user", Medium),
        ("sudo apt autoremove", "Remove packages that are no longer needed", Easy),
        ("apt list --installed", "List every installed package", Medium),
        ("sudo dpkg -i package.deb", "Install a local .deb file", Medium));
}

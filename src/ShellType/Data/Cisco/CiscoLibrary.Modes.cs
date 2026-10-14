using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

public static partial class CiscoLibrary
{
    private static readonly CommandEntry[] Modes =
    [
        C(User, "enable", "Go from user EXEC (>) to privileged EXEC (#), where you can see and change everything", Easy),
        C(Exec, "disable", "Drop back from privileged EXEC to user EXEC", Easy),
        C(Exec, "configure terminal", "Enter global configuration mode to change the running configuration", Easy),
        C(If, "exit", "Go back one level, here from interface configuration to global configuration", Easy),
        C(If, "end", "Leave configuration mode completely and return to privileged EXEC", Easy),
        C("Router(config)#", "hostname R1", "Name the device; the new name shows up in the prompt at once", Easy),
        C(Exec, "copy running-config startup-config", "Save the active configuration to NVRAM so it survives a reboot", Medium),
        C(Exec, "reload", "Restart the device; unsaved changes to the running config are lost", Easy),
    ];
}

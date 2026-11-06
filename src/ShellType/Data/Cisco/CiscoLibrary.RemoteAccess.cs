using ShellType.Models;
using static ShellType.Models.Difficulty;

namespace ShellType.Data;

public static partial class CiscoLibrary
{
    private static readonly CommandEntry[] Passwords =
    [
        C(Config, "enable secret Cisco123", "Protect privileged EXEC with a hashed password", Medium),
        C(Config, "service password-encryption", "Hide plain-text passwords in the config with weak type 7 encryption", Medium),
        C(Config, "banner motd #Authorized access only#", "Set the message shown to everyone who connects", Hard),
    ];

    private static readonly CommandEntry[] RemoteAccess =
    [
        C(Config, "line console 0", "Configure the console port, the cable you plug in when you are standing at the device", Easy),
        C(Config, "line vty 0 4", "Configure the first five virtual terminal lines used for telnet and ssh", Easy),
        C(Line, "password Cisco123", "Set a password for logging in on this line", Medium),
        C(Line, "login local", "Ask for a username and password from the local user database", Easy),
        C(Config, "username admin secret Cisco123", "Create a local user with a hashed password", Medium),
        C(Config, "ip domain-name lab.local", "Set the domain name; needed together with the hostname to make ssh keys", Medium),
        C(Config, "crypto key generate rsa modulus 2048", "Create the RSA key pair that turns on the ssh server", Hard),
        C(Line, "transport input ssh", "Only allow ssh on these lines, never unencrypted telnet", Medium),
    ];
}

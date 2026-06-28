namespace DemoForge.Core.Safety;

public sealed class CommandSafetyChecker
{
    private static readonly string[] DangerousPatterns =
    [
        "rm -rf",
        "del /s",
        "format ",
        "shutdown",
        "reboot",
        "git clean -fdx",
        "docker system prune",
        "drop database",
        "truncate table"
    ];

    public bool IsDangerous(string command)
    {
        return DangerousPatterns.Any(pattern => command.Contains(pattern, StringComparison.OrdinalIgnoreCase));
    }
}

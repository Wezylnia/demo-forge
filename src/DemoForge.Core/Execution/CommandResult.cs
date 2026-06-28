namespace DemoForge.Core.Execution;

public sealed class CommandResult
{
    public int ExitCode { get; init; }
    public bool TimedOut { get; init; }
    public DateTimeOffset StartedAtUtc { get; init; }
    public DateTimeOffset EndedAtUtc { get; init; }
    public IReadOnlyList<(bool isStdErr, string line, TimeSpan offset)> OutputLines { get; init; } = [];
    public string ShellDisplayName { get; init; } = string.Empty;
}

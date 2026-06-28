namespace DemoForge.Core.Capture;

public sealed class ExecutedStepSession
{
    public string Title { get; init; } = string.Empty;
    public string Command { get; init; } = string.Empty;
    public string WorkingDirectory { get; init; } = string.Empty;
    public DateTimeOffset StartedAtUtc { get; init; }
    public DateTimeOffset EndedAtUtc { get; init; }
    public long DurationMs { get; init; }
    public int ExitCode { get; init; }
    public bool TimedOut { get; init; }
    public List<TerminalEvent> Events { get; init; } = [];
}

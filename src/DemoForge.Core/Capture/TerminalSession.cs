namespace DemoForge.Core.Capture;

public sealed class TerminalSession
{
    public string DemoName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Shell { get; init; } = string.Empty;
    public DateTimeOffset StartedAtUtc { get; init; }
    public long DurationMs { get; init; }
    public List<ExecutedStepSession> Steps { get; init; } = [];
}

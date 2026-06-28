namespace DemoForge.Core.Capture;

public sealed class TerminalEvent
{
    public TimeSpan Offset { get; init; }
    public TerminalEventKind Kind { get; init; }
    public string Text { get; init; } = string.Empty;
}

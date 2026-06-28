namespace DemoForge.Core.Capture;

public sealed class TerminalRecorder
{
    private readonly DateTimeOffset _startedAtUtc = DateTimeOffset.UtcNow;
    private readonly List<TerminalEvent> _events = [];

    public void Record(TerminalEventKind kind, string text)
    {
        _events.Add(new TerminalEvent
        {
            Kind = kind,
            Text = text,
            Offset = DateTimeOffset.UtcNow - _startedAtUtc
        });
    }

    public IReadOnlyList<TerminalEvent> Snapshot()
    {
        return _events.ToArray();
    }

    public DateTimeOffset StartedAtUtc => _startedAtUtc;
}

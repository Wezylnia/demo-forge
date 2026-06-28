using System.Text;
using DemoForge.Core.Capture;

namespace DemoForge.Core.Rendering;

public sealed class TranscriptRenderer
{
    public string Render(TerminalSession session)
    {
        var builder = new StringBuilder();
        builder.AppendLine(session.DemoName);
        builder.AppendLine(new string('=', session.DemoName.Length));
        builder.AppendLine();

        foreach (var step in session.Steps)
        {
            builder.AppendLine($"# {step.Title}");
            foreach (var terminalEvent in step.Events)
            {
                builder.AppendLine(RenderEvent(terminalEvent));
            }

            builder.AppendLine();
        }

        return builder.ToString().TrimEnd() + Environment.NewLine;
    }

    private static string RenderEvent(TerminalEvent terminalEvent)
    {
        return terminalEvent.Kind switch
        {
            TerminalEventKind.CommandTyped => terminalEvent.Text,
            TerminalEventKind.Stdout => terminalEvent.Text,
            TerminalEventKind.Stderr => $"[stderr] {terminalEvent.Text}",
            TerminalEventKind.ExitCode => $"[{terminalEvent.Text}]",
            _ => string.Empty
        };
    }
}

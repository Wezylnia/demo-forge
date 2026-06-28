using System.Text;
using DemoForge.Core.Capture;

namespace DemoForge.Core.Rendering;

public sealed class MarkdownRenderer
{
    public string Render(TerminalSession session)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"# {session.DemoName}");
        builder.AppendLine();

        if (!string.IsNullOrWhiteSpace(session.Description))
        {
            builder.AppendLine(session.Description);
            builder.AppendLine();
        }

        foreach (var step in session.Steps)
        {
            builder.AppendLine($"## {step.Title}");
            builder.AppendLine();
            builder.AppendLine("```bash");

            foreach (var terminalEvent in step.Events)
            {
                var line = terminalEvent.Kind switch
                {
                    TerminalEventKind.StepStarted or TerminalEventKind.StepFinished => null,
                    TerminalEventKind.Stderr => $"# stderr: {terminalEvent.Text}",
                    TerminalEventKind.ExitCode => $"# {terminalEvent.Text}",
                    _ => terminalEvent.Text
                };

                if (line is not null)
                {
                    builder.AppendLine(line);
                }
            }

            builder.AppendLine("```");
            builder.AppendLine();
        }

        return builder.ToString().TrimEnd() + Environment.NewLine;
    }
}

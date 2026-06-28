using System.Net;
using System.Text;
using DemoForge.Core.Capture;
using DemoForge.Core.Recipes;

namespace DemoForge.Core.Rendering;

public sealed class HtmlRenderer
{
    public string Render(TerminalSession session, TerminalOptions options)
    {
        var builder = new StringBuilder();
        builder.AppendLine("<!DOCTYPE html>");
        builder.AppendLine("<html lang=\"en\">");
        builder.AppendLine("<head>");
        builder.AppendLine("  <meta charset=\"utf-8\" />");
        builder.AppendLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />");
        builder.AppendLine($"  <title>{WebUtility.HtmlEncode(session.DemoName)}</title>");
        builder.AppendLine("  <style>");
        builder.AppendLine("    :root { --bg: #0b1220; --panel: #111827; --text: #e5e7eb; --muted: #94a3b8; --accent: #38bdf8; --stderr: #fca5a5; }");
        builder.AppendLine("    body { margin: 0; font-family: 'Cascadia Code', 'Fira Code', monospace; background: radial-gradient(circle at top, #172554 0%, #0b1220 55%); color: var(--text); }");
        builder.AppendLine("    main { max-width: 1000px; margin: 0 auto; padding: 48px 20px 80px; }");
        builder.AppendLine("    h1, h2 { font-family: 'Segoe UI', sans-serif; }");
        builder.AppendLine("    .terminal { background: rgba(15, 23, 42, 0.94); border: 1px solid rgba(148, 163, 184, 0.2); border-radius: 18px; overflow: hidden; box-shadow: 0 24px 80px rgba(15, 23, 42, 0.4); margin-bottom: 28px; }");
        builder.AppendLine("    .terminal-header { display: flex; justify-content: space-between; align-items: center; padding: 14px 18px; background: rgba(30, 41, 59, 0.92); color: var(--muted); font-size: 14px; }");
        builder.AppendLine("    .terminal-body { padding: 18px; font-size: 15px; line-height: 1.55; white-space: pre-wrap; }");
        builder.AppendLine("    .stderr { color: var(--stderr); }");
        builder.AppendLine("    .meta { color: var(--muted); font-size: 14px; margin-bottom: 28px; }");
        builder.AppendLine("    .pill { display: inline-block; padding: 4px 10px; border-radius: 999px; background: rgba(56, 189, 248, 0.12); color: var(--accent); margin-right: 8px; }");
        builder.AppendLine("  </style>");
        builder.AppendLine("</head>");
        builder.AppendLine("<body>");
        builder.AppendLine("  <main>");
        builder.AppendLine($"    <h1>{WebUtility.HtmlEncode(session.DemoName)}</h1>");
        if (!string.IsNullOrWhiteSpace(session.Description))
        {
            builder.AppendLine($"    <p class=\"meta\">{WebUtility.HtmlEncode(session.Description)}</p>");
        }

        builder.AppendLine($"    <p class=\"meta\"><span class=\"pill\">{WebUtility.HtmlEncode(session.Shell)}</span><span class=\"pill\">{options.Width}x{options.Height}</span></p>");

        foreach (var step in session.Steps)
        {
            builder.AppendLine("    <section class=\"terminal\">");
            builder.AppendLine($"      <div class=\"terminal-header\"><strong>{WebUtility.HtmlEncode(step.Title)}</strong><span>{step.DurationMs} ms</span></div>");
            builder.AppendLine("      <div class=\"terminal-body\">");

            foreach (var terminalEvent in step.Events)
            {
                if (terminalEvent.Kind is TerminalEventKind.StepStarted or TerminalEventKind.StepFinished)
                {
                    continue;
                }

                var cssClass = terminalEvent.Kind == TerminalEventKind.Stderr ? " class=\"stderr\"" : string.Empty;
                builder.AppendLine($"        <div{cssClass}>{WebUtility.HtmlEncode(terminalEvent.Text)}</div>");
            }

            builder.AppendLine("      </div>");
            builder.AppendLine("    </section>");
        }

        builder.AppendLine("  </main>");
        builder.AppendLine("</body>");
        builder.AppendLine("</html>");
        return builder.ToString();
    }
}

using DemoForge.Core.Capture;
using DemoForge.Core.Rendering;
using FluentAssertions;

namespace DemoForge.Core.Tests;

public sealed class MarkdownRendererTests
{
    [Fact]
    public void Render_should_include_commands_and_output()
    {
        var session = new TerminalSession
        {
            DemoName = "Demo",
            Steps =
            [
                new ExecutedStepSession
                {
                    Title = "Help",
                    Events =
                    [
                        new TerminalEvent { Kind = TerminalEventKind.CommandTyped, Text = "$ tool --help" },
                        new TerminalEvent { Kind = TerminalEventKind.Stdout, Text = "Usage: tool" },
                        new TerminalEvent { Kind = TerminalEventKind.ExitCode, Text = "exit code: 0" }
                    ]
                }
            ]
        };

        var markdown = new MarkdownRenderer().Render(session);

        markdown.Should().Contain("$ tool --help").And.Contain("Usage: tool").And.Contain("# exit code: 0");
    }
}

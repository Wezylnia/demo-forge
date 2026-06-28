using DemoForge.Core.Capture;
using DemoForge.Core.Recipes;
using DemoForge.Core.Rendering;
using FluentAssertions;

namespace DemoForge.Core.Tests;

public sealed class HtmlRendererTests
{
    [Fact]
    public void Render_should_create_terminal_markup()
    {
        var session = new TerminalSession
        {
            DemoName = "Demo",
            Steps =
            [
                new ExecutedStepSession
                {
                    Title = "Help",
                    DurationMs = 42,
                    Events =
                    [
                        new TerminalEvent { Kind = TerminalEventKind.CommandTyped, Text = "$ tool --help" },
                        new TerminalEvent { Kind = TerminalEventKind.Stdout, Text = "Usage: tool" }
                    ]
                }
            ]
        };

        var html = new HtmlRenderer().Render(session, new TerminalOptions());

        html.Should().Contain("<!DOCTYPE html>").And.Contain("terminal").And.Contain("Usage: tool");
    }
}

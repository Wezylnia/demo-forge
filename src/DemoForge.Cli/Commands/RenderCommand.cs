using DemoForge.Cli.Infrastructure;
using DemoForge.Core.Recipes;
using DemoForge.Core.Rendering;
using Spectre.Console;
using Spectre.Console.Cli;

namespace DemoForge.Cli.Commands;

public sealed class RenderCommand : AsyncCommand<RenderCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<session>")]
        public string SessionPath { get; init; } = string.Empty;

        [CommandOption("--format <FORMAT>")]
        public string Format { get; init; } = ArtifactNames.HtmlFormat;

        [CommandOption("--output <PATH>")]
        public string OutputDirectory { get; init; } = "assets/rendered";
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var session = AppHost.SessionStore.Load(settings.SessionPath);
        var formats = settings.Format.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        await AppHost.Services.ExportManager.RenderFromSessionAsync(
            session,
            new TerminalOptions(),
            new ReadmeOptions(),
            Path.GetFullPath(settings.OutputDirectory),
            formats,
            cancellationToken);

        AnsiConsole.MarkupLine($"[green]Rendered session into:[/] {Path.GetFullPath(settings.OutputDirectory)}");
        return 0;
    }
}

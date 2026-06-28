using DemoForge.Cli.Infrastructure;
using DemoForge.Core.Execution;
using Spectre.Console;
using Spectre.Console.Cli;

namespace DemoForge.Cli.Commands;

public sealed class RunCommand : AsyncCommand<RunCommand.Settings>
{
    public sealed class Settings : RecipeSettings
    {
        [CommandOption("--dry-run")]
        public bool DryRun { get; init; }

        [CommandOption("--output <PATH>")]
        public string? OutputOverride { get; init; }

        [CommandOption("--allow-dangerous")]
        public bool AllowDangerous { get; init; }
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var recipePath = settings.ResolveRecipePath();
        var loaded = AppHost.Services.RecipeLoader.Load(recipePath);
        var projectRoot = ProjectRootResolver.ResolveFromRecipePath(loaded.SourcePath);

        var result = await AppHost.Services.DemoRunner.RunAsync(
            loaded.Recipe,
            projectRoot,
            new DemoRunOptions
            {
                DryRun = settings.DryRun,
                AllowDangerousCommands = settings.AllowDangerous,
                OutputOverride = settings.OutputOverride
            },
            cancellationToken);

        if (result.WasDryRun)
        {
            AnsiConsole.MarkupLine("[yellow]Dry run. Commands that would run:[/]");
            foreach (var step in result.Session.Steps)
            {
                AnsiConsole.MarkupLine($"- {Markup.Escape(step.Command)}");
            }

            return 0;
        }

        AnsiConsole.MarkupLine("[green]Demo generated successfully.[/]");
        foreach (var format in result.Manifest.Formats)
        {
            AnsiConsole.MarkupLine($"- {Markup.Escape(format.Key)}: {Markup.Escape(Path.Combine(result.Manifest.OutputDirectory, format.Value))}");
        }

        return 0;
    }
}

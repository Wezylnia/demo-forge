using DemoForge.Cli.Infrastructure;
using Spectre.Console;
using Spectre.Console.Cli;

namespace DemoForge.Cli.Commands;

public sealed class ValidateCommand : Command<RecipeSettings>
{
    protected override int Execute(CommandContext context, RecipeSettings settings, CancellationToken cancellationToken)
    {
        var recipePath = settings.ResolveRecipePath();
        var loaded = AppHost.Services.RecipeLoader.Load(recipePath);
        var validation = AppHost.Services.RecipeValidator.Validate(loaded.Recipe, ProjectRootResolver.ResolveFromRecipePath(loaded.SourcePath));

        if (!validation.IsValid)
        {
            AnsiConsole.MarkupLine("[red]Recipe validation failed:[/]");
            foreach (var error in validation.Errors)
            {
                AnsiConsole.MarkupLine($"[red]-[/] {Markup.Escape(error)}");
            }

            return 1;
        }

        AnsiConsole.MarkupLine($"[green]Recipe is valid:[/] {loaded.SourcePath}");
        return 0;
    }
}

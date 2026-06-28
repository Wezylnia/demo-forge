using Spectre.Console.Cli;

namespace DemoForge.Cli.Commands;

public class RecipeSettings : CommandSettings
{
    [CommandArgument(0, "[recipe]")]
    public string? RecipePath { get; init; }

    public string ResolveRecipePath()
    {
        return string.IsNullOrWhiteSpace(RecipePath) ? Path.Combine(".demoforge", "demo.yml") : RecipePath;
    }
}

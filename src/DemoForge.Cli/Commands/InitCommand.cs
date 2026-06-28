using Spectre.Console;
using Spectre.Console.Cli;

namespace DemoForge.Cli.Commands;

public sealed class InitCommand : Command<InitCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandOption("--template <TEMPLATE>")]
        public string Template { get; init; } = "cli";

        [CommandOption("--output <PATH>")]
        public string Output { get; init; } = Path.Combine(".demoforge", "demo.yml");

        [CommandOption("--force")]
        public bool Force { get; init; }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        if (!string.Equals(settings.Template, "cli", StringComparison.OrdinalIgnoreCase))
        {
            AnsiConsole.MarkupLine("[red]Only the 'cli' template is available in this build.[/]");
            return 1;
        }

        var outputPath = Path.GetFullPath(settings.Output);
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

        if (File.Exists(outputPath) && !settings.Force)
        {
            AnsiConsole.MarkupLine($"[red]Recipe already exists:[/] {outputPath}");
            AnsiConsole.MarkupLine("Use [yellow]--force[/] to overwrite it.");
            return 1;
        }

        File.WriteAllText(outputPath, GetTemplate());
        AnsiConsole.MarkupLine($"[green]Created recipe:[/] {outputPath}");
        return 0;
    }

    private static string GetTemplate()
    {
        return """
version: 1

demo:
  name: "My CLI Demo"
  description: "Show the core flow of my CLI."
  outputDir: "assets/demo"
  cleanOutputDir: true

terminal:
  theme: "dark"
  width: 100
  height: 28
  fontSize: 16
  prompt: "$"
  showPrompt: true

workingDirectory: "."

setup:
  - title: "Build the sample CLI"
    command: "dotnet build"
    timeoutSeconds: 120

steps:
  - title: "Show help"
    command: "dotnet run --project samples/fake-cli/FakeCli -- --help"
    timeoutSeconds: 30
    expectExitCode: 0

export:
  formats:
    - transcript
    - session-json
    - markdown
    - html
    - readme-snippet
  readme:
    title: "Demo"
    relativeAssetPath: "assets/demo/demo.html"
""";
    }
}

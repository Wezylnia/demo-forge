namespace DemoForge.Core.Recipes;

public sealed class DemoRecipe
{
    public int Version { get; init; } = 1;
    public DemoInfo Demo { get; init; } = new();
    public TerminalOptions Terminal { get; init; } = new();
    public string WorkingDirectory { get; init; } = ".";
    public Dictionary<string, string> Env { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    public List<CommandStep> Setup { get; init; } = [];
    public List<CommandStep> Steps { get; init; } = [];
    public RedactionOptions Redact { get; init; } = new();
    public ExportOptions Export { get; init; } = new();
}

public sealed class DemoInfo
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string OutputDir { get; init; } = "assets/demo";
    public bool CleanOutputDir { get; init; }
}

public sealed class TerminalOptions
{
    public string Theme { get; init; } = "dark";
    public int Width { get; init; } = 100;
    public int Height { get; init; } = 28;
    public int FontSize { get; init; } = 16;
    public string Prompt { get; init; } = "$";
    public bool ShowPrompt { get; init; } = true;
    public bool TypeCommand { get; init; } = true;
    public int TypingSpeed { get; init; } = 35;
    public int LineDelayMs { get; init; } = 60;
    public int StepDelayMs { get; init; } = 900;
}

public sealed class CommandStep
{
    public string Title { get; init; } = string.Empty;
    public string Command { get; init; } = string.Empty;
    public string WorkingDirectory { get; init; } = string.Empty;
    public int TimeoutSeconds { get; init; }
    public int ExpectExitCode { get; init; }
    public bool ContinueOnError { get; init; }
}

public sealed class RedactionOptions
{
    public List<RegexRedactionPattern> Patterns { get; init; } = [];
    public List<LiteralRedactionPattern> Literals { get; init; } = [];
}

public sealed class RegexRedactionPattern
{
    public string Name { get; init; } = string.Empty;
    public string Regex { get; init; } = string.Empty;
    public string Replacement { get; init; } = "[redacted]";
}

public sealed class LiteralRedactionPattern
{
    public string Value { get; init; } = string.Empty;
    public string Replacement { get; init; } = "[redacted]";
}

public sealed class ExportOptions
{
    public List<string> Formats { get; init; } = ["transcript", "session-json", "markdown", "html", "readme-snippet"];
    public ReadmeOptions Readme { get; init; } = new();
}

public sealed class ReadmeOptions
{
    public string Title { get; init; } = "Demo";
    public string RelativeAssetPath { get; init; } = "assets/demo/demo.html";
}

using DemoForge.Core.Execution;
using DemoForge.Core.Export;
using DemoForge.Core.Recipes;
using DemoForge.Core.Rendering;
using DemoForge.Core.Safety;

namespace DemoForge.Cli.Infrastructure;

public sealed class AppServices
{
    public RecipeLoader RecipeLoader { get; } = new();
    public RecipeValidator RecipeValidator { get; } = new();
    public ProcessExecutor ProcessExecutor { get; } = new();
    public TranscriptRenderer TranscriptRenderer { get; } = new();
    public MarkdownRenderer MarkdownRenderer { get; } = new();
    public HtmlRenderer HtmlRenderer { get; } = new();
    public ReadmeSnippetRenderer ReadmeSnippetRenderer { get; } = new();
    public CommandSafetyChecker CommandSafetyChecker { get; } = new();

    public ExportManager ExportManager => new(
        TranscriptRenderer,
        MarkdownRenderer,
        HtmlRenderer,
        ReadmeSnippetRenderer);

    public DemoRunner DemoRunner => new(
        RecipeValidator,
        new StepRunner(ProcessExecutor),
        ExportManager,
        CommandSafetyChecker);
}

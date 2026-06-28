using System.Text.Json;
using System.Text.Json.Serialization;
using DemoForge.Core.Capture;
using DemoForge.Core.Recipes;
using DemoForge.Core.Rendering;
using DemoForge.Core.Safety;

namespace DemoForge.Core.Export;

public sealed class ExportManager
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly TranscriptRenderer _transcriptRenderer;
    private readonly MarkdownRenderer _markdownRenderer;
    private readonly HtmlRenderer _htmlRenderer;
    private readonly ReadmeSnippetRenderer _readmeSnippetRenderer;

    public ExportManager(
        TranscriptRenderer transcriptRenderer,
        MarkdownRenderer markdownRenderer,
        HtmlRenderer htmlRenderer,
        ReadmeSnippetRenderer readmeSnippetRenderer)
    {
        _transcriptRenderer = transcriptRenderer;
        _markdownRenderer = markdownRenderer;
        _htmlRenderer = htmlRenderer;
        _readmeSnippetRenderer = readmeSnippetRenderer;
    }

    public async Task<OutputManifest> ExportAsync(
        DemoRecipe recipe,
        TerminalSession session,
        string projectRoot,
        string? outputOverride,
        CancellationToken cancellationToken)
    {
        var outputDirectory = Path.GetFullPath(Path.Combine(projectRoot, outputOverride ?? recipe.Demo.OutputDir));
        var manifest = new OutputManifest(recipe.Demo.Name, outputDirectory);
        var redactor = new Redactor(recipe.Redact);

        if (recipe.Demo.CleanOutputDir && Directory.Exists(outputDirectory))
        {
            Directory.Delete(outputDirectory, recursive: true);
        }

        Directory.CreateDirectory(outputDirectory);
        var redactedSession = RedactSession(session, redactor);

        foreach (var format in recipe.Export.Formats)
        {
            cancellationToken.ThrowIfCancellationRequested();

            switch (format.ToLowerInvariant())
            {
                case ArtifactNames.TranscriptFormat:
                    await WriteAsync(outputDirectory, ArtifactNames.TranscriptFileName, _transcriptRenderer.Render(redactedSession), cancellationToken);
                    manifest.Formats[ArtifactNames.TranscriptFormat] = ArtifactNames.TranscriptFileName;
                    break;
                case ArtifactNames.SessionJsonFormat:
                    await WriteAsync(outputDirectory, ArtifactNames.SessionJsonFileName, JsonSerializer.Serialize(redactedSession, JsonOptions), cancellationToken);
                    manifest.Formats[ArtifactNames.SessionJsonFormat] = ArtifactNames.SessionJsonFileName;
                    break;
                case ArtifactNames.MarkdownFormat:
                    await WriteAsync(outputDirectory, ArtifactNames.MarkdownFileName, _markdownRenderer.Render(redactedSession), cancellationToken);
                    manifest.Formats[ArtifactNames.MarkdownFormat] = ArtifactNames.MarkdownFileName;
                    break;
                case ArtifactNames.HtmlFormat:
                    await WriteAsync(outputDirectory, ArtifactNames.HtmlFileName, _htmlRenderer.Render(redactedSession, recipe.Terminal), cancellationToken);
                    manifest.Formats[ArtifactNames.HtmlFormat] = ArtifactNames.HtmlFileName;
                    break;
                case ArtifactNames.ReadmeSnippetFormat:
                    await WriteAsync(outputDirectory, ArtifactNames.ReadmeSnippetFileName, _readmeSnippetRenderer.Render(redactedSession, recipe.Export.Readme), cancellationToken);
                    manifest.Formats[ArtifactNames.ReadmeSnippetFormat] = ArtifactNames.ReadmeSnippetFileName;
                    break;
            }
        }

        await WriteAsync(outputDirectory, ArtifactNames.ManifestFileName, JsonSerializer.Serialize(manifest, JsonOptions), cancellationToken);
        manifest.Formats["manifest"] = ArtifactNames.ManifestFileName;

        return manifest;
    }

    public async Task RenderFromSessionAsync(
        TerminalSession session,
        TerminalOptions options,
        ReadmeOptions readmeOptions,
        string outputDirectory,
        IReadOnlyList<string> formats,
        CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(outputDirectory);

        foreach (var format in formats)
        {
            cancellationToken.ThrowIfCancellationRequested();

            switch (format.ToLowerInvariant())
            {
                case ArtifactNames.MarkdownFormat:
                    await WriteAsync(outputDirectory, ArtifactNames.MarkdownFileName, _markdownRenderer.Render(session), cancellationToken);
                    break;
                case ArtifactNames.HtmlFormat:
                    await WriteAsync(outputDirectory, ArtifactNames.HtmlFileName, _htmlRenderer.Render(session, options), cancellationToken);
                    break;
                case ArtifactNames.TranscriptFormat:
                    await WriteAsync(outputDirectory, ArtifactNames.TranscriptFileName, _transcriptRenderer.Render(session), cancellationToken);
                    break;
                case ArtifactNames.ReadmeSnippetFormat:
                    await WriteAsync(outputDirectory, ArtifactNames.ReadmeSnippetFileName, _readmeSnippetRenderer.Render(session, readmeOptions), cancellationToken);
                    break;
            }
        }
    }

    private static TerminalSession RedactSession(TerminalSession session, Redactor redactor)
    {
        return new TerminalSession
        {
            DemoName = redactor.Apply(session.DemoName),
            Description = redactor.Apply(session.Description),
            Shell = session.Shell,
            StartedAtUtc = session.StartedAtUtc,
            DurationMs = session.DurationMs,
            Steps = session.Steps.Select(step => new ExecutedStepSession
            {
                Title = redactor.Apply(step.Title),
                Command = redactor.Apply(step.Command),
                WorkingDirectory = step.WorkingDirectory,
                StartedAtUtc = step.StartedAtUtc,
                EndedAtUtc = step.EndedAtUtc,
                DurationMs = step.DurationMs,
                ExitCode = step.ExitCode,
                TimedOut = step.TimedOut,
                Events = step.Events.Select(terminalEvent => new TerminalEvent
                {
                    Offset = terminalEvent.Offset,
                    Kind = terminalEvent.Kind,
                    Text = redactor.Apply(terminalEvent.Text)
                }).ToList()
            }).ToList()
        };
    }

    private static Task WriteAsync(string directory, string fileName, string content, CancellationToken cancellationToken)
    {
        return File.WriteAllTextAsync(Path.Combine(directory, fileName), content, cancellationToken);
    }
}

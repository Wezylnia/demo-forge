using DemoForge.Core.Rendering;

namespace DemoForge.Core.Recipes;

public sealed class RecipeValidator
{
    private static readonly HashSet<string> KnownFormats = new(StringComparer.OrdinalIgnoreCase)
    {
        ArtifactNames.TranscriptFormat,
        ArtifactNames.SessionJsonFormat,
        ArtifactNames.MarkdownFormat,
        ArtifactNames.HtmlFormat,
        ArtifactNames.ReadmeSnippetFormat,
        ArtifactNames.GifFormat,
        ArtifactNames.Mp4Format
    };

    private static readonly HashSet<string> ImplementedFormats = new(StringComparer.OrdinalIgnoreCase)
    {
        ArtifactNames.TranscriptFormat,
        ArtifactNames.SessionJsonFormat,
        ArtifactNames.MarkdownFormat,
        ArtifactNames.HtmlFormat,
        ArtifactNames.ReadmeSnippetFormat
    };

    public RecipeValidationResult Validate(DemoRecipe recipe, string projectRoot)
    {
        var result = new RecipeValidationResult();

        if (recipe.Version != 1)
        {
            result.Errors.Add("version must be 1.");
        }

        if (string.IsNullOrWhiteSpace(recipe.Demo.Name))
        {
            result.Errors.Add("demo.name is required.");
        }

        if (string.IsNullOrWhiteSpace(recipe.Demo.OutputDir))
        {
            result.Errors.Add("demo.outputDir is required.");
        }
        else
        {
            var outputDir = ResolvePath(projectRoot, recipe.Demo.OutputDir);
            var rootPath = Path.GetFullPath(projectRoot);

            if (string.Equals(outputDir, rootPath, StringComparison.OrdinalIgnoreCase))
            {
                result.Errors.Add("demo.outputDir must not be the project root.");
            }

            var rootInfo = new DirectoryInfo(rootPath).Root.FullName;
            if (string.Equals(outputDir, rootInfo, StringComparison.OrdinalIgnoreCase))
            {
                result.Errors.Add("demo.outputDir must not be a filesystem root.");
            }
        }

        if (recipe.Steps.Count == 0)
        {
            result.Errors.Add("at least one step is required.");
        }

        ValidateSteps(recipe.Setup, "setup", result);
        ValidateSteps(recipe.Steps, "steps", result);
        ValidateExportFormats(recipe.Export.Formats, result);
        ValidateRedactions(recipe.Redact, result);
        ValidateTerminal(recipe.Terminal, result);

        return result;
    }

    private static void ValidateSteps(IReadOnlyList<CommandStep> steps, string label, RecipeValidationResult result)
    {
        for (var index = 0; index < steps.Count; index++)
        {
            var step = steps[index];
            if (string.IsNullOrWhiteSpace(step.Command))
            {
                result.Errors.Add($"{label}[{index}].command is required.");
            }

            if (step.TimeoutSeconds < 0)
            {
                result.Errors.Add($"{label}[{index}].timeoutSeconds must be positive when provided.");
            }
        }
    }

    private static void ValidateExportFormats(IReadOnlyList<string> formats, RecipeValidationResult result)
    {
        if (formats.Count == 0)
        {
            result.Errors.Add("export.formats must contain at least one value.");
            return;
        }

        foreach (var format in formats)
        {
            if (!KnownFormats.Contains(format))
            {
                result.Errors.Add($"export.formats contains unsupported format: {format}.");
                continue;
            }

            if (!ImplementedFormats.Contains(format))
            {
                result.Errors.Add($"Format '{format}' is not supported in this build yet.");
            }
        }
    }

    private static void ValidateRedactions(RedactionOptions redact, RecipeValidationResult result)
    {
        for (var index = 0; index < redact.Patterns.Count; index++)
        {
            var pattern = redact.Patterns[index];
            if (string.IsNullOrWhiteSpace(pattern.Regex))
            {
                result.Errors.Add($"redact.patterns[{index}].regex is required.");
                continue;
            }

            try
            {
                _ = new System.Text.RegularExpressions.Regex(pattern.Regex);
            }
            catch (ArgumentException)
            {
                result.Errors.Add($"redact.patterns[{index}].regex is not a valid regular expression.");
            }
        }

        for (var index = 0; index < redact.Literals.Count; index++)
        {
            if (string.IsNullOrWhiteSpace(redact.Literals[index].Value))
            {
                result.Errors.Add($"redact.literals[{index}].value is required.");
            }
        }
    }

    private static void ValidateTerminal(TerminalOptions terminal, RecipeValidationResult result)
    {
        if (terminal.Width <= 0)
        {
            result.Errors.Add("terminal.width must be positive.");
        }

        if (terminal.Height <= 0)
        {
            result.Errors.Add("terminal.height must be positive.");
        }

        if (terminal.FontSize <= 0)
        {
            result.Errors.Add("terminal.fontSize must be positive.");
        }
    }

    private static string ResolvePath(string projectRoot, string path)
    {
        return Path.GetFullPath(Path.IsPathRooted(path) ? path : Path.Combine(projectRoot, path));
    }
}

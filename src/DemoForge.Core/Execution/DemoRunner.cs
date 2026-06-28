using DemoForge.Core.Capture;
using DemoForge.Core.Export;
using DemoForge.Core.Recipes;
using DemoForge.Core.Safety;

namespace DemoForge.Core.Execution;

public sealed class DemoRunner
{
    private readonly RecipeValidator _recipeValidator;
    private readonly StepRunner _stepRunner;
    private readonly ExportManager _exportManager;
    private readonly CommandSafetyChecker _commandSafetyChecker;

    public DemoRunner(
        RecipeValidator recipeValidator,
        StepRunner stepRunner,
        ExportManager exportManager,
        CommandSafetyChecker commandSafetyChecker)
    {
        _recipeValidator = recipeValidator;
        _stepRunner = stepRunner;
        _exportManager = exportManager;
        _commandSafetyChecker = commandSafetyChecker;
    }

    public async Task<DemoRunResult> RunAsync(
        DemoRecipe recipe,
        string projectRoot,
        DemoRunOptions options,
        CancellationToken cancellationToken)
    {
        var validation = _recipeValidator.Validate(recipe, projectRoot);
        if (!validation.IsValid)
        {
            throw new InvalidOperationException("Recipe validation failed:" + Environment.NewLine + string.Join(Environment.NewLine, validation.Errors.Select(error => $"- {error}")));
        }

        var dangerousCommands = recipe.Setup.Concat(recipe.Steps)
            .Where(step => _commandSafetyChecker.IsDangerous(step.Command))
            .Select(step => step.Command)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (dangerousCommands.Length > 0 && !options.AllowDangerousCommands)
        {
            throw new InvalidOperationException("Refusing to run dangerous commands without --allow-dangerous:" + Environment.NewLine + string.Join(Environment.NewLine, dangerousCommands.Select(command => $"- {command}")));
        }

        var outputDir = Path.GetFullPath(Path.Combine(projectRoot, options.OutputOverride ?? recipe.Demo.OutputDir));
        if (options.DryRun)
        {
            var dryRunSession = new TerminalSession
            {
                DemoName = recipe.Demo.Name,
                Description = recipe.Demo.Description,
                Shell = OperatingSystem.IsWindows() ? "powershell" : "bash",
                StartedAtUtc = DateTimeOffset.UtcNow,
                DurationMs = 0,
                Steps = recipe.Setup.Concat(recipe.Steps).Select((step, index) => new ExecutedStepSession
                {
                    Title = string.IsNullOrWhiteSpace(step.Title) ? $"Step {index + 1}" : step.Title,
                    Command = step.Command,
                    WorkingDirectory = ResolveWorkingDirectory(projectRoot, recipe.WorkingDirectory, step.WorkingDirectory),
                    StartedAtUtc = DateTimeOffset.UtcNow,
                    EndedAtUtc = DateTimeOffset.UtcNow,
                    DurationMs = 0,
                    ExitCode = 0,
                    TimedOut = false
                }).ToList()
            };

            var dryRunManifest = new OutputManifest(recipe.Demo.Name, outputDir);
            return new DemoRunResult { Session = dryRunSession, Manifest = dryRunManifest, WasDryRun = true };
        }

        var sessionStartedAt = DateTimeOffset.UtcNow;
        var sessionSteps = new List<ExecutedStepSession>();

        foreach (var step in recipe.Setup)
        {
            sessionSteps.Add(await RunStepAsync(step, recipe, projectRoot, isSetup: true, cancellationToken));
        }

        foreach (var step in recipe.Steps)
        {
            var executedStep = await RunStepAsync(step, recipe, projectRoot, isSetup: false, cancellationToken);
            sessionSteps.Add(executedStep);

            var expectedExitCode = step.ExpectExitCode;
            var failed = executedStep.TimedOut || executedStep.ExitCode != expectedExitCode;
            if (failed && !step.ContinueOnError)
            {
                break;
            }
        }

        var session = new TerminalSession
        {
            DemoName = recipe.Demo.Name,
            Description = recipe.Demo.Description,
            Shell = OperatingSystem.IsWindows() ? "powershell" : "bash",
            StartedAtUtc = sessionStartedAt,
            DurationMs = (long)(DateTimeOffset.UtcNow - sessionStartedAt).TotalMilliseconds,
            Steps = sessionSteps
        };

        var manifest = await _exportManager.ExportAsync(recipe, session, projectRoot, options.OutputOverride, cancellationToken);
        return new DemoRunResult { Session = session, Manifest = manifest, WasDryRun = false };
    }

    private async Task<ExecutedStepSession> RunStepAsync(CommandStep step, DemoRecipe recipe, string projectRoot, bool isSetup, CancellationToken cancellationToken)
    {
        var observer = new StepRunner.RecordingObserver();
        var workingDirectory = ResolveWorkingDirectory(projectRoot, recipe.WorkingDirectory, step.WorkingDirectory);

        return await _stepRunner.RunAsync(
            step,
            workingDirectory,
            recipe.Env,
            recipe.Terminal.Prompt,
            recipe.Terminal.ShowPrompt,
            observer,
            isSetup,
            cancellationToken);
    }

    private static string ResolveWorkingDirectory(string projectRoot, string recipeWorkingDirectory, string stepWorkingDirectory)
    {
        var path = string.IsNullOrWhiteSpace(stepWorkingDirectory) ? recipeWorkingDirectory : stepWorkingDirectory;
        return Path.GetFullPath(Path.Combine(projectRoot, path));
    }
}

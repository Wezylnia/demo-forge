using DemoForge.Core.Capture;
using DemoForge.Core.Recipes;

namespace DemoForge.Core.Execution;

public sealed class StepRunner
{
    private readonly ProcessExecutor _processExecutor;

    public StepRunner(ProcessExecutor processExecutor)
    {
        _processExecutor = processExecutor;
    }

    public async Task<ExecutedStepSession> RunAsync(
        CommandStep step,
        string workingDirectory,
        IReadOnlyDictionary<string, string> environmentVariables,
        string prompt,
        bool showPrompt,
        IStepObserver observer,
        bool isSetup,
        CancellationToken cancellationToken)
    {
        var title = string.IsNullOrWhiteSpace(step.Title)
            ? (isSetup ? "Setup step" : "Demo step")
            : step.Title;

        observer.OnEvent(TerminalEventKind.StepStarted, title);
        observer.OnEvent(TerminalEventKind.CommandTyped, showPrompt ? $"{prompt} {step.Command}" : step.Command);

        var timeout = TimeSpan.FromSeconds(step.TimeoutSeconds > 0 ? step.TimeoutSeconds : isSetup ? 120 : 60);
        var result = await _processExecutor.ExecuteAsync(new ProcessExecutionRequest
        {
            Command = step.Command,
            WorkingDirectory = workingDirectory,
            EnvironmentVariables = environmentVariables,
            Timeout = timeout
        }, cancellationToken);

        foreach (var line in result.OutputLines)
        {
            observer.OnEvent(line.isStdErr ? TerminalEventKind.Stderr : TerminalEventKind.Stdout, line.line);
        }

        var exitText = result.TimedOut
            ? $"timed out after {timeout.TotalSeconds:0} seconds"
            : $"exit code: {result.ExitCode}";

        observer.OnEvent(TerminalEventKind.ExitCode, exitText);
        observer.OnEvent(TerminalEventKind.StepFinished, title);

        return new ExecutedStepSession
        {
            Title = title,
            Command = step.Command,
            WorkingDirectory = workingDirectory,
            StartedAtUtc = result.StartedAtUtc,
            EndedAtUtc = result.EndedAtUtc,
            DurationMs = (long)(result.EndedAtUtc - result.StartedAtUtc).TotalMilliseconds,
            ExitCode = result.ExitCode,
            TimedOut = result.TimedOut,
            Events = observer switch
            {
                RecordingObserver recordingObserver => recordingObserver.Events.ToList(),
                _ => []
            }
        };
    }

    public sealed class RecordingObserver : IStepObserver
    {
        private readonly TerminalRecorder _recorder = new();

        public void OnEvent(TerminalEventKind kind, string text)
        {
            _recorder.Record(kind, text);
        }

        public IReadOnlyList<TerminalEvent> Events => _recorder.Snapshot();
    }
}

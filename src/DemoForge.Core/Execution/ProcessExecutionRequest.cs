namespace DemoForge.Core.Execution;

public sealed class ProcessExecutionRequest
{
    public string Command { get; init; } = string.Empty;
    public string WorkingDirectory { get; init; } = string.Empty;
    public IReadOnlyDictionary<string, string> EnvironmentVariables { get; init; } = new Dictionary<string, string>();
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(60);
}

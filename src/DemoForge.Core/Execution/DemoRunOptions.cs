namespace DemoForge.Core.Execution;

public sealed class DemoRunOptions
{
    public bool DryRun { get; init; }
    public bool AllowDangerousCommands { get; init; }
    public string? OutputOverride { get; init; }
}

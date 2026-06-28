using DemoForge.Core.Capture;
using DemoForge.Core.Export;

namespace DemoForge.Core.Execution;

public sealed class DemoRunResult
{
    public required TerminalSession Session { get; init; }
    public required OutputManifest Manifest { get; init; }
    public bool WasDryRun { get; init; }
}

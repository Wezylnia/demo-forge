using DemoForge.Core.Capture;

namespace DemoForge.Core.Execution;

public interface IStepObserver
{
    void OnEvent(TerminalEventKind kind, string text);
}

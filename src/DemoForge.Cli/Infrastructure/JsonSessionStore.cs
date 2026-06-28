using System.Text.Json;
using DemoForge.Core.Capture;

namespace DemoForge.Cli.Infrastructure;

public sealed class JsonSessionStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public TerminalSession Load(string sessionPath)
    {
        var fullPath = Path.GetFullPath(sessionPath);
        var content = File.ReadAllText(fullPath);
        return JsonSerializer.Deserialize<TerminalSession>(content, JsonOptions)
            ?? throw new InvalidOperationException($"Could not deserialize session file: {fullPath}");
    }
}

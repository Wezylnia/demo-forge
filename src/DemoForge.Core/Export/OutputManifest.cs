namespace DemoForge.Core.Export;

public sealed class OutputManifest
{
    public OutputManifest(string name, string outputDirectory)
    {
        Name = name;
        OutputDirectory = outputDirectory;
    }

    public string Name { get; }
    public string OutputDirectory { get; }
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public Dictionary<string, string> Formats { get; } = new(StringComparer.OrdinalIgnoreCase);
}

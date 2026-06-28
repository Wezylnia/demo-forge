namespace DemoForge.Cli.Infrastructure;

public static class AppHost
{
    public static AppServices Services { get; } = new();
    public static JsonSessionStore SessionStore { get; } = new();
}

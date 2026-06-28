namespace DemoForge.Cli.Infrastructure;

public static class ProjectRootResolver
{
    public static string ResolveFromRecipePath(string recipePath)
    {
        var recipeDirectory = Path.GetDirectoryName(Path.GetFullPath(recipePath))!;
        if (string.Equals(Path.GetFileName(recipeDirectory), ".demoforge", StringComparison.OrdinalIgnoreCase))
        {
            return Directory.GetParent(recipeDirectory)!.FullName;
        }

        var current = new DirectoryInfo(recipeDirectory);
        while (current is not null)
        {
            if (Directory.Exists(Path.Combine(current.FullName, ".git")) ||
                File.Exists(Path.Combine(current.FullName, "global.json")) ||
                current.GetFiles("*.sln").Length > 0 ||
                current.GetFiles("*.slnx").Length > 0)
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        return recipeDirectory;
    }
}

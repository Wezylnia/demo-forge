namespace DemoForge.Core.Recipes;

public sealed class RecipeLoadResult
{
    public RecipeLoadResult(DemoRecipe recipe, string sourcePath)
    {
        Recipe = recipe;
        SourcePath = sourcePath;
    }

    public DemoRecipe Recipe { get; }
    public string SourcePath { get; }
}

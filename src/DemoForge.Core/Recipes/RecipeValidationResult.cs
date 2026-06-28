namespace DemoForge.Core.Recipes;

public sealed class RecipeValidationResult
{
    public List<string> Errors { get; } = [];
    public bool IsValid => Errors.Count == 0;
}

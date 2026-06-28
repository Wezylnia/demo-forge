using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DemoForge.Core.Recipes;

public sealed class RecipeLoader
{
    private readonly IDeserializer _deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    public RecipeLoadResult Load(string recipePath)
    {
        var fullPath = Path.GetFullPath(recipePath);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Recipe file was not found: {fullPath}", fullPath);
        }

        var yaml = File.ReadAllText(fullPath);
        var recipe = _deserializer.Deserialize<DemoRecipe>(yaml) ?? new DemoRecipe();
        return new RecipeLoadResult(recipe, fullPath);
    }
}

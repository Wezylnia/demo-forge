using DemoForge.Core.Recipes;
using FluentAssertions;

namespace DemoForge.Core.Tests;

public sealed class RecipeLoaderTests
{
    [Fact]
    public void Load_should_parse_yaml_recipe()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path,
"""
version: 1
demo:
  name: "Demo"
  outputDir: "assets/demo"
steps:
  - command: "tool --help"
export:
  formats:
    - markdown
""");

            var result = new RecipeLoader().Load(path);

            result.Recipe.Demo.Name.Should().Be("Demo");
            result.Recipe.Steps.Should().ContainSingle();
        }
        finally
        {
            File.Delete(path);
        }
    }
}

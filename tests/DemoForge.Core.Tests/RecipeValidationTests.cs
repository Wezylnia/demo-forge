using DemoForge.Core.Recipes;
using FluentAssertions;

namespace DemoForge.Core.Tests;

public sealed class RecipeValidationTests
{
    [Fact]
    public void Validate_should_accept_minimal_valid_recipe()
    {
        var recipe = new DemoRecipe
        {
            Demo = new DemoInfo { Name = "Demo", OutputDir = "assets/demo" },
            Steps = [new CommandStep { Title = "Help", Command = "tool --help" }]
        };

        var result = new RecipeValidator().Validate(recipe, Environment.CurrentDirectory);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_should_reject_unimplemented_media_formats()
    {
        var recipe = new DemoRecipe
        {
            Demo = new DemoInfo { Name = "Demo", OutputDir = "assets/demo" },
            Steps = [new CommandStep { Command = "tool --help" }],
            Export = new ExportOptions { Formats = ["markdown", "gif"] }
        };

        var result = new RecipeValidator().Validate(recipe, Environment.CurrentDirectory);

        result.Errors.Should().Contain(error => error.Contains("Format 'gif' is not supported", StringComparison.Ordinal));
    }
}

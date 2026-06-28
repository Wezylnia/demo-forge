using DemoForge.Core.Recipes;
using DemoForge.Core.Safety;
using FluentAssertions;

namespace DemoForge.Core.Tests;

public sealed class RedactorTests
{
    [Fact]
    public void Apply_should_redact_default_and_custom_patterns()
    {
        var redactor = new Redactor(new RedactionOptions
        {
            Literals = [new LiteralRedactionPattern { Value = "SECRET_TOKEN", Replacement = "[secret]" }]
        });

        var output = redactor.Apply("mail me at test@example.com with SECRET_TOKEN");

        output.Should().Be("mail me at [redacted-email] with [secret]");
    }
}

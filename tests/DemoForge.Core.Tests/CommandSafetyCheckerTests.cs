using DemoForge.Core.Safety;
using FluentAssertions;

namespace DemoForge.Core.Tests;

public sealed class CommandSafetyCheckerTests
{
    [Theory]
    [InlineData("rm -rf ./tmp")]
    [InlineData("git clean -fdx")]
    public void IsDangerous_should_flag_known_patterns(string command)
    {
        new CommandSafetyChecker().IsDangerous(command).Should().BeTrue();
    }

    [Fact]
    public void IsDangerous_should_allow_normal_commands()
    {
        new CommandSafetyChecker().IsDangerous("dotnet build").Should().BeFalse();
    }
}

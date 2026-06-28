using DemoForge.Core.Capture;
using DemoForge.Core.Recipes;

namespace DemoForge.Core.Rendering;

public sealed class ReadmeSnippetRenderer
{
    public string Render(TerminalSession session, ReadmeOptions options)
    {
        return $"## {options.Title}{Environment.NewLine}{Environment.NewLine}Open the generated CLI demo:{Environment.NewLine}{Environment.NewLine}[View demo]({options.RelativeAssetPath}){Environment.NewLine}";
    }
}

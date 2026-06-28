using System.Text.RegularExpressions;
using DemoForge.Core.Recipes;

namespace DemoForge.Core.Safety;

public sealed class Redactor
{
    private readonly IReadOnlyList<(Regex regex, string replacement)> _regexPatterns;
    private readonly IReadOnlyList<(string value, string replacement)> _literalPatterns;

    public Redactor(RedactionOptions options)
    {
        var regexPatterns = new List<(Regex regex, string replacement)>
        {
            (new Regex(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}", RegexOptions.Compiled), "[redacted-email]"),
            (new Regex(@"\bgh[pousr]_[A-Za-z0-9]{20,}\b", RegexOptions.Compiled), "[redacted-github-token]"),
            (new Regex(@"\beyJ[A-Za-z0-9_-]+\.[A-Za-z0-9._-]+\.[A-Za-z0-9._-]+\b", RegexOptions.Compiled), "[redacted-jwt]"),
            (new Regex(@"(?i)(authorization:\s*bearer\s+)[^\s]+", RegexOptions.Compiled), "$1[redacted-token]"),
            (new Regex(@"(?i)(password\s*=\s*)[^;,\s]+", RegexOptions.Compiled), "$1[redacted-password]"),
            (new Regex(@"(?i)(api[_-]?key\s*[=:]\s*)[^\s;,""]+", RegexOptions.Compiled), "$1[redacted-api-key]")
        };

        regexPatterns.AddRange(options.Patterns
            .Where(pattern => !string.IsNullOrWhiteSpace(pattern.Regex))
            .Select(pattern => (new Regex(pattern.Regex, RegexOptions.Compiled), pattern.Replacement)));

        _regexPatterns = regexPatterns;
        _literalPatterns = options.Literals
            .Where(literal => !string.IsNullOrWhiteSpace(literal.Value))
            .Select(literal => (literal.Value, literal.Replacement))
            .ToArray();
    }

    public string Apply(string input)
    {
        var output = input;

        foreach (var (regex, replacement) in _regexPatterns)
        {
            output = regex.Replace(output, replacement);
        }

        foreach (var (value, replacement) in _literalPatterns)
        {
            output = output.Replace(value, replacement, StringComparison.Ordinal);
        }

        return output;
    }
}

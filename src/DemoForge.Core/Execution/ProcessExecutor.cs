using System.Diagnostics;

namespace DemoForge.Core.Execution;

public sealed class ProcessExecutor
{
    public async Task<CommandResult> ExecuteAsync(ProcessExecutionRequest request, CancellationToken cancellationToken)
    {
        var shell = ShellInfo.CreateDefault();
        var startedAtUtc = DateTimeOffset.UtcNow;
        var stopwatch = Stopwatch.StartNew();
        var output = new List<(bool isStdErr, string line, TimeSpan offset)>();

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = shell.FileName,
                Arguments = $"{shell.ArgumentsPrefix} \"{EscapeForShell(request.Command, shell)}\"",
                WorkingDirectory = request.WorkingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            EnableRaisingEvents = true
        };

        foreach (var pair in request.EnvironmentVariables)
        {
            process.StartInfo.Environment[pair.Key] = pair.Value;
        }

        process.OutputDataReceived += (_, args) =>
        {
            if (args.Data is not null)
            {
                lock (output)
                {
                    output.Add((false, args.Data, stopwatch.Elapsed));
                }
            }
        };

        process.ErrorDataReceived += (_, args) =>
        {
            if (args.Data is not null)
            {
                lock (output)
                {
                    output.Add((true, args.Data, stopwatch.Elapsed));
                }
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        using var timeoutCts = new CancellationTokenSource(request.Timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            await process.WaitForExitAsync(linkedCts.Token);
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !process.HasExited)
        {
            process.Kill(entireProcessTree: true);
            await process.WaitForExitAsync(CancellationToken.None);

            return new CommandResult
            {
                ExitCode = -1,
                TimedOut = true,
                StartedAtUtc = startedAtUtc,
                EndedAtUtc = DateTimeOffset.UtcNow,
                OutputLines = output.OrderBy(item => item.offset).ToArray(),
                ShellDisplayName = shell.DisplayName
            };
        }

        return new CommandResult
        {
            ExitCode = process.ExitCode,
            TimedOut = false,
            StartedAtUtc = startedAtUtc,
            EndedAtUtc = DateTimeOffset.UtcNow,
            OutputLines = output.OrderBy(item => item.offset).ToArray(),
            ShellDisplayName = shell.DisplayName
        };
    }

    private static string EscapeForShell(string command, ShellInfo shell)
    {
        return command.Replace(shell.QuoteEscapeTarget, shell.QuoteEscapeReplacement, StringComparison.Ordinal);
    }

    private sealed record ShellInfo(
        string FileName,
        string ArgumentsPrefix,
        string QuoteEscapeTarget,
        string QuoteEscapeReplacement,
        string DisplayName)
    {
        public static ShellInfo CreateDefault()
        {
            if (OperatingSystem.IsWindows())
            {
                return new ShellInfo("powershell", "-NoProfile -Command", "\"", "`\"", "powershell");
            }

            return new ShellInfo("/bin/bash", "-lc", "\"", "\\\"", "bash");
        }
    }
}

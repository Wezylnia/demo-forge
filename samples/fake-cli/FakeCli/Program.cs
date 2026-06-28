var command = args.FirstOrDefault() ?? "hello";

if (command is "--help" or "help")
{
    Console.WriteLine("FakeCli");
    Console.WriteLine("Usage: dotnet run --project samples/fake-cli/FakeCli -- [hello|fail|wait]");
    return;
}

switch (command)
{
    case "hello":
        Console.WriteLine("Hello from FakeCli");
        break;
    case "fail":
        Console.Error.WriteLine("Simulated failure");
        Environment.ExitCode = 2;
        break;
    case "wait":
        await Task.Delay(3000);
        Console.WriteLine("Finished waiting");
        break;
    default:
        Console.WriteLine($"Unknown command: {command}");
        Environment.ExitCode = 1;
        break;
}

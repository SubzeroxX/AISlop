

using System.Diagnostics;

namespace AISlop;

public class ExecuteTerminal : ITool
{
    public string Name => "executeterminal";
    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        return _ExecuteTerminal(
            args.GetValueOrDefault("command"),
            context.CurrentWorkingDirectory
            );
    }

    private Task<string> _ExecuteTerminal(string command, string cwd)
    {
        var processInfo = new ProcessStartInfo("cmd.exe", $"/c {command}")
        {
            WorkingDirectory = Path.Combine("./", cwd),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = false
        };

        using var process = Process.Start(processInfo);

        string output = process!.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (string.IsNullOrWhiteSpace(output.Trim()))
            output = "Command success!";

        return Task.FromResult(output + error);
    }
}

using System.Text;

namespace AISlop;

public class ListDirectory : ITool
{
    public string Name => "listdirectory";

    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        return _ListDirectory(
            context.CurrentWorkingDirectory);
    }

    private Task<string> _ListDirectory(string cwd)
    {
        if (!Directory.Exists(cwd))
            return Task.FromResult("Current CWD is empty");

        var sb = new StringBuilder();
        var option = SearchOption.TopDirectoryOnly;

        foreach (var dir in Directory.GetDirectories(cwd, "*", option))
            sb.AppendLine("[DIR] " + dir.Replace($"{cwd}\\", ""));

        foreach (var file in Directory.GetFiles(cwd, "*", option))
            sb.AppendLine("[FILE] " + file.Replace($"{cwd}\\", ""));

        return Task.FromResult($"Current directory: {cwd}\n" + sb.ToString());
    }
}

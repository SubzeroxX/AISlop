using System.Text;
using System.Text.RegularExpressions;

namespace AISlop;

public class WriteFile : ITool
{
    public string Name => "writefile";
    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        return _OverwriteFile(
            args.GetValueOrDefault("filename"),
            args.GetValueOrDefault("content"),
            context.CurrentWorkingDirectory
            );
    }

    private Task<string> _CreateFile(string filename, string content, string cwd)
    {
        string filePath = Path.Combine(cwd, filename);
        if (File.Exists(filePath))
            return Task.FromResult($"A file with that name already exists in the workspace: {filename}");

        using var file = File.Create(filePath);
        using StreamWriter sw = new(file, Encoding.UTF8);

        content = Regex.Unescape(content);
        sw.Write(content);

        return Task.FromResult($"File has been created: \"{filename}\" and content written into it");
    }

    public Task<string> _OverwriteFile(string filename, string text, string cwd)
    {
        string filePath = Path.Combine(cwd, filename);

        if (File.Exists(filePath))
            File.Delete(filePath);

        return _CreateFile(filename, text, cwd);
    }
}

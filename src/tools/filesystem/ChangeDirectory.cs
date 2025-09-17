namespace AISlop;

public class ChangeDirectory : ITool
{
    public string Name => "changedirectory";

    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        return _ChangeDirectory(args.GetValueOrDefault("dirname"), context);
    }

    private Task<string> _ChangeDirectory(string folderName, ToolExecutionContext cwd)
    {
        if (folderName == "/")
        {
            cwd.CurrentWorkingDirectory = "environment";
            return Task.FromResult($"Successfully changed to folder \"{cwd}\"");
        }

        if (cwd.CurrentWorkingDirectory.Contains(folderName))
            return Task.FromResult($"Already in a folder named \"{folderName}\"");

        string path = Path.Combine(cwd.CurrentWorkingDirectory, folderName);
        if (!Directory.Exists(path))
            return Task.FromResult($"Directory \"{folderName}\" does not exist");

        cwd.CurrentWorkingDirectory = path;
        return Task.FromResult($"Successfully changed to folder \"{folderName}\"");
    }
}

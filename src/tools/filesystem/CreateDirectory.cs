namespace AISlop;

public class CreateDirectory : ITool
{
    public string Name => "createdirectory";

    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        return _CreateDirectory(
            args.GetValueOrDefault("dirname"),
            context.CurrentWorkingDirectory);
    }

    private Task<string> _CreateDirectory(string name, string cwd)
    {
        string folder = Path.Combine(cwd, name);
        if (Directory.Exists(folder))
            return Task.FromResult($"Directory already exists with name: \"{name}\"");

        var output = Directory.CreateDirectory(folder);
        return Task.FromResult($"Directory created at: \"{folder}\".");
    }
}

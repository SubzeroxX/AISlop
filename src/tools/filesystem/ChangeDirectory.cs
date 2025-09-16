

namespace AISlop;

public class ChangeDirectory : ITool
{
    public string Name => "changedirectory";

    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        return Task.FromResult("");
    }
}

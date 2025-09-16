

namespace AISlop;

public class ListDirectory : ITool
{
    public string Name => "listdirectory";

    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        return Task.FromResult("");
    }
}

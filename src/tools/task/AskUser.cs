

namespace AISlop;

public class AskUser : ITool
{
    public string Name => "askuser";
    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        return Task.FromResult("");
    }
}

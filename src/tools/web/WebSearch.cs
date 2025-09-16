

namespace AISlop;

public class WebSearch : ITool
{
    public string Name => "websearch";
    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        return Task.FromResult("");
    }
}



namespace AISlop;

public class ExecuteTerminal : ITool
{
    public string Name => "executeterminal";
    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        return Task.FromResult("");
    }
}

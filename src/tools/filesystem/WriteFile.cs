

namespace AISlop;

public class WriteFile : ITool
{
    public string Name => "writefile";
    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        return Task.FromResult("");
    }
}

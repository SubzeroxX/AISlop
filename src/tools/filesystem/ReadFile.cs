

namespace AISlop;

public class ReadFile : ITool
{   
    public string Name => "readfile";
    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        return Task.FromResult("");
    }
}

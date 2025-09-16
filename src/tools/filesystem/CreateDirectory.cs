

namespace AISlop;

public class CreateDirectory : ITool
{
    public string Name => "createdirectory";

    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        return Task.FromResult("");
    }
}



namespace AISlop;

public class TaskDone : ITool
{
    public string Name => "taskdone";
    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        return Task.FromResult("");
    }
}

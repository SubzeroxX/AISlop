

namespace AISlop;

public class TaskDone : ITool
{
    public string Name => "taskdone";
    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        return Task.FromResult(_TaskDone(args.GetValueOrDefault("message")));
    }

    private string _TaskDone(string message)
    {
        Logging.DisplayAgentThought(message, ConsoleColor.Yellow);
        return "Task completion message displayed.";
    }
}

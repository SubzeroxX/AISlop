namespace AISlop;

public interface ITool
{
    string Name { get; }
    Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context);
}


namespace AISlop;

public class CreatePdfFile : ITool
{
    public string Name => "createpdffile";
    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        return Task.FromResult("");
    }
}

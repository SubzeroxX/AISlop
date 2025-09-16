

namespace AISlop;

public class ReadTextFromPDF : ITool
{
    public string Name => "readtextfrompdf";
    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        return Task.FromResult("");
    }
}

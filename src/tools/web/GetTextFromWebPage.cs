

namespace AISlop;

public class GetTextFromWebPage : ITool
{
    public string Name => "gettextfromwebpage";
    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        return Task.FromResult("");
    }
}

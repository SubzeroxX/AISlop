

namespace AISlop;

public class WebSearch : ITool
{
    public string Name => "websearch";
    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        return _WebSearch(
            args.GetValueOrDefault("query")
            );
    }

    private async Task<string> _WebSearch(string query)
    {
        return await WebScraper.Search(query);
    }
}

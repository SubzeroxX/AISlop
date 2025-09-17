namespace AISlop;

public class GetTextFromWebPage : ITool
{
    public string Name => "gettextfromwebpage";
    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        return _GetTextFromWebPage(
            args.GetValueOrDefault("url")
            );
    }

    private async Task<string> _GetTextFromWebPage(string url)
    {
        return await WebScraper.ScrapeTextFromUrlAsync(url);
    }
}

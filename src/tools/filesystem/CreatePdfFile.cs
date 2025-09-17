using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Markdown;
namespace AISlop;

public class CreatePdfFile : ITool
{
    public string Name => "createpdffile";
    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        return _CreatePdfFile(
            args.GetValueOrDefault("filename"),
            args.GetValueOrDefault("markdown_content"),
            context.CurrentWorkingDirectory
            );
    }

    private Task<string> _CreatePdfFile(string filename, string markdowntext, string cwd)
    {
        var path = Path.Combine(cwd, filename);
        if (File.Exists(path))
            return Task.FromResult($"File already exists with name {filename} in CWD");

        markdowntext = markdowntext.Replace("\\n", "\n").Replace("\\t", "\t");
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.PageColor(Colors.White);
                page.Margin(40);
                page.Content().Markdown(markdowntext);
            });
        });

        document.GeneratePdf(path);
        return Task.FromResult($"File has been created: \"{path}\" and content written into it");
    }
}

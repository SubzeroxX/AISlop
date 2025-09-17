

using System.Text;
using UglyToad.PdfPig;

namespace AISlop;

public class ReadFile : ITool
{   
    public string Name => "readfile";
    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        return _ReadFile(
            args.GetValueOrDefault("filename"),
            context.CurrentWorkingDirectory);
    }

    private Task<string> _ReadFile(string filename, string cwd)
    {
        if (filename.Contains(".pdf"))
            return _ReadTextFromPDF(filename, cwd);

        string filePath = Path.Combine(cwd, filename);
        if (!File.Exists(filePath))
            return Task.FromResult($"The file does not exists: \"{filePath}\"");

        var file = File.OpenRead(filePath);
        using StreamReader sr = new(file);

        return Task.FromResult($"{filename} content:\n```\n" + sr.ReadToEnd().ToString() + "\n```");
    }

    private Task<string> _ReadTextFromPDF(string filename, string cwd)
    {
        var filePath = filename;
        if (!File.Exists(filePath))
            return Task.FromResult($"File \"{filename}\" does not exist.");

        using var document = PdfDocument.Open(filePath);
        StringBuilder sb = new();
        foreach (var page in document.GetPages())
        {
            double? lastY = null;
            foreach (var word in page.GetWords())
            {
                var y = word.BoundingBox.Top;
                if (lastY != null && Math.Abs(lastY.Value - y) > 5)
                    sb.AppendLine();

                sb.Append($"{word.Text} ");
                lastY = y;
            }
        }

        return Task.FromResult($"PDF file content:\n{sb.ToString()}");
    }
}

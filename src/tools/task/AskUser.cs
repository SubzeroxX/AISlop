

namespace AISlop;

public class AskUser : ITool
{
    public string Name => "askuser";
    public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
    {
        return Task.Run(() => _AskUser(args.GetValueOrDefault("question")));
    }

    private string _AskUser(string message)
    {
        // Your original blocking method
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[Agent Asks]: {message}");
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write("Response: ");
        Console.ResetColor();
        return Console.ReadLine()!;
    }
}

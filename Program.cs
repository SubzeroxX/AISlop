using AISlop;
using System.Reflection;
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
Config.LoadConfig();

Console.Write("Task: ");
string taskString = Console.ReadLine()!; Console.WriteLine();

StreamWriter sw = null!;
if (Config.Settings.generate_log)
{
    var fileName = $"{DateTime.Now:yyyy-MM-dd_HH-mm}-log.txt";
    sw = new StreamWriter(fileName, append: true) { AutoFlush = true };
    Console.SetOut(new MultiTextWriter(Console.Out, sw));
}

int flags = 0;
flags |= (Config.Settings.display_thought ? (int)ProcessingState.StreamingThought : 0);
flags |= (Config.Settings.display_toolcall ? (int)ProcessingState.StreamingToolCalls : 0);

var tools = Assembly.GetExecutingAssembly()
    .GetTypes()
    .Where(t => !t.IsAbstract && !t.IsInterface && typeof(ITool).IsAssignableFrom(t))
    .Select(t => (ITool)Activator.CreateInstance(t)!)
    .ToList();

var agentHandler = new AgentHandler(tools, new AIWrapper(Config.Settings.model_name, flags));
await agentHandler.RunAsync(taskString);

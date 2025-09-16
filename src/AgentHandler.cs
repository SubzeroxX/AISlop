using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;

namespace AISlop
{
    public class AgentHandler
    {
        private readonly AIWrapper _agent;
        private string _cwd = "environment";
        private bool _agentRunning = true;
        private readonly Dictionary<string, ITool> _tools;

        /// <summary>
        /// Initializes the Tools, Agent, and a ToolHandler for this instance
        /// </summary>
        /// <param name="modelName">Ollama model name</param>
        public AgentHandler(IEnumerable<ITool> availableTools, AIWrapper wrapper)
        {
            _agent = wrapper;
            _tools = availableTools.ToDictionary(t => t.Name.ToLowerInvariant());
        }
        /// <summary>
        /// Main function of the agent. Handles the recursion
        /// </summary>
        /// <param name="initialTask">Task string, what the agent should do</param>
        /// <exception cref="ArgumentNullException">Invalid task was given</exception>
        public async Task RunAsync(string initialTask)
        {
            if (string.IsNullOrWhiteSpace(initialTask))
                throw new ArgumentNullException("Task was an empty string!");

            Logging.DisplayAgentThought(ConsoleColor.Green);
            var agentResponse = await _agent.AskAi($"{initialTask}\nCurrent cwd: \"{_cwd}\"");

            while (_agentRunning)
                agentResponse = await HandleAgentResponse(agentResponse);
        }
        /// <summary>
        /// Handles the agents response and task phases
        /// </summary>
        /// <param name="rawResponse">Agents response (raw response)</param>
        /// <returns>API Responses</returns>
        private async Task<string> HandleAgentResponse(string rawResponse)
        {
            var parsedToolCalls = Parser.Parse(rawResponse);
            if (parsedToolCalls.Count() == 1 && !string.IsNullOrWhiteSpace(parsedToolCalls.First().Error))
                return await HandleInvalidToolcall(parsedToolCalls.First().Error);

            string toolCallOutputs = await ExecuteTool(parsedToolCalls);

            if (!string.IsNullOrEmpty(toolCallOutputs))
                Logging.DisplayToolCallUsage(toolCallOutputs);

            if (!_agentRunning)
                return await HandleTaskCompletion(toolCallOutputs);
            else
                return await ContinueAgent(toolCallOutputs);
        }
        /// <summary>
        /// Gives back the toolcall exception message to the Agent
        /// </summary>
        /// <param name="toolException">Toolcall output</param>
        /// <returns>agents response</returns>
        private async Task<string> HandleInvalidToolcall(string toolException)
        {
            Logging.DisplayToolCallUsage(toolException);
            Logging.DisplayAgentThought(ConsoleColor.Green);
            return await _agent.AskAi($"Tool result: {toolException}\nCurrent cwd: \"{_cwd}\"");
        }
        /// <summary>
        /// Executes tools in order
        /// </summary>
        /// <param name="toolcalls">Tools to execute</param>
        /// <returns>Tool outputs</returns>
        private async Task<string> ExecuteTool(IEnumerable<Parser.Command> toolcalls)
        {
            StringBuilder sb = new();
            string currentToolName = "";
            try
            {
                foreach (var singleCall in toolcalls)
                {
                    currentToolName = singleCall.Tool;
                    if (_tools.TryGetValue(singleCall.Tool.ToLowerInvariant(), out var tool))
                    {
                        var context = new ToolExecutionContext { CurrentWorkingDirectory = _cwd };
                        string result = await tool.ExecuteAsync(singleCall.Args, context);
                        _cwd = context.CurrentWorkingDirectory; // Update CWD if changed by the tool
                        sb.AppendLine($"{singleCall.Tool} output: {result}");
                    }
                    else
                    {
                        sb.AppendLine($"{singleCall.Tool} error: Tool not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"An exception occurred during {currentToolName} execution: {ex.Message}");
            }

            return sb.ToString();
        }
        /// <summary>
        /// Task ended handle. `end` ends the current chat
        /// New prompt will launch a follow up to the task it was doing before.
        /// </summary>
        /// <param name="completeMessage">Agent completition message</param>
        /// <returns>Agent response</returns>
        /// <exception cref="ArgumentNullException">No task was given</exception>
        private async Task<string> HandleTaskCompletion(string completeMessage)
        {
            Console.Beep();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("New task: (type \"end\" to end the process)");
            string newTask = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(newTask))
                throw new ArgumentNullException("Task was an empty string!");
            if (newTask.ToLower() == "end")
                return completeMessage;

            Console.WriteLine();
            _agentRunning = true;
            Logging.DisplayAgentThought(ConsoleColor.Green);
            return await _agent.AskAi($"User followup question/task: {newTask}\nCurrent cwd: \"{_cwd}\"");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toolOutput"></param>
        /// <returns></returns>
        private async Task<string> ContinueAgent(string toolOutput)
        {
            Logging.DisplayAgentThought(ConsoleColor.Green);
            return await _agent.AskAi(
                $"Tool result: \"{toolOutput}\"\nCurrent cwd: \"{_cwd}\""
            );
        }
    }
}

using LlmTornado;
using LlmTornado.Chat;
using LlmTornado.Chat.Models;
using System.Text;

namespace AISlop
{
    public class AIWrapper
    {
        /// <summary>
        /// Api support: https://github.com/lofcz/LlmTornado
        /// Note: this is a private VPN IP change this to either "localhost" or your LLM server
        /// </summary>
        TornadoApi api = new(new Uri(Config.Settings.ollama_url)); // default Ollama port, API key can be passed in the second argument if needed
        Conversation _conversation = null!;
        int _streamingStates;
        public AIWrapper(string model, int streamingState)
        {
            _conversation = api.Chat.CreateConversation(new ChatModel(model));
            _conversation.AddSystemMessage(GetInstruction("SlopInstruction"));
            _streamingStates = streamingState;
        }
        public async Task<string> AskAi(string message)
        {
            var responseBuilder = new StringBuilder();
            bool toolcallStarted = false;
            bool thoughtDone = false;

            string newLineBuffer = "";
            string lastPrintedThought = "";
            int thoughtValueStartIndex = -1;
            const string thoughtKey = "\"thought\": \"";
            const string thoughtTerminator = "\",";

            await _conversation.AppendUserInput(message)
                .StreamResponse(chunk =>
                {
                    responseBuilder.Append(chunk);
                    string currentFullResponse = responseBuilder.ToString();

                    if ((_streamingStates & (int)ProcessingState.StreamingThought) != 0 && !toolcallStarted && !thoughtDone)
                    {
                        if (thoughtValueStartIndex == -1)
                        {
                            int keyIndex = currentFullResponse.IndexOf(thoughtKey);
                            if (keyIndex != -1)
                                thoughtValueStartIndex = keyIndex + thoughtKey.Length;
                        }

                        if (thoughtValueStartIndex != -1)
                        {
                            string potentialContent = currentFullResponse.Substring(thoughtValueStartIndex);
                            string currentThoughtValue;

                            int endMarkerIndex = potentialContent.IndexOf(thoughtTerminator);

                            if (endMarkerIndex != -1)
                            {
                                currentThoughtValue = potentialContent.Substring(0, endMarkerIndex);
                                toolcallStarted = thoughtDone = true;
                                Console.WriteLine(Environment.NewLine);
                            }
                            else
                            {
                                currentThoughtValue = potentialContent;
                            }

                            if (currentThoughtValue.Length > lastPrintedThought.Length && currentThoughtValue.StartsWith(lastPrintedThought))
                            {
                                string newContent = currentThoughtValue.Substring(lastPrintedThought.Length);
                                if (newContent.Contains("\\"))
                                {
                                    newLineBuffer = newContent;
                                }
                                else
                                {
                                    if (newLineBuffer != "")
                                    {
                                        Console.Write($"{(newLineBuffer + newContent).Replace("\\n", "")}{Environment.NewLine}");
                                        newLineBuffer = "";
                                    }
                                    else
                                        Console.Write(newContent);
                                }
                                Console.Out.Flush();
                            }

                            lastPrintedThought = currentThoughtValue;
                        }

                    }
                    else if ((_streamingStates & (int)ProcessingState.StreamingToolCalls) != 0 && toolcallStarted)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        if (currentFullResponse.Contains("["))
                            if (currentFullResponse.Count(x => x == '[') == currentFullResponse.Count(x => x == ']'))
                                toolcallStarted = false;
                        Console.Write(chunk);
                    }
                });

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            return responseBuilder.ToString();
        }

        private string GetInstruction(string instructName)
        {
            string solutionDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
            string instructFolder = Path.Combine(solutionDir, "instructions");

            if (!Directory.Exists(instructFolder))
                throw new DirectoryNotFoundException("Instructions folder not found.");

            return File.ReadAllText(Path.Combine(instructFolder, $"{instructName}.md"));
        }
    }
}

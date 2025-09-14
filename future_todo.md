# Important Refactoring Plan

This document outlines the key architectural improvements needed to elevate the project to a more robust, maintainable, and testable state. These changes are crucial for future development, especially for the planned Blazor UI integration and database connectivity.

The current implementation is a solid proof-of-concept, but to move towards a production-quality application, we need to address several core architectural patterns.

## 1. Implement Dependency Injection (DI) and Inversion of Control (IoC)

**Problem:** Classes like `AgentHandler` are currently creating their own dependencies (e.g., `_tools = new Tools(...)`). This pattern, known as tight coupling, makes the code rigid and difficult to test in isolation.

**Solution:** Refactor the application to use a Dependency Injection container. Dependencies should be "injected" into classes through their constructors instead of being created internally.

**Action Items:**
-   Set up a DI container in `Program.cs` (e.g., using `Microsoft.Extensions.DependencyInjection`).
-   Register services like `AgentHandler`, `AIWrapper`, and the new tool services (see point 2) with the container.
-   Modify constructors to accept dependencies as parameters (interfaces).

**Benefit:** This is the foundational step for unit testing and building a flexible, modular architecture required for the Blazor front-end.

## 2. Decompose the `Tools.cs` Monolith into a Modular, Interface-Based System

**Problem:** The `Tools.cs` class is becoming a "God Class" that handles file system operations, web scraping, terminal execution, and more. As new tools are added, this class will become increasingly large and difficult to maintain.

**Solution:** Break down tools into small, single-responsibility classes that implement a common `ITool` interface. The `AgentHandler` will then work with a collection of `ITool` instances, completely decoupled from the specific implementations.

**Proposed Architecture:**

1.  **Define a common tool interface:**
    ```csharp
    // in ITool.cs
    public interface ITool
    {
        string Name { get; }
        string Description { get; } // Useful for the AI prompt
        Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context);
    }
    
    // Context object to pass state like CWD without using `ref`
    public class ToolExecutionContext
    {
        public string CurrentWorkingDirectory { get; set; }
    }
    ```

2.  **Create specific tool classes:**
    ```csharp
    // in Tools/FileSystem/CreateFileTool.cs
    public class CreateFileTool : ITool
    {
        public string Name => "createfile";
        public string Description => "Creates a new file with the specified content.";
    
        public Task<string> ExecuteAsync(Dictionary<string, string> args, ToolExecutionContext context)
        {
            var filename = args.GetValueOrDefault("filename");
            var content = args.GetValueOrDefault("content");
            var cwd = context.CurrentWorkingDirectory;
            // ... implementation logic ...
            return Task.FromResult("File created successfully.");
        }
    }
    ```

3.  **Refactor `AgentHandler` to consume a collection of `ITool`:**
    ```csharp
    // in AgentHandler.cs
    public class AgentHandler
    {
        private readonly Dictionary<string, ITool> _tools;

        // Dependencies are injected by the DI container
        public AgentHandler(IEnumerable<ITool> availableTools, /*... other services ...*/)
        {
            _tools = availableTools.ToDictionary(t => t.Name.ToLowerInvariant());
        }

        private async Task<string> ExecuteTool(Parser.Command singleCall)
        {
            if (_tools.TryGetValue(singleCall.Tool.ToLowerInvariant(), out var tool))
            {
                var context = new ToolExecutionContext { CurrentWorkingDirectory = _cwd };
                string result = await tool.ExecuteAsync(singleCall.Args, context);
                _cwd = context.CurrentWorkingDirectory; // Update CWD if changed by the tool
                return result;
            }
            return $"Error: Tool '{singleCall.Tool}' not found.";
        }
    }
    ```

**Benefit:** This approach follows the **Open/Closed Principle**. Adding a new tool only requires creating a new class, not modifying `AgentHandler`. It also makes each tool individually testable.

## 3. Enhance Unit Testing with Mocking

**Problem:** The current tight coupling makes it nearly impossible to write effective unit tests for business logic without hitting the file system or making real network calls.

**Solution:** With DI and interfaces in place, we can use a mocking framework (like `Moq` or `NSubstitute`) to test components in isolation.

**Action Items:**
-   Add a unit testing project to the solution if one doesn't exist.
-   Add a mocking library (e.g., Moq) as a dependency.
-   Write unit tests for individual `ITool` implementations.
-   Write unit tests for `AgentHandler`'s logic by providing it with mock `ITool` objects. Verify that the correct tools are called with the correct parameters based on parsed AI responses.

**Benefit:** Creates a safety net against regressions, validates logic, and enforces good design. This is a non-negotiable practice for a mid-level developer.

## 4. Externalize the System Prompt

**Problem:** The large system prompt is hardcoded as a string literal inside `AIWrapper.cs`. This makes it difficult to tweak and maintain without recompiling the application.

**Solution:** Move the prompt into an external `.md` or `.txt` file.

**Action Items:**
-   Create a `Prompts/SystemPrompt.md` file in the project.
-   Set its "Copy to Output Directory" property to "Copy if newer".
-   Modify `AIWrapper.cs` to read the prompt from this file at runtime.

**Benefit:** Allows for rapid iteration on the prompt, which is a core part of the agent's behavior. The prompt can be version-controlled independently of the C# code.

## Summary of Priority

1.  **Implement DI and the `ITool` interface architecture.** This is the highest priority as it unblocks all other improvements.
2.  **Write comprehensive Unit Tests** for the new tool classes and the refactored `AgentHandler`.
3.  **Externalize the System Prompt** for easier maintenance.

Executing this plan will transform the project from a promising prototype into a scalable and professional-grade application.
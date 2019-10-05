using CodeRunner.Loggings;
using CodeRunner.Managements;
using CodeRunner.Managements.Configurations;
using CodeRunner.Operations;
using CodeRunner.Pipelines;
using CodeRunner.Templates;
using System;
using System.Threading.Tasks;

namespace CodeRunner.Test.Mocks
{
    public class TestWorkspace : IWorkspace
    {
        public TestWorkspace(LoggerScope scope,
                             ITemplateManager? templateManager = null,
                             IOperationManager? operationManager = null,
                             WorkspaceSettings? settings = null,
                             Func<Task>? onInitialize = null,
                             Func<Task>? onClear = null,
                             Func<string, ITemplate?, Func<VariableCollection, ResolveContext, Task>, Task<IWorkItem?>>? onCreate = null,
                             Func<IWorkItem?, IOperation, Func<VariableCollection, ResolveContext, Task>, OperationWatcher, ILogger, Task<PipelineResult<Wrapper<bool>>>>? onExecute = null)
        {
            Templates = templateManager ?? new InMemoryTemplateManager();
            Operations = operationManager ?? new InMemoryOperationManager();
            Settings = Task.FromResult(settings);
            OnCreate = onCreate;
            OnClear = onClear;
            OnInitialize = onInitialize;
            OnExecute = onExecute;
            LogScope = scope;
        }

        private LoggerScope LogScope { get; set; }

        public ITemplateManager Templates { get; }

        public IOperationManager Operations { get; }

        public Task<WorkspaceSettings?> Settings { get; }

        private Func<string, ITemplate?, Func<VariableCollection, ResolveContext, Task>, Task<IWorkItem?>>? OnCreate { get; }

        private Func<Task>? OnClear { get; }

        private Func<Task>? OnInitialize { get; }

        private Func<IWorkItem?, IOperation, Func<VariableCollection, ResolveContext, Task>, OperationWatcher, ILogger, Task<PipelineResult<Wrapper<bool>>>>? OnExecute { get; }

        public virtual async Task Clear()
        {
            LogScope.Information("Invoke");
            if (OnClear != null)
                await OnClear();
        }

        public virtual async Task<IWorkItem?> Create(string name, ITemplate? from, Func<VariableCollection, ResolveContext, Task> resolveCallback)
        {
            LogScope.Information("Invoke");
            return OnCreate != null ? await OnCreate(name, from, resolveCallback) : null;
        }

        public virtual async Task<PipelineResult<Wrapper<bool>>> Execute(IWorkItem? workItem, IOperation from, Func<VariableCollection, ResolveContext, Task> resolveCallback, OperationWatcher watcher, ILogger logger)
        {
            LogScope.Information("Invoke");
            return OnExecute != null ? await OnExecute(workItem, from, resolveCallback, watcher, logger) : new PipelineResult<Wrapper<bool>>(false, null, Array.Empty<LogItem>());
        }

        public virtual async Task Initialize()
        {
            LogScope.Information("Invoke");
            if (OnInitialize != null)
                await OnInitialize();
        }
    }
}

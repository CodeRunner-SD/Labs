using CodeRunner.Managements;
using CodeRunner.Managements.Configurations;
using CodeRunner.Operations;
using CodeRunner.Packaging;
using CodeRunner.Templates;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeRunner.Test.Mocks
{
    public class InMemoryTemplateManager : InMemoryItemManager<TemplateSettings, Package<ITemplate>>, ITemplateManager { }

    public class InMemoryOperationManager : InMemoryItemManager<OperationSettings, Package<IOperation>>, IOperationManager { }

    public class InMemoryItemManager<TSettings, TItem> : IItemManager<TSettings, TItem> where TItem : class where TSettings : class
    {
        public InMemoryItemManager(TSettings? settings = null) => Settings = Task.FromResult(settings);

        public Task<TSettings?> Settings { get; }

        private Dictionary<string, TItem> Inner { get; } = new Dictionary<string, TItem>();

        public Task Clear()
        {
            Inner.Clear();
            return Task.CompletedTask;
        }

        public async IAsyncEnumerable<string> GetKeys()
        {
            foreach (string item in Inner.Keys)
                yield return await Task.FromResult(item);
        }

        public Task<TItem?> GetValue(string id) => Task.FromResult<TItem?>(Inner.TryGetValue(id, out TItem? value) ? value : null);

        public async IAsyncEnumerable<TItem?> GetValues()
        {
            foreach (TItem item in Inner.Values)
                yield return await Task.FromResult(item);
        }

        public Task<bool> HasKey(string id) => Task.FromResult(Inner.ContainsKey(id));

        public Task Initialize() => Task.CompletedTask;

        public Task SetValue(string id, TItem? value)
        {
            if (value == null)
                _ = Inner.Remove(id);
            else if (Inner.ContainsKey(id))
                Inner[id] = value;
            else Inner.Add(id, value);
            return Task.CompletedTask;
        }
    }
}

using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace GemsAi.Core.Memory
{
    public class InMemoryMemoryStore : IMemoryStore
    {
        private readonly ConcurrentDictionary<string, string> _store = new();

        public Task StoreAsync(string key, string value)
        {
            _store[key] = value;
            return Task.CompletedTask;
        }

        public Task<string?> RetrieveAsync(string key)
        {
            _store.TryGetValue(key, out var value);
            return Task.FromResult(value);
        }
    }
}
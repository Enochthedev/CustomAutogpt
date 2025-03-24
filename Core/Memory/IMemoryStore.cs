using System.Threading.Tasks;

namespace GemsAi.Core.Memory
{
    public interface IMemoryStore
    {
        Task StoreAsync(string key, string value);
        Task<string?> RetrieveAsync(string key);
    }
}
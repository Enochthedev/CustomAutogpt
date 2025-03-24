using System.Collections.Generic;
using System.Threading.Tasks;

namespace GemsAi.Core.Memory
{
    public interface IVectorMemoryStore
    {
        Task StoreAsync(string key, string text, float[] embedding);
        Task<(string key, float[] embedding, string text)?> RetrieveClosestAsync(string query, float threshold = 0.8f);
    }
}
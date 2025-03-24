using System.Threading.Tasks;

namespace GemsAi.Core.Memory
{
    public interface IEmbedder
    {
        Task<float[]> GetEmbeddingAsync(string text);
    }
}
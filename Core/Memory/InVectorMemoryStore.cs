using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GemsAi.Core.Memory;

namespace GemsAi.Core.Memory
{
    public class InMemoryVectorMemoryStore : IVectorMemoryStore
    {
        private readonly List<MemoryEntry> _entries = new List<MemoryEntry>();
        private readonly IEmbedder _embedder;

        public InMemoryVectorMemoryStore(IEmbedder embedder)
        {
            _embedder = embedder;
        }

        public async Task StoreAsync(string key, string text, float[] embedding)
        {
            _entries.Add(new MemoryEntry { Key = key, Text = text, Embedding = embedding });
            await Task.CompletedTask;
        }

        public async Task<(string key, float[] embedding, string text)?> RetrieveClosestAsync(string query, float threshold = 0.8f)
        {
            var queryEmbedding = await _embedder.GetEmbeddingAsync(query);
            (string key, float[] embedding, string text)? bestMatch = null;
            float bestScore = threshold;

            foreach (var entry in _entries)
            {
                var similarity = CosineSimilarity(queryEmbedding, entry.Embedding);
                if (similarity > bestScore)
                {
                    bestScore = similarity;
                    bestMatch = (entry.Key, entry.Embedding, entry.Text);
                }
            }
            return bestMatch;
        }

        private float CosineSimilarity(float[] vectorA, float[] vectorB)
        {
            if (vectorA.Length != vectorB.Length)
                throw new ArgumentException("Vectors must be the same length");

            float dot = 0, normA = 0, normB = 0;
            for (int i = 0; i < vectorA.Length; i++)
            {
                dot += vectorA[i] * vectorB[i];
                normA += vectorA[i] * vectorA[i];
                normB += vectorB[i] * vectorB[i];
            }
            return dot / ((float)Math.Sqrt(normA) * (float)Math.Sqrt(normB) + 1e-6f);
        }

        private class MemoryEntry
        {
            public string Key { get; set; } = "";
            public string Text { get; set; } = "";
            public float[] Embedding { get; set; } = Array.Empty<float>();
        }
    }
}
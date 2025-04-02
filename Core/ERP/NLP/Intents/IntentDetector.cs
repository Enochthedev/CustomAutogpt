using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using GemsAi.Core.Memory.Embedders;

namespace GemsAi.Core.ERP.NLP.Intents
{
    public class IntentDetector
    {
        private readonly List<IntentData> _intents;
        private readonly Dictionary<string, float[]> _intentEmbeddings;
        private readonly TextEmbedder _embedder;

        public IntentDetector()
        {
            _embedder = new TextEmbedder();
            _intents = LoadIntents();
            _intentEmbeddings = PrecomputeEmbeddings(_intents);
        }

        private List<IntentData> LoadIntents()
        {
            try
            {
                string filePath = Path.Combine("Core", "ERP", "NLP", "Intents", "IntentData.json");
                string jsonData = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<IntentData>>(jsonData) ?? new List<IntentData>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading intents: {ex.Message}");
                return new List<IntentData>();
            }
        }

        private Dictionary<string, float[]> PrecomputeEmbeddings(List<IntentData> intents)
        {
            var embeddings = new Dictionary<string, float[]>();
            foreach (var intent in intents)
            {
                var embedding = _embedder.Embed(intent.Text);
                if (embedding.Length > 0)
                {
                    if (!string.IsNullOrEmpty(intent.Label))
                    {
                        embeddings[intent.Label] = embedding;
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Intent with text '{intent.Text}' has a null or empty label and will be skipped.");
                    }
                }
                else
                {
                    Console.WriteLine($"Warning: Failed to generate embedding for intent '{intent.Text}'");
                }
            }
            return embeddings;
        }

        public string Detect(string input)
        {
            // Direct match first
            foreach (var intent in _intents)
            {
                if (input.Contains(intent.Text, StringComparison.OrdinalIgnoreCase))
                {
                    return intent.Label ?? "Unknown";
                }
            }

            // Fallback to embedding-based similarity
            string closestIntent = FindClosestIntent(input);
            return closestIntent ?? "Unknown";
        }

        private string FindClosestIntent(string input)
        {
            var inputEmbedding = _embedder.Embed(input);

            string? bestMatch = null;
            double highestSimilarity = 0.0;

            foreach (var intent in _intentEmbeddings)
            {
                double similarity = CosineSimilarity(inputEmbedding, intent.Value);
                if (similarity > highestSimilarity)
                {
                    highestSimilarity = similarity;
                    bestMatch = intent.Key;
                }
            }

            return bestMatch ?? "Unknown";
        }

        private double CosineSimilarity(float[] vec1, float[] vec2)
        {
            double dot = 0.0, mag1 = 0.0, mag2 = 0.0;
            for (int i = 0; i < Math.Min(vec1.Length, vec2.Length); i++)
            {
                dot += vec1[i] * vec2[i];
                mag1 += Math.Pow(vec1[i], 2);
                mag2 += Math.Pow(vec2[i], 2);
            }
            return dot / (Math.Sqrt(mag1) * Math.Sqrt(mag2));
        }

        public void LearnNewIntent(string text, string label)
        {
            var newIntent = new IntentData { Text = text, Label = label };
            _intents.Add(newIntent);

            // Precompute embedding for the new intent
            var embedding = _embedder.Embed(text);
            if (embedding.Length > 0)
            {
                _intentEmbeddings[label] = embedding;
                SaveIntents();
                Console.WriteLine($"New intent '{label}' learned successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to learn intent '{label}' due to embedding error.");
            }
        }

        private void SaveIntents()
        {
            try
            {
                string filePath = Path.Combine("Core", "ERP", "NLP", "Intents", "IntentData.json");
                string jsonData = JsonConvert.SerializeObject(_intents, Formatting.Indented);
                File.WriteAllText(filePath, jsonData);
                Console.WriteLine("Intents saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving intents: {ex.Message}");
            }
        }
    }
}
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GemsAi.Core.Memory.Embedders
{
    public class TextEmbedder
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;

        public TextEmbedder()
        {
            _mlContext = new MLContext();

            // Create a pipeline that featurizes text into a vector
            var pipeline = _mlContext.Transforms.Text
                .FeaturizeText(outputColumnName: "Features", inputColumnName: "Text");

            // Fit the model using an empty dataset (no training needed for TF-IDF)
            var emptyData = _mlContext.Data.LoadFromEnumerable(new List<TextData>());
            _model = pipeline.Fit(emptyData);
        }

        public float[] Embed(string text)
        {
            try
            {
                var data = _mlContext.Data.LoadFromEnumerable(new[] { new TextData { Text = text } });
                var transformedData = _model.Transform(data);

                // Use VBuffer to handle dynamically sized vectors
                var embedding = _mlContext.Data
                    .CreateEnumerable<EmbeddingData>(transformedData, reuseRowObject: false)
                    .FirstOrDefault();

                if (embedding != null && embedding.Features.IsDense)
                {
                    var denseVector = embedding.Features.GetValues().ToArray();
                    Console.WriteLine($"✅ Embedding created. Vector length: {denseVector.Length}");
                    return denseVector;
                }
                else
                {
                    Console.WriteLine("⚠️ Embedding failed or is empty.");
                    return Array.Empty<float>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🛑 Error during embedding: {ex.Message}");
                return Array.Empty<float>();
            }
        }

        private class TextData
        {
            public string? Text { get; set; }
        }

        private class EmbeddingData
        {
            public VBuffer<float> Features { get; set; }
        }
    }
}
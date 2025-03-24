using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Onnx;

namespace GemsAi.Core.Memory
{
    public class MLNetEmbedder : IEmbedder
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;
        private readonly PredictionEngine<EmbeddingInput, EmbeddingOutput> _predictionEngine;

        public MLNetEmbedder(string modelPath)
        {
            _mlContext = new MLContext();
            // Load a dummy data view for fitting the pipeline.
            var emptyData = _mlContext.Data.LoadFromEnumerable(new List<EmbeddingInput>());
            var pipeline = _mlContext.Transforms.ApplyOnnxModel(modelFile: modelPath);
            _model = pipeline.Fit(emptyData);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<EmbeddingInput, EmbeddingOutput>(_model);
        }

        public Task<float[]> GetEmbeddingAsync(string text)
        {
            var input = new EmbeddingInput { Text = text };
            var result = _predictionEngine.Predict(input);
            return Task.FromResult(result.Embedding);
        }
    }

    public class EmbeddingInput
    {
        public string Text { get; set; } = "";
    }

    public class EmbeddingOutput
    {
        // Adjust the dimensions to match your model's output (e.g., 128, 512, etc.)
        [VectorType(128)]
        public float[] Embedding { get; set; } = new float[128];
    }
}
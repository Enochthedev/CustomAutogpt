using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace Core.InProg.NLP.POS
{
    public class PosTagger
    {
        private readonly InferenceSession _session;
        private readonly BertTokenizer _tokenizer;
        private readonly Dictionary<int, string> _labelMap = new()
        {
            [0] = "O",
            [1] = "B-MISC",
            [2] = "I-MISC",
            [3] = "B-PER",
            [4] = "I-PER",
            [5] = "B-ORG",
            [6] = "I-ORG",
            [7] = "B-LOC",
            [8] = "I-LOC"
        };

        public PosTagger(string modelPath = "Assets/Models/POS/bert-pos.onnx", string vocabPath = "Assets/Models/POS/vocab.txt")
        {
            string baseDir = AppContext.BaseDirectory;
            string fullModelPath = Path.Combine(baseDir, modelPath);
            string fullVocabPath = Path.Combine(baseDir, vocabPath);

            _session = new InferenceSession(fullModelPath);
            _tokenizer = new BertTokenizer(fullVocabPath);
        }

        public List<(string Word, string Tag)> Predict(string sentence)
        {
            var tokenIds = _tokenizer.TokenizeToIds(sentence);        // includes [CLS] and [SEP]
            var tokens = _tokenizer.BasicTokenize(sentence);          // raw word tokens without special tokens

            int sequenceLength = tokenIds.Count;
            var inputIdsTensor = new DenseTensor<long>(new[] { 1, sequenceLength });
            var attentionTensor = new DenseTensor<long>(new[] { 1, sequenceLength });

            for (int i = 0; i < sequenceLength; i++)
            {
                inputIdsTensor[0, i] = tokenIds[i];
                attentionTensor[0, i] = 1;
            }

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input_ids", inputIdsTensor),
                NamedOnnxValue.CreateFromTensor("attention_mask", attentionTensor),
            };

            using var results = _session.Run(inputs);
            var logits = results.First().AsTensor<float>(); // [1, seq_len, num_labels]

            var predictedTags = new List<string>();
            int seqLen = logits.Dimensions[1];
            int numTags = logits.Dimensions[2];

            for (int i = 0; i < seqLen; i++)
            {
                float max = float.MinValue;
                int maxIndex = -1;

                for (int j = 0; j < numTags; j++)
                {
                    var score = logits[0, i, j];
                    if (score > max)
                    {
                        max = score;
                        maxIndex = j;
                    }
                }

                _labelMap.TryGetValue(maxIndex, out string? tag);
                predictedTags.Add(tag ?? "O");
            }

            // 🔍 Skip first [CLS] and last [SEP] if needed
            var trimmedTags = predictedTags.Skip(1).Take(tokens.Count).ToList();

            return tokens.Zip(trimmedTags, (word, tag) => (word, tag)).ToList();
        }
    }
}
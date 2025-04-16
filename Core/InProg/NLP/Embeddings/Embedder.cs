namespace Core.InProg.NLP
{
    public class Embedder
    {
        public float[] Run(string input)
        {
            return Enumerable.Range(0, 10).Select(i => (float)(i * 0.1)).ToArray();
        }
    }
}
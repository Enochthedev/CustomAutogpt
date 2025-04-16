namespace Core.InProg.NLP
{
    public class SentimentAnalyzer
    {
        public string Run(string input)
        {
            if (input.Contains("love") || input.Contains("great"))
                return "Positive";
            if (input.Contains("hate") || input.Contains("terrible"))
                return "Negative";
            return "Neutral";
        }
    }
}
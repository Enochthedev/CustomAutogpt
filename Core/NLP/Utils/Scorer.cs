using System.Text.RegularExpressions;

namespace GemsAI.Core.NLP.Utils
{
    public static class Scorer
    {
        public static int ScoreConfidence(string input, string pattern)
        {
            int score = 0;
            if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
                score += 50;

            if (input.Split(' ').Length < 5)
                score += 20;

            return score;
        }
    }
}
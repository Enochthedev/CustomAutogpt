using System.Text.RegularExpressions;

namespace GemsAI.Core.NLP.Utils
{
    public static class TextCleaner
    {
        public static string Clean(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            // Remove punctuation and extra spaces
            input = Regex.Replace(input, @"[^\w\s]", "");
            input = Regex.Replace(input, @"\s+", " ");
            return input.Trim();
        }
    }
}
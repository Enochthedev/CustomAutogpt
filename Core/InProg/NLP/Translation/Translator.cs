namespace Core.InProg.NLP
{
    public class Translator
    {
        public string Run(string input, string targetLanguage = "fr")
        {
            return $"[Translated to {targetLanguage}]: Ceci est une phrase exemple.";
        }
    }
}
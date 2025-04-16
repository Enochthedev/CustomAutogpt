namespace Core.InProg.NLP
{
    public class DependencyParser
    {
        public List<(string Word, string Head, string Relation)> Run(string sentence)
        {
            return new List<(string, string, string)>
            {
                ("John", "loves", "nsubj"),
                ("loves", "ROOT", "root"),
                ("Mary", "loves", "dobj")
            };
        }
    }
}
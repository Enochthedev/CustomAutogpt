using System.Collections.Generic;

namespace GemsAi.Core.ERP.NLP.Entities
{
    public static class EntityDefinitions
    {
        public static readonly Dictionary<string, string> EntityPatterns = new Dictionary<string, string>
        {
            { "NameEntity", @"\b[A-Z][a-z]+\b" },   // Capitalized words
            { "DateEntity", @"\b\d{4}-\d{2}-\d{2}\b|\blast quarter\b|\bnext week\b" }, // Dates and time phrases
            { "ActionEntity", @"\bonboard\b|\bupdate\b|\bgenerate\b|\bcreate\b" },  // Common actions
            { "DepartmentEntity", @"\bHR\b|\bFinance\b|\bSales\b|\bIT\b" },   // Known departments
            { "TaskEntity", @"\bpayroll report\b|\bsales report\b|\bmeeting\b" }  // Specific tasks
        };
    }
}
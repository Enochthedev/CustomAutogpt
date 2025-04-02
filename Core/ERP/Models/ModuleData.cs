using System;
using System.Collections.Generic;

namespace GemsAi.Core.ERP.Models
{
    public class ModuleData
    {
        public string ModuleName { get; set; }
        public string Action { get; set; }  // e.g., Create, Update, Delete
        public Dictionary<string, string> Fields { get; set; }

        public ModuleData(string moduleName, string action)
        {
            ModuleName = moduleName;
            Action = action;
            Fields = new Dictionary<string, string>();
        }

        public void AddField(string fieldName, string fieldValue)
        {
            if (!Fields.ContainsKey(fieldName))
            {
                Fields.Add(fieldName, fieldValue);
            }
            else
            {
                Fields[fieldName] = fieldValue;
            }
        }

        public void Display()
        {
            Console.WriteLine($"Module: {ModuleName}, Action: {Action}");
            foreach (var field in Fields)
            {
                Console.WriteLine($"{field.Key}: {field.Value}");
            }
        }

        public override string ToString()
        {
            var details = $"Module: {ModuleName}, Action: {Action}\n";
            foreach (var field in Fields)
            {
                details += $"{field.Key}: {field.Value}\n";
            }
            return details;
        }
    }
}
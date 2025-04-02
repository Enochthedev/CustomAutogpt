using System.Collections.Generic;

namespace GemsAi.Core.ERP.Interfaces
{
    public interface IModuleHandler
    {
        string ModuleName { get; }
        Dictionary<string, string> ParseInput(string input);
        void Execute(Dictionary<string, string> data);
    }
}
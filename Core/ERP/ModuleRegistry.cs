using System;
using System.Collections.Generic;
using GemsAi.Core.ERP.Interfaces;
using GemsAi.Core.ERP.Modules.Onboarding;

namespace GemsAi.Core.ERP
{
    public static class ModuleRegistry
    {
        private static readonly Dictionary<string, IModuleHandler> Handlers = new()
        {
            { "Onboarding", new OnboardingHandler() }
        };

        public static IModuleHandler? GetHandler(string moduleName)
        {
            if (Handlers.TryGetValue(moduleName, out var handler))
            {
                return handler;
            }
            Console.WriteLine($"⚠️ Warning: No handler found for module '{moduleName}'");
            return null;
        }
    }
}
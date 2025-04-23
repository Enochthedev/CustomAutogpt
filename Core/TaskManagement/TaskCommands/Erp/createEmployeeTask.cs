using GemsAi.Core.Ai;
using GemsAi.Core.Services;
using GemsAi.Core.TaskManagement.TaskCommands;
using GemsAi.Core.Models;
using Microsoft.Extensions.Configuration;


namespace GemsAi.Core.TaskManagement.TaskCommands.Erp
{
    public class CreateEmployeeTask : ErpTaskBase
    {
        private readonly IErpApiClient _erpClient;

        // The intent(s) this task handles (lowercase, as LLM outputs)
        private static readonly HashSet<string> SupportedIntents = new() { "add_employee", "onboard_employee" };

        public CreateEmployeeTask(IAiClient client, IErpApiClient erpClient, IConfiguration configuration)
            : base(client, "employee",configuration)
        {
            _erpClient = erpClient;
        }

        public override bool CanHandle(string input)
        {
            input = input.ToLower();
            return (input.Contains("add") || input.Contains("onboard") || input.Contains("invite")) 
                && (input.Contains("department") || input.Contains("team") || input.Contains("employee") || input.Contains("staff"));
        }

        public override bool CanHandleIntent(string intent)
        {
            return SupportedIntents.Contains(intent?.ToLower() ?? "");
        }

        protected override async Task<string> HandleErpOperationAsync(Dictionary<string, string> parsed)
        {
            // ... (your implementation as before)
            // confirmation, DTO, call to _erpClient, etc
            var summary = "Please confirm the following details:\n" +
                          string.Join("\n", parsed.Select(kv => $"{kv.Key}: {kv.Value}"));
            Console.WriteLine(summary);

            Console.Write("Are these details correct? (yes/no): ");
            var confirm = Console.ReadLine()?.Trim().ToLowerInvariant();

            if (confirm != "yes")
                return "Operation cancelled by user.";

            var dto = new CreateEmployeeDto
            {
                Name = parsed["name"],
                Department = parsed["department"],
                StartDate = DateTime.Parse(parsed["date"])
            };
            var result = await _erpClient.CreateEmployeeAsync(dto);

            return result.IsSuccess
                ? $"✅ Employee '{result.EmployeeName}' created with ID {result.EmployeeId}."
                : $"❌ Failed to create employee: {result.ErrorMessage}";
        }
    }
}
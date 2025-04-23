using GemsAi.Core.Ai;
using GemsAi.Core.Services;
using Microsoft.Extensions.Configuration;
using GemsAi.Core.Models;

namespace GemsAi.Core.TaskManagement.TaskCommands.Erp
{
    public class UpdateEmployeeTask : ErpTaskBase
    {
        private readonly IErpApiClient _erpClient;

        private static readonly HashSet<string> SupportedIntents = new() { "update_employee", "edit_employee", "change_employee" };

        public UpdateEmployeeTask(IAiClient client, IErpApiClient erpClient, IConfiguration configuration)
            : base(client, "update_employee", configuration)
        {
            _erpClient = erpClient;
        }

        public override bool CanHandle(string input)
        {
            input = input.ToLower();
            return input.Contains("update") || input.Contains("edit") || input.Contains("change");
        }

        public override bool CanHandleIntent(string intent)
        {
            return SupportedIntents.Contains(intent?.ToLower() ?? "");
        }

        protected override async Task<string> HandleErpOperationAsync(Dictionary<string, string> parsed)
        {
            while (true)
            {
                // Show the fields and values
                Console.WriteLine("Please confirm the following details:");
                foreach (var kv in parsed)
                    Console.WriteLine($"{kv.Key}: {kv.Value}");

                Console.Write("Are these details correct? (yes/no/edit): ");
                var confirm = Console.ReadLine()?.Trim().ToLowerInvariant();

                if (confirm == "yes")
                {
                    var dto = new UpdateEmployeeDto
                    {
                        Name = parsed["name"],
                        Department = parsed["department"],
                        NewPosition = parsed["new_position"],
                        EffectiveDate = DateTime.Parse(parsed["effective_date"]),
                        Email = parsed["email"],
                        Phone = parsed["phone"]
                    };
                    var result = await _erpClient.UpdateEmployeeAsync(dto);

                    return result.IsSuccess
                        ? $"✅ Employee '{dto.Name}' updated successfully."
                        : $"❌ Failed to update employee: {result.ErrorMessage}";
                }
                else if (confirm == "edit")
                {
                    Console.Write("Which field would you like to edit? (" +
                        string.Join("/", parsed.Keys) + "): ");
                    var field = Console.ReadLine()?.Trim().ToLowerInvariant();
                    if (!parsed.ContainsKey(field))
                    {
                        Console.WriteLine("Invalid field. Try again.");
                        continue;
                    }
                    Console.Write($"Enter new value for '{field}': ");
                    var newValue = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrEmpty(newValue))
                    {
                        parsed[field] = newValue;
                    }
                }
                else
                {
                    Console.WriteLine("Operation cancelled by user.");
                    return "Operation cancelled by user.";
                }
            }
        }
    }
}
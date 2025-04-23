using System;

namespace GemsAi.Core.Models
{
    public class CreateEmployeeDto
    {
        public required string Name { get; set; }
        public required string Department { get; set; }
        public DateTime StartDate { get; set; }

        public string? jobTitle { get; set; }
        public string? email { get; set; }

        public string? phone { get; set; }
    }
}

using System;

namespace GemsAi.Core.Models
{
    public class UpdateEmployeeDto
    {
        public string Name { get; set; }
        public string Department { get; set; }
        public string NewPosition { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
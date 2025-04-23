namespace GemsAi.Core.Models
{
    public class UpdateEmployeeResponse
    {
        public bool IsSuccess { get; set; }
        public required string EmployeeName { get; set; }
        public required string EmployeeId { get; set; }
        public string? ErrorMessage { get; set; }

        // Additional properties can be added as needed
        public DateTime? LastUpdated { get; set; }
        public string? UpdatedBy { get; set; }
    }
    public class ErpResult
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        // Additional properties can be added as needed
        public DateTime? ProcessedAt { get; set; }
        public string? ProcessedBy { get; set; }
    }
    
}
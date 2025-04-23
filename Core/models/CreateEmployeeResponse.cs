namespace GemsAi.Core.Models
{
    public class CreateEmployeeResponse
    {
        public bool IsSuccess { get; set; }
        public required string EmployeeName { get; set; }
        public required string EmployeeId { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
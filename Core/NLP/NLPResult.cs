public class NLPResult
{
    public string? Intent { get; set; } // e.g., "generate_text", "fetch_data"
    public Dictionary<string, string> Entities { get; set; } = new Dictionary<string, string>(); // Extracted data (e.g., dates, names)
    public string ProcessedPrompt { get; set; } = string.Empty; // Reformatted prompt
}
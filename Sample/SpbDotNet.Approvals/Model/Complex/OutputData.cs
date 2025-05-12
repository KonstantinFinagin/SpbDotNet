namespace SpbDotNet.Approvals.Model.Complex
{
    public class OutputData
    {
        public int UserId { get; set; }
        public string? NormalizedInput { get; set; }
        public int InputLength { get; set; }
        public bool ContainsDigits { get; set; }
        public string? ReversedInput { get; set; }
        public DateTime ProcessedAt { get; set; }
        public string? Hash { get; set; }
        public bool IsPalindrome { get; set; }

        public string? RawProcessingLog { get; set; }

        public InputMetadata Metadata { get; set; } = null!;
        public ExternalAnalysis ExternalInfo { get; set; } = null!;
    }
}

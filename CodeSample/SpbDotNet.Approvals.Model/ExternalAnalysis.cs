namespace SpbDotNet.Approvals.Model;

public class ExternalAnalysis
{
    public ExternalAnalysis()
    {
        Tags = new();
    }

    public int UserId;
    public string? Category { get; set; }
    public double Score { get; set; }
    public Dictionary<string, string> Tags { get; set; }
}

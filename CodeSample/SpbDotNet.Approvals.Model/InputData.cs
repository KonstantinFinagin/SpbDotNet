namespace SpbDotNet.Approvals.Model;

public class InputData
{
    public InputData()
    {
    }

    public InputData(int userId, string? rawInput)
    {
        UserId = userId;
        RawInput = rawInput;
    }

    public int UserId { get; set; }
    public string? RawInput { get; set; }
}

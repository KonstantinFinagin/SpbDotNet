namespace SpbDotNet.Approvals.Model.Complex
{
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
}

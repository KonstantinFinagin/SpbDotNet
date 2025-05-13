namespace SpbDotNet.Approvals.Model
{
    public class InputMetadata
    {
        public InputMetadata()
        {
            CharFrequency = new();
        }

        public int VowelCount { get; set; }
        public int ConsonantCount { get; set; }
        public Dictionary<char, int> CharFrequency { get; set; }
    }
}

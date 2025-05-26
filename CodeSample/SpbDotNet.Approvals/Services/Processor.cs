using SpbDotNet.Approvals.Model;

namespace SpbDotNet.Approvals.Services;

public class Processor
{
    private readonly IExternalAnalyzer _external;

    public Processor()
    {
        // empty constructor for basic test
    }

    public Processor(IExternalAnalyzer external)
    {
        _external = external;
    }

    public string SayHelloWorld()
    {
        return "Hello, world!";
    }

    public OutputData Process(InputData input)
    {
        var normalized = input.RawInput?.Trim().ToLowerInvariant() ?? "";
        var reversed = new string(normalized.Reverse().ToArray());
        var vowels = "aeiou";
        var charFreq = normalized.GroupBy(c => c)
                                 .ToDictionary(g => g.Key, g => g.Count());

        int vowelCount = normalized.Count(c => vowels.Contains(c));
        int consonantCount = normalized.Count(c => char.IsLetter(c) && !vowels.Contains(c));

        var externalResult = _external.Analyze(normalized, input.UserId);
        var processingLog = $"Processing data for {input.UserId}: lorem ipsum"; // emulated log

        return new OutputData
        {
            UserId = input.UserId,
            NormalizedInput = normalized,
            InputLength = normalized.Length + 1,
            ContainsDigits = normalized.Any(char.IsDigit),
            ReversedInput = reversed,
            ProcessedAt = DateTime.UtcNow, // TODO DateTime Wrapper
            Hash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(normalized)), // simple hash
            IsPalindrome = normalized == reversed,
            Metadata = new InputMetadata
            {
                VowelCount = vowelCount,
                ConsonantCount = consonantCount,
                CharFrequency = charFreq
            },

            ExternalInfo = externalResult,
            RawProcessingLog = processingLog
        };
    }

    public List<OutputData> ProcessBulk(IEnumerable<InputData> inputs)
    {
        return inputs.Select(Process).ToList();
    }
}

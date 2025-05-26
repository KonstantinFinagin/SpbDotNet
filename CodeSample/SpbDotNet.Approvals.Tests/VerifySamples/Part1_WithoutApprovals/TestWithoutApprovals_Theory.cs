namespace SpbDotNet.Approvals.Tests.VerifyTestsSamples.Part1_WithoutApprovals;

public class TestWithoutApprovals_Theory
{
    [Theory]
    [InlineData(1, " Hello123 ", "hello123", 8, true, "321olleh", "aGVsbG8xMjM=", false)]
    [InlineData(2, "Madam", "madam", 5, false, "madam", "bWFkYW0=", true)]
    [InlineData(3, "R@c3Car!", "r@c3car!", 8, true, "!rac3c@r", "ckBjM2NhciE=", false)]
    public void Processor_OutputData_IsCorrect(
        int userId,
        string rawInput,
        string expectedNormalized,
        int expectedLength,
        bool expectedDigits,
        string expectedReversed,
        string expectedHash,
        bool expectedIsPalindrome)
    {
        // Arrange
        DateTimeWrapper.Set(() => new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc));

        var analyzer = AnalyzerMocks.GetAnalyzerMock();

        var processor = new Processor(analyzer);
        var input = new InputData { UserId = userId, RawInput = rawInput };

        // Act
        var result = processor.Process(input);

        // Assert
        Assert.Equal(userId, result.UserId);
        Assert.Equal(expectedNormalized, result.NormalizedInput);
        Assert.Equal(expectedLength, result.InputLength);
        Assert.Equal(expectedDigits, result.ContainsDigits);
        Assert.Equal(expectedReversed, result.ReversedInput);
        Assert.Equal(expectedHash, result.Hash);
        Assert.Equal(expectedIsPalindrome, result.IsPalindrome);
        Assert.Equal(new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc), result.ProcessedAt);

        Assert.Equal("mocked", result.ExternalInfo.Category);
        Assert.Equal(0.9, result.ExternalInfo.Score);
        Assert.Equal("test", result.ExternalInfo.Tags["type"]);

        DateTimeWrapper.Reset();
    }
}

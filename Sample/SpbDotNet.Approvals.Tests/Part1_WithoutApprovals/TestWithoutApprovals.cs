using NSubstitute;
using SpbDotNet.Approvals.Helpers;
namespace SpbDotNet.Approvals.Tests.Part1_WithoutApprovals
{
    public class TestWithoutApprovals
    {
        [Fact]
        public void Processor_OutputData_IsCorrect()
        {
            // Arrange
            DateTimeWrapper.Set(() => new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc));
            var analyzer = AnalyzerMocks.GetAnalyzerMock();

            var processor = new Processor(analyzer);
            var input = new InputData { UserId = 1, RawInput = " Hello123 " };

            // Act
            var result = processor.Process(input);

            // Assert
            Assert.Equal(1, result.UserId);
            Assert.Equal("hello123", result.NormalizedInput);
            Assert.Equal(8, result.InputLength);
            Assert.True(result.ContainsDigits);
            Assert.Equal("321olleh", result.ReversedInput);
            Assert.Equal(new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc), result.ProcessedAt);
            Assert.Equal("aGVsbG8xMjM=", result.Hash);
            Assert.False(result.IsPalindrome);

            Assert.Equal(2, result.Metadata!.VowelCount);
            Assert.Equal(3, result.Metadata.ConsonantCount);
            Assert.Equal(8, result.Metadata.CharFrequency.Values.Sum());
            Assert.Equal(1, result.Metadata.CharFrequency['h']);
            Assert.Equal(2, result.Metadata.CharFrequency['l']);

            Assert.Equal("mocked", result.ExternalInfo.Category);
            Assert.Equal(0.9, result.ExternalInfo.Score);
            Assert.True(result.ExternalInfo.Tags.ContainsKey("type"));
            Assert.Equal("test", result.ExternalInfo.Tags["type"]);

            Assert.Contains("Processing data for 1", result.RawProcessingLog);
        }
    }
}

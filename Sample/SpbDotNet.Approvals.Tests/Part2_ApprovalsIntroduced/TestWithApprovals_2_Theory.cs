namespace SpbDotNet.Approvals.Tests.Part2_ApprovalsIntroduced
{
    [UseReporter(typeof(DiffReporter))]
    [UseApprovalSubdirectory("Results")]
    public class TestWithApprovals_2_Theory
    {
        [Theory]
        [InlineData(1, " Hello123 ")]
        public void Processor_OutputData_IsCorrect(int userId, string rawInput)
        {
            NamerFactory.AdditionalInformation = $"user_{userId}_rawInput_{rawInput.Substring(5)}";

            // Arrange
            DateTimeWrapper.Set(() => new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc));
            var analyzer = AnalyzerMocks.GetAnalyzerMock();
            var processor = new Processor(analyzer);
            var input = new InputData(userId, rawInput);

            // Act
            var result = processor.Process(input);

            // Assert
            ApprovalTests.Approvals.Verify(SerializeToJson(result));
        }

        private string SerializeToJson(OutputData data)
        {
            return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}

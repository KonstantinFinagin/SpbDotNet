namespace SpbDotNet.Approvals.Tests.Part2_ApprovalsIntroduced
{
    [UseReporter(typeof(DiffReporter))]
    [UseApprovalSubdirectory("Results")]
    public class TestWithApprovals_1
    {
        [Fact]
        public void Processor_OutputData_IsCorrect()
        {
            // Arrange
            DateTimeWrapper.Set(() => new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc));
            var analyzer = AnalyzerMocks.GetAnalyzerMock();
            var processor = new Processor(analyzer);
            var input = new InputData(1, " Hello123 ");

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

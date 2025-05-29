namespace SpbDotNet.Approvals.Tests.Part3_TableFormatting;

[UseReporter(typeof(DiffReporter))]
[UseApprovalSubdirectory("Results")]
public class TestBulkWithTableFormatting
{
    [Fact]
    public void Approves_BulkOutput_AsAsciiTable()
    {
        // Arrange
        DateTimeWrapper.Set(() => DateTime.Parse("2024-01-01"));

        var inputs = new[]
        {
            new InputData { UserId = 1, RawInput = "Racecar" },
            new InputData { UserId = 2, RawInput = "Test123" },
            new InputData { UserId = 3, RawInput = "Hello World!" }
        };

        var analyzer = AnalyzerMocks.GetAnalyzerMock();
        var processor = new Processor(analyzer);

        // Act
        var results = processor.ProcessBulk(inputs);

        // Assert
        var table = AsciiTableFormatter.Format(results);
        ApprovalTests.Approvals.Verify(table);
    }
}

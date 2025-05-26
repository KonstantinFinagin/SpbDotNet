namespace SpbDotNet.Approvals.Tests.VerifySamples.Part2_ApprovalsIntroduced;

public class TestWithVerify_1
{
    private VerifySettings _settings;

    public TestWithVerify_1()
    {
        _settings = new VerifySettings();
        _settings.UseDirectory("Results");
    }

    [Fact]
    public async Task Processor_OutputData_IsCorrect()
    {
        // Arrange
        var analyzer = AnalyzerMocks.GetAnalyzerMock();
        var processor = new Processor(analyzer);

        var input = new InputData(1, " Hello123 ");

        // Act
        var result = processor.Process(input);

        // Assert
        await Verifier.Verify(result, _settings)
            .ScrubMembersWithType<DateTime>()
            .ScrubLinesContaining("2024");
    }
}
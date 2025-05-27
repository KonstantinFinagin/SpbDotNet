namespace SpbDotNet.Verify.Tests.Part2_ApprovalsIntroduced;

public class TestWithVerify_5_Partial
{
    private VerifySettings _settings;

    // Verify test focuses on built-in capabilities of ignore mostly
    public TestWithVerify_5_Partial()
    {
        _settings = new VerifySettings();

        _settings.IgnoreMember<ExternalAnalysis>(x => x.Tags);
        _settings.IgnoreMember<OutputData>(x => x.RawProcessingLog);
        _settings.ScrubInlineDateTimes("s");

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
        await Verifier.Verify(result, _settings);
    }
}

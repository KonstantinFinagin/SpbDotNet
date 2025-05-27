namespace SpbDotNet.Verify.Tests.Part2_ApprovalsIntroduced;

public class TestWithVerify_3_PipelineAdapted
{
    private VerifySettings _settings;

    public TestWithVerify_3_PipelineAdapted() 
    {
        _settings = new VerifySettings();
        _settings.UseDirectory("Results");

        if(Environment.GetEnvironmentVariable("CI")?.ToLowerInvariant() == "true") 
        {
            _settings.DisableDiff();
        }
        // or #if DEBUG ... (see TestWithApprovals_4_PipelineAdapted)
    }

    [Theory]
    [InlineData(1, " Hello123 ")]
    public async Task Processor_OutputData_IsCorrect(int userId, string rawInput)
    {
        // Arrange
        var analyzer = AnalyzerMocks.GetAnalyzerMock();
        var processor = new Processor(analyzer);
        var input = new InputData(userId, rawInput);

        // Act
        var result = processor.Process(input);

        // Assert
        await Verifier.Verify(result)
            .UseTextForParameters($"user_{userId}_rawInput_{rawInput.Substring(5)}")
            .ScrubMembersWithType<DateTime>();
    }
}
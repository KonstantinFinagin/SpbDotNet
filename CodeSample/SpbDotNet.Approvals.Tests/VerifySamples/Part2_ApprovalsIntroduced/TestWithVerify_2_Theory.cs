namespace SpbDotNet.Approvals.Tests.VerifySamples.Part2_ApprovalsIntroduced;

public partial class TestWithVerify_2_Theory : VerifyBase
{
    private VerifySettings _settings;

    public TestWithVerify_2_Theory() : base()
    {
        _settings = new VerifySettings();
        _settings.UseDirectory("Results");
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
            .UseTextForParameters($"user_{userId}_rawInput_{rawInput.Substring(5)}") // analog to NamerFactory in ApprovalTestsSamples counterpart
            .ScrubMembersWithType<DateTime>();
    }
}
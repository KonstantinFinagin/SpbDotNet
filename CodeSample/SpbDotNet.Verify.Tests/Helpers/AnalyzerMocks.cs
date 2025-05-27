namespace SpbDotNet.Verify.Tests.Helpers;

internal static class AnalyzerMocks
{
    public static IExternalAnalyzer GetAnalyzerMock()
    {
        var analyzer = Substitute.For<IExternalAnalyzer>();
        analyzer.Analyze(Arg.Any<string>(), Arg.Any<int>()).Returns(callInfo => new ExternalAnalysis
        {
            UserId = callInfo.ArgAt<int>(1),
            Category = "mocked",
            Score = 0.9,
            Tags = new Dictionary<string, string> { ["type"] = "test" }
        });
        return analyzer;
    }
}

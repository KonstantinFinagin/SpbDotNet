using System.Text.RegularExpressions;

namespace SpbDotNet.Approvals.Tests.VerifySamples.Part3_TableFormatting;

public class TestBulkWithTableFormattingVerify
{
    private VerifySettings _settings;

    public TestBulkWithTableFormattingVerify()
    {
        // Scrub dates from the output string
        _settings = new VerifySettings();
        _settings.UseDirectory("Results");

        // VerifierSettings.ScrubInlineDateTimes("yyyy-MM-ddTHH:mm:ss");
        // doesn't work because breaks column length!

        VerifierSettings.AddScrubber(sb =>
        {
            var text = sb.ToString();
            var regex = new Regex(@"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}");

            var replaced = regex.Replace(text, m =>
            {
                var length = m.Value.Length; // 19 for your format
                var replacement = "<DateTime>" + new string('_', length - "<DateTime>".Length);
                return replacement;
            });

            sb.Clear();
            sb.Append(replaced);
        });
    }

    [Fact]
    public async Task Approves_BulkOutput_AsAsciiTable()
    {
        // Arrange
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

        // Format as ASCII table
        var table = AsciiTableFormatter.Format(results);
        await Verify(table, _settings);
    }
}

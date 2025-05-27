using Microsoft.VisualStudio.TestPlatform.CoreUtilities.Extensions;

namespace SpbDotNet.Approvals.Tests.Part3_TableFormatting;

[UseReporter(typeof(DiffReporter))]
[UseApprovalSubdirectory("Results")]
public class TestBulkWithTableFormatting_Flattened
{
    [Fact]
    public void Approves_BulkOutput_AsAsciiTable_Flattened()
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

        DateTimeWrapper.Set(() => DateTime.Parse("2024-01-01"));

        // Act
        var results = processor.ProcessBulk(inputs);

        // Assert

        var sb = new StringBuilder();

        // TABLE 1
        sb.AppendLine("Main info:");
        var flattenedMain = results.Select(FlattenMainData).ToList();
        var formattedMain = AsciiTableFormatter.Format(flattenedMain);
        sb.AppendSafeWithNewLine(formattedMain);

        // TABLE 2
        sb.AppendLine("External info:");
        var externalInfo = results.Select(r => r.ExternalInfo);
        var formattedExternal = AsciiTableFormatter.Format(externalInfo);
        sb.AppendSafeWithNewLine(formattedExternal);

        ApprovalTests.Approvals.Verify(sb.ToString());
    }

    #region formatters

    private static object FlattenMainData(OutputData data)
    {
        // Creating an anonymous object with simple key-value pairs
        return new
        {
            data.UserId,
            data.NormalizedInput,
            data.InputLength,
            data.ContainsDigits,
            data.ReversedInput,
            data.ProcessedAt,
            data.Hash,
            data.IsPalindrome,
            data.Metadata?.VowelCount,
            data.Metadata?.ConsonantCount,
            data.Metadata?.CharFrequency,
            data.RawProcessingLog
        };
    }

    #endregion formatters
}

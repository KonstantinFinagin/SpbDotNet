using Microsoft.VisualStudio.TestPlatform.CoreUtilities.Extensions;
using SpbDotNet.Approvals.Tests.Helpers;

namespace SpbDotNet.Approvals.Tests.Part3_TableFormatting
{
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

            // Act
            var results = processor.ProcessBulk(inputs);

            // Assert

            var sb = new StringBuilder();

            sb.AppendLine("Main info:");
            var flattened = results.Select(Flatten).ToList();
            var formattedMain = AsciiTableFormatter.Format(flattened);
            sb.AppendSafeWithNewLine(formattedMain);

            sb.AppendLine("External info:");
            var externalInfo = results.Select(r => r.ExternalInfo);
            var formattedExternal = AsciiTableFormatter.Format(externalInfo);
            sb.AppendSafeWithNewLine(formattedExternal);

            ApprovalTests.Approvals.Verify(sb.ToString());
        }

        #region formatters

        private static object Flatten(OutputData data)
        {
            // Creating an anonymous object with simple key-value pairs
            return new
            {
                UserId = data.UserId,
                NormalizedInput = data.NormalizedInput,
                InputLength = data.InputLength,
                ContainsDigits = data.ContainsDigits,
                ReversedInput = data.ReversedInput,
                ProcessedAt = data.ProcessedAt,
                Hash = data.Hash,
                IsPalindrome = data.IsPalindrome,
                VowelCount = data.Metadata?.VowelCount,
                ConsonantCount = data.Metadata?.ConsonantCount,
                CharFrequency = data.Metadata?.CharFrequency,
                RawProcessingLog = data.RawProcessingLog
            };
        }

        #endregion formatters
    }
}

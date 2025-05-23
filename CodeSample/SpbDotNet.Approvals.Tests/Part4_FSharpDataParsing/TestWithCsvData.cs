using SpbDotNet.Approvals.TestData;
using SpbDotNet.Approvals.Tests.Helpers;

namespace SpbDotNet.Approvals.Tests.Part4_FSharpDataParsing
{
    [UseReporter(typeof(DiffReporter))]
    [UseApprovalSubdirectory("Results")]
    public class TestBulkWithoutFormatting
    {
        [Fact]
        public void Approves_BulkOutput_AsTable()
        {
            // Arrange
            var inputs = ExcelDataHelper.GetInputDataItems("Part4_FSharpDataParsing/TestData/TestData.xlsx");
            var analyzer = AnalyzerMocks.GetAnalyzerMock();
            var processor = new Processor(analyzer);

            DateTimeWrapper.Set(() => new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc));

            // Act
            var results = processor.ProcessBulk(inputs);

            // Assert
            var table = AsciiTableFormatter.Format(results);
            ApprovalTests.Approvals.Verify(table);
        }
    }
}

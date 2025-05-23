namespace SpbDotNet.Approvals.Tests.Part0_HelloWorld
{
    [UseReporter(typeof(DiffReporter))]
    [UseApprovalSubdirectory("Results")]
    public class BasicApprovalTest
    {
        private Processor? _testedProcessor;

        [Fact]
        public void Processor_OutputData_IsCorrect()
        {
            // Arrange
            _testedProcessor = new();

            // Act
            var helloWorld = _testedProcessor.SayHelloWorld();

            // Assert
            ApprovalTests.Approvals.Verify(helloWorld);
        }
    }
}

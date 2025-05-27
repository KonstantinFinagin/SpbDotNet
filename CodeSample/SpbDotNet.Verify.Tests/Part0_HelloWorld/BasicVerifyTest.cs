namespace SpbDotNet.Verify.Tests.Part0_HelloWorld;

public class BasicVerifyTest
{
    private Processor? _testedProcessor;

    [Fact]
    public async Task Processor_OutputData_IsCorrect()
    {
        // Arrange
        _testedProcessor = new();

        // Act
        var helloWorld = _testedProcessor.SayHelloWorld();

        // Assert
        await Verifier.Verify(helloWorld);
    }
}

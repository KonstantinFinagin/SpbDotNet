using SpbDotNet.Approvals.Model.Complex;

namespace SpbDotNet.Approvals.Services
{
    public interface IExternalAnalyzer
    {
        ExternalAnalysis Analyze(string normalizedInput, int userId);
    }
}

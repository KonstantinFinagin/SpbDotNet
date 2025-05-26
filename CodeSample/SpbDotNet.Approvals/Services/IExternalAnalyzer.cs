using SpbDotNet.Approvals.Model;

namespace SpbDotNet.Approvals.Services;

public interface IExternalAnalyzer
{
    ExternalAnalysis Analyze(string normalizedInput, int userId);
}

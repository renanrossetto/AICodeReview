namespace AICodeReview.Interfaces
{
    public interface ICodeAnalyzerService
    {
        List<string> Analyze(string code);
    }
}

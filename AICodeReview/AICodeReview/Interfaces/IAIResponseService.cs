namespace AICodeReview.Interfaces
{
    public interface IAIResponseService
    {
        Task<string> AIResponse(string prompt);
    }
}

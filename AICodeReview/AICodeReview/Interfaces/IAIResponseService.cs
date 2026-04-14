namespace AICodeReview.Interfaces
{
    public interface IAiResponseService
    {
        Task<string?> AIResponse(string prompt);
    }
}

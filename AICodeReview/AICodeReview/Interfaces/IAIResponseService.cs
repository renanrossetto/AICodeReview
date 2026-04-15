namespace AICodeReview.Interfaces
{
    public interface IAiResponseService
    {
        Task<string?> AiResponse(string prompt);
    }
}

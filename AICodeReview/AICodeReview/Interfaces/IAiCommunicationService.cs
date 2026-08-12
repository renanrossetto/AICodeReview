namespace AICodeReview.Interfaces
{
    public interface IAiCommunicationService
    {
        Task<string?> AiResponse(string prompt);
    }
}

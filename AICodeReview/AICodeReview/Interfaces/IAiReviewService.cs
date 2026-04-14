namespace AICodeReview.Interfaces
{
    public interface IAiReviewService
    {
        Task<string> ReviewAsync(string code, List<string> warnings);
        Task<string> ReviewDiffAsync(string diff, List<string> warnings);
    }
}

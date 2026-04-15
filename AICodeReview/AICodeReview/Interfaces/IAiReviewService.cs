namespace AICodeReview.Interfaces
{
    public interface IAiReviewService
    {
        Task<string> ManualReview(string code, List<string> warnings);
        Task<string> GitCompareReview(string diff, List<string> warnings);
    }
}

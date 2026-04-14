namespace AICodeReview.Models
{
    public class BranchReviewRequest
    {
        public string RepositoryPath { get; set; } = string.Empty;
        public string BaseBranch { get; set; } = string.Empty;
        public string CompareBranch { get; set; } = string.Empty;
    }
}

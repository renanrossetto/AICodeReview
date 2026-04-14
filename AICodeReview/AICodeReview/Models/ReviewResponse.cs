namespace AICodeReview.Models
{
    public class ReviewResponse
    {
        public List<string> Warnings { get; set; } = new List<string>();
        public string ReviewResult { get; set; } = string.Empty;
    }
}

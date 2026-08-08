namespace AICodeReview.Telemetry.TelemetryNames
{
    public static class Metrics
    {
        public const string ManualReviews = "ai_manual_reviews_total";
        public const string PullRequestReviews = "ai_pr_reviews_total";
        public const string Errors = "ai_review_errors_total";
        public const string Duration = "ai_review_duration_ms";
        public const string InputSize = "ai_review_input_size";
    }
}

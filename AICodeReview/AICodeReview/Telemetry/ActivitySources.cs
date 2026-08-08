using System.Diagnostics;

namespace AICodeReview.Telemetry
{
    public static class ActivitySources
    {
        public const string SourceName = "AICodeReview";

        public static readonly ActivitySource AiReview =
            new(SourceName);
    }
}

using System.Diagnostics.Metrics;

namespace AICodeReview.Telemetry
{
    public static class AiReviewTelemetry
    {
        public static readonly Meter Meter = new("AICodeReview");

        public static readonly Counter<long> ManualReviews =
            Meter.CreateCounter<long>(
                TelemetryNames.Metrics.ManualReviews);

        public static readonly Counter<long> PullRequestReviews =
            Meter.CreateCounter<long>(
                TelemetryNames.Metrics.PullRequestReviews);

        public static readonly Counter<long> ReviewErrors =
            Meter.CreateCounter<long>(
                TelemetryNames.Metrics.Errors);

        public static readonly Histogram<double> AiDuration =
            Meter.CreateHistogram<double>(
                TelemetryNames.Metrics.Duration,
                unit: "ms");

        public static readonly Histogram<long> InputSize =
            Meter.CreateHistogram<long>(
                TelemetryNames.Metrics.InputSize,
                unit: "chars");
    }
}
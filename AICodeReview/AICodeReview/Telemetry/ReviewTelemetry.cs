using AICodeReview.Interfaces;
using AICodeReview.Telemetry.TelemetryNames;
using System.Diagnostics;

namespace AICodeReview.Telemetry
{
    public class ReviewTelemetry : IReviewTelemetry
    {
        public void RecordStartTags(Activity? activity, string reviewType, int inputSize, int warningsCount)
        {
            activity?.SetTag(Tags.ReviewType, reviewType);
            activity?.SetTag(Tags.InputSize, inputSize);
            activity?.SetTag(Tags.Warnings, warningsCount);
        }

        public void IncrementReviewCounter(string reviewType)
        {
            if (reviewType == ReviewTypes.Manual)
                AiReviewTelemetry.ManualReviews.Add(1);
            else
                AiReviewTelemetry.PullRequestReviews.Add(1);
        }

        public void RecordSuccess(double durationMs, int inputSize, string reviewType, Activity? activity, string result)
        {
            AiReviewTelemetry.AiDuration.Record(durationMs, new KeyValuePair<string, object?>(Tags.ReviewType, reviewType));
            AiReviewTelemetry.InputSize.Record(inputSize, new KeyValuePair<string, object?>(Tags.ReviewType, reviewType));

            activity?.SetTag(Tags.Success, !string.IsNullOrWhiteSpace(result));
            activity?.SetTag(Tags.Duration, durationMs);
        }

        public void RecordFailure(string reviewType, Activity? activity)
        {
            AiReviewTelemetry.ReviewErrors.Add(1, new KeyValuePair<string, object?>(Tags.ReviewType, reviewType));
            activity?.SetTag(Tags.Success, false);
        }
    }
}

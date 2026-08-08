using System.Diagnostics;

namespace AICodeReview.Interfaces
{
    public interface IReviewTelemetry
    {
        void RecordStartTags(Activity? activity, string reviewType, int inputSize, int warningsCount);
        void IncrementReviewCounter(string reviewType);
        void RecordSuccess(double durationMs, int inputSize, string reviewType, Activity? activity, string result);
        void RecordFailure(string reviewType, Activity? activity);
    }
}

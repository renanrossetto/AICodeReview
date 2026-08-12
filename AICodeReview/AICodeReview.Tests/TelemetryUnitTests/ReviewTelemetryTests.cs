using AICodeReview.Telemetry;
using AICodeReview.Telemetry.TelemetryNames;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace AICodeReview.Tests.TelemetryUnitTests
{
    public class ReviewTelemetryTests
    {
        [Fact]
        public void RecordStartTags_SetsActivityTags()
        {
            // Arrange
            var telemetry = new ReviewTelemetry();
            using var activity = new Activity("test");
            activity.Start();

            // Act
            telemetry.RecordStartTags(activity, "manual", 123, 2);

            // Assert
            Assert.Equal("manual", activity.GetTagItem(Tags.ReviewType)?.ToString());
            Assert.Equal("123", activity.GetTagItem(Tags.InputSize)?.ToString());
            Assert.Equal("2", activity.GetTagItem(Tags.Warnings)?.ToString());

            activity.Stop();
        }

        [Fact]
        public void RecordFailure_SetsSuccessFalse()
        {
            // Arrange
            var telemetry = new ReviewTelemetry();
            using var activity = new Activity("test");
            activity.Start();

            // Act
            telemetry.RecordFailure("manual", activity);

            // Assert
            var successStr = activity.GetTagItem(Tags.Success)?.ToString();
            Assert.True(bool.TryParse(successStr, out var success) && !success);
            activity.Stop();
        }

        [Fact]
        public void IncrementReviewCounter_EmitsCounterMeasurements()
        {
            // Arrange
            var telemetry = new ReviewTelemetry();

            var counters = new ConcurrentDictionary<string, long>();

            using var listener = new MeterListener();

            listener.InstrumentPublished = (instrument, l) =>
            {
                if (instrument.Meter.Name == AiReviewTelemetry.Meter.Name)
                {
                    l.EnableMeasurementEvents(instrument);
                }
            };

            listener.SetMeasurementEventCallback<long>((instrument, measurement, tags, state) =>
            {
                counters.AddOrUpdate(instrument.Name, measurement, (_, old) => old + measurement);
            });

            listener.Start();

            // Act
            telemetry.IncrementReviewCounter(ReviewTypes.Manual);
            telemetry.IncrementReviewCounter(ReviewTypes.PullRequest);

            // Assert
            Assert.True(counters.TryGetValue(Metrics.ManualReviews, out var manualCount));
            Assert.Equal(1L, manualCount);

            Assert.True(counters.TryGetValue(Metrics.PullRequestReviews, out var prCount));
            Assert.Equal(1L, prCount);
        }

        [Fact]
        public void RecordSuccess_EmitsHistogramMeasurements()
        {
            // Arrange
            var telemetry = new ReviewTelemetry();

            var doubles = new ConcurrentDictionary<string, double>();
            var longs = new ConcurrentDictionary<string, long>();

            using var listener = new MeterListener();

            listener.InstrumentPublished = (instrument, l) =>
            {
                if (instrument.Meter.Name == AiReviewTelemetry.Meter.Name)
                {
                    l.EnableMeasurementEvents(instrument);
                }
            };

            listener.SetMeasurementEventCallback<double>((instrument, measurement, tags, state) =>
            {
                doubles.AddOrUpdate(instrument.Name, measurement, (_, old) => old + measurement);
            });

            listener.SetMeasurementEventCallback<long>((instrument, measurement, tags, state) =>
            {
                longs.AddOrUpdate(instrument.Name, measurement, (_, old) => old + measurement);
            });

            listener.Start();

            using var activity = new Activity("test");
            activity.Start();

            // Act
            telemetry.RecordSuccess(150.5, 42, ReviewTypes.Manual, activity, "some result");

            // Assert
            Assert.True(doubles.TryGetValue(Metrics.Duration, out var duration));
            Assert.Equal(150.5, duration, 3);

            Assert.True(longs.TryGetValue(Metrics.InputSize, out var inputSize));
            Assert.Equal(42L, inputSize);

            activity.Stop();
        }
    }
}

using AICodeReview.Telemetry;
using AICodeReview.Telemetry.TelemetryNames;
using System.Diagnostics;

namespace AICodeReview.Tests.ServicesUnitTests.TelemetryUnitTests
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
    }
}

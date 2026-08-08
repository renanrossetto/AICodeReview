using AICodeReview.Interfaces;
using System.Diagnostics;
using System.Text.Json;

namespace AICodeReview.Services
{
    public class AiReviewService : IAiReviewService
    {
        private const string AppSettingsSection = "AiReview";
        private readonly IAiCommunicationService _aiCommunicationService;
        private readonly IPromptBuilder _promptBuilder;
        private readonly IReviewTelemetry _reviewTelemetry;
        private readonly string _codeReviewTemplate;
        private readonly string _pullRequestReview;

        private static readonly ActivitySource ActivitySource = new(AppSettingsSection);

        public AiReviewService(
            IAiCommunicationService aiCommunicationService,
            IConfiguration config,
            IPromptBuilder? promptBuilder = null,
            IReviewTelemetry? reviewTelemetry = null)
        {
            _aiCommunicationService = aiCommunicationService;
            _codeReviewTemplate = config.GetSection(AppSettingsSection).GetValue<string>("CodeReviewTemplate") ?? string.Empty;
            _pullRequestReview = config.GetSection(AppSettingsSection).GetValue<string>("DiffReviewTemplate") ?? string.Empty;

            _promptBuilder = promptBuilder ?? new PromptBuilder();
            _reviewTelemetry = reviewTelemetry ?? new Telemetry.ReviewTelemetry();
        }

        public Task<string> ManualReview(string? code, List<string>? warnings)
        {
            return RunReview(
                activityName: "Manual Review",
                reviewType: "manual",
                template: _codeReviewTemplate,
                input: code ?? string.Empty,
                warnings: warnings,
                inputPlaceholder: "{CODE}");
        }

        public Task<string> GitCompareReview(string? diff, List<string>? warnings)
        {
            return RunReview(
                activityName: "PR Review",
                reviewType: "diff",
                template: _pullRequestReview,
                input: diff ?? string.Empty,
                warnings: warnings,
                inputPlaceholder: "{DIFF}");
        }

        private async Task<string> RunReview(string activityName, string reviewType, string template, string input, List<string>? warnings, string inputPlaceholder)
        {
            using var activity = ActivitySource.StartActivity(activityName);

            var stopwatch = Stopwatch.StartNew();

            var safeInput = input ?? string.Empty;
            var warningsCount = warnings?.Count ?? 0;

            _reviewTelemetry.RecordStartTags(activity, reviewType, safeInput.Length, warningsCount);
            _reviewTelemetry.IncrementReviewCounter(reviewType);

            var prompt = _promptBuilder.Build(template, inputPlaceholder, safeInput, warnings);

            try
            {
                var result = await ExecutePrompt(prompt).ConfigureAwait(false);

                stopwatch.Stop();

                _reviewTelemetry.RecordSuccess(stopwatch.Elapsed.TotalMilliseconds, safeInput.Length, reviewType, activity, result);

                return result;
            }
            catch
            {
                stopwatch.Stop();

                _reviewTelemetry.RecordFailure(reviewType, activity);

                throw;
            }
        }

        private async Task<string> ExecutePrompt(string prompt)
        {
            var json = await _aiCommunicationService.AiResponse(prompt);

            if (string.IsNullOrWhiteSpace(json)) return "";

            using var doc = JsonDocument.Parse(json);

            return doc.RootElement.GetProperty("response").GetString() ?? "";
        }
    }
}

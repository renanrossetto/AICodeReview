using AICodeReview.Interfaces;
using System.Diagnostics;
using System.Text.Json;

namespace AICodeReview.Services.AI
{
    public class AiReviewService : IAiReviewService
    {
        private const string AppSettingsSection = "AiReview";
        private readonly IAiResponseService _aiResponseService;
        private readonly string _codeReviewTemplate;
        private readonly string _pullRequestReview;
        
        private static readonly ActivitySource ActivitySource = new(AppSettingsSection);

        public AiReviewService(IAiResponseService aIResponseService, IConfiguration config)
        {     
            _aiResponseService = aIResponseService;
            _codeReviewTemplate = config.GetSection(AppSettingsSection).GetValue<string>("CodeReviewTemplate") ?? string.Empty;
            _pullRequestReview = config.GetSection(AppSettingsSection).GetValue<string>("DiffReviewTemplate") ?? string.Empty;
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

            var warningsText = string.Join(Environment.NewLine, warnings ?? []);
            var safeInput = input ?? string.Empty;

            activity?.SetTag("review.type", reviewType);
            activity?.SetTag("input.size", safeInput.Length);
            activity?.SetTag("warnings.count", warnings?.Count ?? 0);

            var prompt = template
                .Replace("{WARNINGS}", warningsText)
                .Replace(inputPlaceholder, safeInput);

            var result = await ExecutePrompt(prompt);

            activity?.SetTag("ai.success", !string.IsNullOrEmpty(result));

            return result;
        }

        private async Task<string> ExecutePrompt(string prompt)
        {
            var json = await _aiResponseService.AiResponse(prompt);

            if (string.IsNullOrWhiteSpace(json)) return "";

            using var doc = JsonDocument.Parse(json);

            return doc.RootElement.GetProperty("response").GetString() ?? "";
        }
    }
}

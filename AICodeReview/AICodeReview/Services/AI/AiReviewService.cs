using AICodeReview.Interfaces;
using System.Text.Json;

namespace AICodeReview.Services.AI
{
    public class AiReviewService : IAiReviewService
    {
        private readonly IAiResponseService _aiResponseService;
        private readonly string _codeReviewTemplate;
        private readonly string _pullRequestReview;

        public AiReviewService(IAiResponseService aIResponseService,IConfiguration config)
        {     
            _aiResponseService = aIResponseService;
            _codeReviewTemplate = config.GetSection("AiReview").GetValue<string>("CodeReviewTemplate") ?? string.Empty;
            _pullRequestReview = config.GetSection("AiReview").GetValue<string>("DiffReviewTemplate") ?? string.Empty;
        }

        public async Task<string> ManualReview(string? code, List<string>? warnings)
        {
            var warningsText = string.Join(Environment.NewLine, warnings ?? []);
            var safeCode = code ?? string.Empty;

            var prompt = _codeReviewTemplate
                .Replace("{WARNINGS}", warningsText)
                .Replace("{CODE}", safeCode);

            return await ExecutePrompt(prompt);
        }

        public async Task<string> GitCompareReview(string? diff, List<string>? warnings)
        {
            var warningsText = string.Join(Environment.NewLine, warnings ?? []);
            var safeDiff = diff ?? string.Empty;

            var prompt = _pullRequestReview
                .Replace("{WARNINGS}", warningsText)
                .Replace("{DIFF}", safeDiff);

            return await ExecutePrompt(prompt);
        }

        private async Task<string> ExecutePrompt(string prompt)
        {
            var json = await _aiResponseService.AIResponse(prompt);

            if (string.IsNullOrWhiteSpace(json)) return "";

            using var doc = JsonDocument.Parse(json);

            return doc.RootElement
                      .GetProperty("response")
                      .GetString() ?? "";
        }
    }
}

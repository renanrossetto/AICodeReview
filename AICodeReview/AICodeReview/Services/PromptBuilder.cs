using AICodeReview.Interfaces;

namespace AICodeReview.Services
{
    public class PromptBuilder : IPromptBuilder
    {
        public string Build(string template, string inputPlaceholder, string safeInput, List<string>? warnings)
        {
            var warningsText = string.Join(Environment.NewLine, warnings ?? new List<string>());

            return template
                .Replace("{WARNINGS}", warningsText)
                .Replace(inputPlaceholder, safeInput);
        }
    }
}

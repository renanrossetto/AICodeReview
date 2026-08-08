namespace AICodeReview.Interfaces
{
    public interface IPromptBuilder
    {
        string Build(string template, string inputPlaceholder, string safeInput, List<string>? warnings);
    }
}

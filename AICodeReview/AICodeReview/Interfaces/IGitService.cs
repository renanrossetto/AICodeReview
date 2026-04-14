namespace AICodeReview.Interfaces
{
    public interface IGitService
    {
        string GetModifiedCsFiles(string repositoryPath, string baseBranch, string compareBranch);
    }
}

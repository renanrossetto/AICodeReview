using AICodeReview.Common;
using AICodeReview.Interfaces;
using LibGit2Sharp;

namespace AICodeReview.Services
{
    public class GitService : IGitService
    {
        public string GetModifiedCsFiles(string repositoryPath,string baseBranch,string compareBranch)
        {
            using var repo = new Repository(repositoryPath);

            var baseCommit = repo.Branches[baseBranch]?.Tip;
            var compareCommit = repo.Branches[compareBranch]?.Tip;

            if (baseCommit == null || compareCommit == null)
                throw new Exception(Messages.BranchNotFound);

            var patch = repo.Diff.Compare<Patch>(baseCommit.Tree, compareCommit.Tree);

            var csDiffs = patch
                .Where(p => p.Path.EndsWith(".cs"))
                .Select(p => p.Patch);

            return string.Join("\n", csDiffs);
        }
    }
}

using LibGit2Sharp;
using System.IO;
using AICodeReview.Services.Git;

namespace AICodeReview.Tests.ServicesUnitTests.GitUnitTests
{
    public class GitServiceUnitTests
    {
        [Fact]
        public void GetModifiedCsFiles_ReturnsDiffForCsFiles()
        {
            var repoPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(repoPath);

            try
            {
                Repository.Init(repoPath);

                using (var repo = new Repository(repoPath))
                {
                    var sig = new Signature("tester", "test@test", DateTimeOffset.Now);

                    var filePath = Path.Combine(repoPath, "A.cs");
                    File.WriteAllText(filePath, "class A { }\n");
                    Commands.Stage(repo, "A.cs");
                    repo.Commit("initial", sig, sig);

                    repo.CreateBranch("main");

                    var feature = repo.CreateBranch("feature");
                    Commands.Checkout(repo, feature);

                    File.WriteAllText(filePath, "class A { int x; }\n");
                    Commands.Stage(repo, "A.cs");
                    repo.Commit("change", sig, sig);
                }

                var service = new GitService();
                var result = service.GetModifiedCsFiles(repoPath, "main", "feature");

                Assert.False(string.IsNullOrWhiteSpace(result));
                Assert.Contains("A.cs", result);
                Assert.Contains("int x", result);
            }
            finally
            {
                try { Directory.Delete(repoPath, true); } catch { }
            }
        }

        [Fact]
        public void GetModifiedCsFiles_Throws_WhenBranchNotFound()
        {
            var repoPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(repoPath);

            try
            {
                Repository.Init(repoPath);

                using (var repo = new Repository(repoPath))
                {
                    var sig = new Signature("tester", "test@test", DateTimeOffset.Now);
                    var filePath = Path.Combine(repoPath, "A.cs");
                    File.WriteAllText(filePath, "class A { }\n");
                    Commands.Stage(repo, "A.cs");
                    repo.Commit("initial", sig, sig);

                    repo.CreateBranch("main");
                }

                var service = new GitService();

                Assert.Throws<Exception>(() => service.GetModifiedCsFiles(repoPath, "nonexistent", "main"));
            }
            finally
            {
                try { Directory.Delete(repoPath, true); } catch { }
            }
        }
    }
}

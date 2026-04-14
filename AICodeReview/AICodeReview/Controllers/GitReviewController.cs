using AICodeReview.Interfaces;
using AICodeReview.Models;
using Microsoft.AspNetCore.Mvc;

namespace AICodeReview.Controllers
{
    [ApiController]
    [Route("git-branch-review")]
    public class GitReviewController : ControllerBase
    {
        private readonly IGitService _git;
        private readonly IAiReviewService _ai;

        public GitReviewController(IGitService git, ICodeAnalyzerService stat, IAiReviewService ai)
        {
            _git = git;
            _ai = ai;
        }

        [HttpPost]
        public async Task<IActionResult> ReviewBranch([FromBody] BranchReviewRequest request)
        {
            var diff = _git.GetModifiedCsFiles(request.RepositoryPath, request.BaseBranch, request.CompareBranch);

            if (string.IsNullOrWhiteSpace(diff))
            {
                return Ok(new
                {
                    Message = "Nenhuma alteração em arquivos .cs encontrada."
                });
            }

            var warnings = new List<string>
            {
                "Análise baseada em diff (Pull Request)"
            };

            var aiReview = await _ai.GitCompareReview(diff, warnings);

            return Ok(new
            {
                DiffSize = diff.Length,
                AiReview = aiReview
            });
        }
    }
}

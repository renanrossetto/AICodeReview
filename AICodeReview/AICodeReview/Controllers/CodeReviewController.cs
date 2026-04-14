using Microsoft.AspNetCore.Mvc;
using AICodeReview.Models;
using AICodeReview.Interfaces;

namespace AICodeReview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CodeReviewController : ControllerBase
    {
        private readonly ICodeAnalyzerService _static;
        private readonly IAiReviewService _ai;

        public CodeReviewController(ICodeAnalyzerService stat, IAiReviewService ai)
        {
            _static = stat;
            _ai = ai;
        }

        [HttpPost("manual-review")]
        public async Task<ReviewResponse> Review([FromBody] ReviewRequest request)
        {
            try
            {
                var warnings = _static.Analyze(request.Code);

                var aiReview = await _ai.ReviewAsync(request.Code, warnings);

                return new ReviewResponse
                {
                    Warnings = warnings,
                    ReviewResult = aiReview
                };
            }
            catch (Exception) { throw; }
        }
    }
}

using AICodeReview.Controllers;
using AICodeReview.Interfaces;
using AICodeReview.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AICodeReview.Tests.ControllersUnitTests
{
    public class GitReviewControllerUnitTests
    {
        private readonly Mock<IGitService> _mockGit;
        private readonly Mock<IAiReviewService> _mockAi;
        private readonly Mock<ICodeAnalyzerService> _mockStat;

        public GitReviewControllerUnitTests()
        {
            _mockGit = new Mock<IGitService>();
            _mockAi = new Mock<IAiReviewService>();
            _mockStat = new Mock<ICodeAnalyzerService>();
        }
        
        [Fact]
        public async Task ReviewBranch_ReturnsMessage_WhenNoDiff()
        {
            // Arrange
            _mockGit
                .Setup(g => g.GetModifiedCsFiles(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(string.Empty);

            var controller = new GitReviewController(_mockGit.Object, _mockStat.Object, _mockAi.Object);

            var request = new BranchReviewRequest
            {
                RepositoryPath = "repo",
                BaseBranch = "main",
                CompareBranch = "feature"
            };

            // Act
            var result = await controller.ReviewBranch(request);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var value = ok.Value!;
            var prop = value.GetType().GetProperty("Message");
            var message = prop?.GetValue(value) as string;

            Assert.Equal("Nenhuma alteração em arquivos .cs encontrada.", message);
        }

        [Fact]
        public async Task ReviewBranch_ReturnsDiffSizeAndAiReview_WhenDiffExists()
        {
            // Arrange
            var diff = "- old\n+ new";

            _mockGit
                .Setup(g => g.GetModifiedCsFiles(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(diff);

            _mockAi
                .Setup(a => a.GitCompareReview(It.IsAny<string>(), It.IsAny<List<string>>() ))
                .ReturnsAsync("ai review result");

            var controller = new GitReviewController(_mockGit.Object, _mockStat.Object, _mockAi.Object);

            var request = new BranchReviewRequest
            {
                RepositoryPath = "repo",
                BaseBranch = "main",
                CompareBranch = "feature"
            };

            // Act
            var result = await controller.ReviewBranch(request);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var value = ok.Value!;

            var diffSizeProp = value.GetType().GetProperty("DiffSize");
            var aiReviewProp = value.GetType().GetProperty("AiReview");

            var diffSize = diffSizeProp?.GetValue(value);
            var aiReview = aiReviewProp?.GetValue(value) as string;

            Assert.Equal(diff.Length, diffSize);
            Assert.Equal("ai review result", aiReview);

            _mockAi.Verify(a => a.GitCompareReview(It.Is<string>(s => s == diff), It.Is<List<string>>(l => l.Contains("Análise baseada em diff (Pull Request)"))), Times.Once);
        }
    }
}

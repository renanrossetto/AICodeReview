using AICodeReview.Controllers;
using AICodeReview.Interfaces;
using AICodeReview.Models;
using Moq;

namespace AICodeReview.Tests.ControllersUnitTests
{
    public class CodeReviewControllerUnitTests
    {
        [Fact]
        public async Task Review_ShouldReturnWarningsAndReviewResult_WhenSuccess()
        {
            // Arrange
            var mockStatic = new Mock<IAnalyzeService>();
            var mockAi = new Mock<IAiReviewService>();

            var request = new ReviewRequest
            {
                Code = "int x = 0;"
            };

            var expectedWarnings = new List<string> { "Use var" };
            var expectedReview = "Código simples, mas pode melhorar.";

            mockStatic
                .Setup(s => s.Analyze(request.Code))
                .Returns(expectedWarnings);

            mockAi
                .Setup(a => a.ManualReview(request.Code, expectedWarnings))
                .ReturnsAsync(expectedReview);

            var controller = new CodeReviewController(
                mockStatic.Object,
                mockAi.Object);

            // Act
            var result = await controller.Review(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedWarnings, result.Warnings);
            Assert.Equal(expectedReview, result.ReviewResult);
        }

        [Fact]
        public async Task Review_ShouldThrowException_WhenAIFail()
        {
            // Arrange
            var mockStatic = new Mock<IAnalyzeService>();
            var mockAi = new Mock<IAiReviewService>();

            var request = new ReviewRequest
            {
                Code = "int x = 0;"
            };

            var expectedWarnings = new List<string>();

            mockStatic
                .Setup(s => s.Analyze(It.IsAny<string>()))
                .Returns(expectedWarnings);

            mockAi
                .Setup(a => a.ManualReview(It.IsAny<string>(), It.IsAny<List<string>>()))
                .ThrowsAsync(new Exception("Erro na IA"));

            var controller = new CodeReviewController(
                mockStatic.Object,
                mockAi.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => controller.Review(request));
        }
    }
}

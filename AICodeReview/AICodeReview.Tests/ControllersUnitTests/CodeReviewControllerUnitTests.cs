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
            var mockStatic = new Mock<ICodeAnalyzerService>();
            var mockAi = new Mock<IAiReviewService>();

            var request = new ReviewRequest
            {
                Code = "int x = 0;"
            };

            var warningsEsperados = new List<string> { "Use var" };
            var reviewEsperado = "Código simples, mas pode melhorar.";

            mockStatic
                .Setup(s => s.Analyze(request.Code))
                .Returns(warningsEsperados);

            mockAi
                .Setup(a => a.ManualReview(request.Code, warningsEsperados))
                .ReturnsAsync(reviewEsperado);

            var controller = new CodeReviewController(
                mockStatic.Object,
                mockAi.Object);

            // Act
            var result = await controller.Review(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(warningsEsperados, result.Warnings);
            Assert.Equal(reviewEsperado, result.ReviewResult);
        }

        [Fact]
        public async Task Review_ShouldThrowException_WhenAIFail()
        {
            // Arrange
            var mockStatic = new Mock<ICodeAnalyzerService>();
            var mockAi = new Mock<IAiReviewService>();

            var request = new ReviewRequest
            {
                Code = "int x = 0;"
            };

            var warningsEsperados = new List<string>();

            mockStatic
                .Setup(s => s.Analyze(It.IsAny<string>()))
                .Returns(warningsEsperados);

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

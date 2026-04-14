using AICodeReview.Interfaces;
using AICodeReview.Services.AI;
using Microsoft.Extensions.Configuration;
using Moq;

namespace AICodeReview.Tests.ServicesUnitTests.AIUnitTests
{
    public class AiReviewServiceUnitTests
    {
        [Fact]
        public async Task ReviewAsync_ReturnsEmptyString_WhenAiResponseIsNull()
        {
            // Arrange
            var mockAiResponse = new Mock<IAIResponseService>();
            var inMemory = new Dictionary<string, string>
            {
                { "AiReview:CodeReviewTemplate", "WARNINGS:{WARNINGS};CODE:{CODE}" }
            };
            var config = new ConfigurationBuilder().AddInMemoryCollection(inMemory).Build();

            mockAiResponse
                .Setup(a => a.AIResponse(It.IsAny<string>()))
                .ReturnsAsync((string?)null);

            var service = new AiReviewService(mockAiResponse.Object, config);

            // Act
            var result = await service.ReviewAsync("var x = 1;", new List<string> { "w1" });

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async Task ReviewAsync_ReturnsEmptyString_WhenAiResponseIsWhitespace()
        {
            // Arrange
            var mockAiResponse = new Mock<IAIResponseService>();
            var inMemory = new Dictionary<string, string>
            {
                { "AiReview:CodeReviewTemplate", "WARNINGS:{WARNINGS};CODE:{CODE}" }
            };
            var config = new ConfigurationBuilder().AddInMemoryCollection(inMemory).Build();

            mockAiResponse
                .Setup(a => a.AIResponse(It.IsAny<string>()))
                .ReturnsAsync("   ");

            var service = new AiReviewService(mockAiResponse.Object, config);

            // Act
            var result = await service.ReviewAsync(null, null);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async Task ReviewAsync_ReturnsParsedResponse_WhenAiReturnsValidJson()
        {
            // Arrange
            var mockAiResponse = new Mock<IAIResponseService>();
            var inMemory = new Dictionary<string, string>
            {
                { "AiReview:CodeReviewTemplate", "WARNINGS:{WARNINGS};CODE:{CODE}" }
            };
            var config = new ConfigurationBuilder().AddInMemoryCollection(inMemory).Build();

            var expected = "This is the review";
            var json = $"{{\"response\":\"{expected}\"}}";

            mockAiResponse
                .Setup(a => a.AIResponse(It.IsAny<string>()))
                .ReturnsAsync(json);

            var service = new AiReviewService(mockAiResponse.Object, config);

            // Act
            var result = await service.ReviewAsync("int x = 0;", new List<string> { "Use var" });

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task ReviewAsync_UsesTemplateAndPassesWarningsAndCodeToAi()
        {
            // Arrange
            var mockAiResponse = new Mock<IAIResponseService>();
            var template = "Warnings:[{WARNINGS}]|Code:[{CODE}]";
            var inMemory = new Dictionary<string, string>
            {
                { "AiReview:CodeReviewTemplate", template }
            };
            var config = new ConfigurationBuilder().AddInMemoryCollection(inMemory).Build();

            string? capturedPrompt = null;
            mockAiResponse
                .Setup(a => a.AIResponse(It.IsAny<string>()))
                .Callback<string>(p => capturedPrompt = p)
                .ReturnsAsync("{\"response\":\"ok\"}");

            var service = new AiReviewService(mockAiResponse.Object, config);

            var warnings = new List<string> { "w1", "w2" };
            var code = "class C {}";

            // Act
            var result = await service.ReviewAsync(code, warnings);

            // Assert
            Assert.NotNull(capturedPrompt);
            var expectedWarnings = string.Join(Environment.NewLine, warnings);
            Assert.Contains(expectedWarnings, capturedPrompt!);
            Assert.Contains(code, capturedPrompt!);
            Assert.Equal("ok", result);
        }

        [Fact]
        public async Task ReviewDiffAsync_ReturnsEmptyString_WhenAiResponseIsNull()
        {
            // Arrange
            var mockAiResponse = new Mock<IAIResponseService>();
            var inMemory = new Dictionary<string, string>
            {
                { "AiReview:DiffReviewTemplate", "WARNINGS:{WARNINGS};DIFF:{DIFF}" }
            };
            var config = new ConfigurationBuilder().AddInMemoryCollection(inMemory).Build();

            mockAiResponse
                .Setup(a => a.AIResponse(It.IsAny<string>()))
                .ReturnsAsync((string?)null);

            var service = new AiReviewService(mockAiResponse.Object, config);

            // Act
            var result = await service.ReviewDiffAsync("diff content", new List<string> { "w1" });

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async Task ReviewDiffAsync_ReturnsEmptyString_WhenAiResponseIsWhitespace()
        {
            // Arrange
            var mockAiResponse = new Mock<IAIResponseService>();
            var inMemory = new Dictionary<string, string>
            {
                { "AiReview:DiffReviewTemplate", "WARNINGS:{WARNINGS};DIFF:{DIFF}" }
            };
            var config = new ConfigurationBuilder().AddInMemoryCollection(inMemory).Build();

            mockAiResponse
                .Setup(a => a.AIResponse(It.IsAny<string>()))
                .ReturnsAsync("   ");

            var service = new AiReviewService(mockAiResponse.Object, config);

            // Act
            var result = await service.ReviewDiffAsync(null, null);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async Task ReviewDiffAsync_ReturnsParsedResponse_WhenAiReturnsValidJson()
        {
            // Arrange
            var mockAiResponse = new Mock<IAIResponseService>();
            var inMemory = new Dictionary<string, string>
            {
                { "AiReview:DiffReviewTemplate", "WARNINGS:{WARNINGS};DIFF:{DIFF}" }
            };
            var config = new ConfigurationBuilder().AddInMemoryCollection(inMemory).Build();

            var expected = "This is the diff review";
            var json = $"{{\"response\":\"{expected}\"}}";

            mockAiResponse
                .Setup(a => a.AIResponse(It.IsAny<string>()))
                .ReturnsAsync(json);

            var service = new AiReviewService(mockAiResponse.Object, config);

            // Act
            var result = await service.ReviewDiffAsync("- old\n+ new", new List<string> { "w1" });

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task ReviewDiffAsync_UsesTemplateAndPassesWarningsAndDiffToAi()
        {
            // Arrange
            var mockAiResponse = new Mock<IAIResponseService>();
            var template = "Warnings:[{WARNINGS}]|Diff:[{DIFF}]";
            var inMemory = new Dictionary<string, string>
            {
                { "AiReview:DiffReviewTemplate", template }
            };
            var config = new ConfigurationBuilder().AddInMemoryCollection(inMemory).Build();

            string? capturedPrompt = null;
            mockAiResponse
                .Setup(a => a.AIResponse(It.IsAny<string>()))
                .Callback<string>(p => capturedPrompt = p)
                .ReturnsAsync("{\"response\":\"ok\"}");

            var service = new AiReviewService(mockAiResponse.Object, config);

            var warnings = new List<string> { "w1", "w2" };
            var diff = "- a\n+ b";

            // Act
            var result = await service.ReviewDiffAsync(diff, warnings);

            // Assert
            Assert.NotNull(capturedPrompt);
            var expectedWarnings = string.Join(Environment.NewLine, warnings);
            Assert.Contains(expectedWarnings, capturedPrompt!);
            Assert.Contains(diff, capturedPrompt!);
            Assert.Equal("ok", result);
        }
    }
}

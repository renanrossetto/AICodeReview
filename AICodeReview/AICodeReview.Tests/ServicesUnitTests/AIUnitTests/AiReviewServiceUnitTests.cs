using AICodeReview.Interfaces;
using AICodeReview.Services.AI;
using Microsoft.Extensions.Configuration;
using Moq;

namespace AICodeReview.Tests.ServicesUnitTests.AIUnitTests
{
    public class AiReviewServiceUnitTests
    {        
        private const string GitComparePrompt = "AiReview:DiffReviewTemplate";
        private const string GitCompareCheck = "WARNINGS:{WARNINGS};DIFF:{DIFF}";
        private const string CodePrompt = "AiReview:CodeReviewTemplate";
        private const string CodeCheck = "WARNINGS:{WARNINGS};CODE:{CODE}";
        
        [Fact]
        public async Task ManualReview_ReturnsEmptyString_WhenAiResponseIsNull()
        {
            // Arrange
            var (mockAiResponse, config) = AppSettingsSetup(CodePrompt, CodeCheck);

            mockAiResponse
                .Setup(a => a.AIResponse(It.IsAny<string>()))
                .ReturnsAsync((string?)null);

            var service = new AiReviewService(mockAiResponse.Object, config);

            // Act
            var result = await service.ManualReview("var x = 1;", new List<string> { "w1" });

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async Task ManualReview_ReturnsEmptyString_WhenAiResponseIsWhitespace()
        {
            // Arrange
            var (mockAiResponse, config) = AppSettingsSetup(CodePrompt, CodeCheck);

            mockAiResponse
                .Setup(a => a.AIResponse(It.IsAny<string>()))
                .ReturnsAsync("   ");

            var service = new AiReviewService(mockAiResponse.Object, config);

            // Act
            var result = await service.ManualReview(null, null);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async Task ManualReview_ReturnsParsedResponse_WhenAiReturnsValidJson()
        {
            // Arrange
            var (mockAiResponse, config) = AppSettingsSetup(CodePrompt, CodeCheck);

            var expected = "This is the review";
            var json = $"{{\"response\":\"{expected}\"}}";

            mockAiResponse
                .Setup(a => a.AIResponse(It.IsAny<string>()))
                .ReturnsAsync(json);

            var service = new AiReviewService(mockAiResponse.Object, config);

            // Act
            var result = await service.ManualReview("int x = 0;", new List<string> { "Use var" });

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task ManualReview_UsesTemplateAndPassesWarningsAndCodeToAi()
        {
            // Arrange
            var template = "Warnings:[{WARNINGS}]|Code:[{CODE}]";
            var (mockAiResponse, config) = AppSettingsSetup(CodePrompt, template);

            string? capturedPrompt = null;
            mockAiResponse
                .Setup(a => a.AIResponse(It.IsAny<string>()))
                .Callback<string>(p => capturedPrompt = p)
                .ReturnsAsync("{\"response\":\"ok\"}");

            var service = new AiReviewService(mockAiResponse.Object, config);

            var warnings = new List<string> { "w1", "w2" };
            var code = "class C {}";

            // Act
            var result = await service.ManualReview(code, warnings);

            // Assert
            Assert.NotNull(capturedPrompt);
            var expectedWarnings = string.Join(Environment.NewLine, warnings);
            Assert.Contains(expectedWarnings, capturedPrompt!);
            Assert.Contains(code, capturedPrompt!);
            Assert.Equal("ok", result);
        }

        [Fact]
        public async Task GitCompareReview_ReturnsEmptyString_WhenAiResponseIsNull()
        {
            // Arrange
            var (mockAiResponse, config) = AppSettingsSetup(GitComparePrompt, GitCompareCheck);

            mockAiResponse
                .Setup(a => a.AIResponse(It.IsAny<string>()))
                .ReturnsAsync((string?)null);

            var service = new AiReviewService(mockAiResponse.Object, config);

            // Act
            var result = await service.GitCompareReview("diff content", new List<string> { "w1" });

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async Task GitCompareReview_ReturnsEmptyString_WhenAiResponseIsWhitespace()
        {
            // Arrange
            var (mockAiResponse, config) = AppSettingsSetup(GitComparePrompt, GitCompareCheck);

            mockAiResponse
                .Setup(a => a.AIResponse(It.IsAny<string>()))
                .ReturnsAsync("   ");

            var service = new AiReviewService(mockAiResponse.Object, config);

            // Act
            var result = await service.GitCompareReview(null, null);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async Task GitCompareReview_ReturnsParsedResponse_WhenAiReturnsValidJson()
        {
            // Arrange
            var (mockAiResponse, config) = AppSettingsSetup(GitComparePrompt, GitCompareCheck);

            var expected = "This is the diff review";
            var json = $"{{\"response\":\"{expected}\"}}";

            mockAiResponse
                .Setup(a => a.AIResponse(It.IsAny<string>()))
                .ReturnsAsync(json);

            var service = new AiReviewService(mockAiResponse.Object, config);

            // Act
            var result = await service.GitCompareReview("- old\n+ new", new List<string> { "w1" });

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task GitCompareReview_UsesTemplateAndPassesWarningsAndDiffToAi()
        {
            // Arrange
            var template = "Warnings:[{WARNINGS}]|Diff:[{DIFF}]";
            var (mockAiResponse, config) = AppSettingsSetup(GitComparePrompt, template);

            string? capturedPrompt = null;
            mockAiResponse
                .Setup(a => a.AIResponse(It.IsAny<string>()))
                .Callback<string>(p => capturedPrompt = p)
                .ReturnsAsync("{\"response\":\"ok\"}");

            var service = new AiReviewService(mockAiResponse.Object, config);

            var warnings = new List<string> { "w1", "w2" };
            var diff = "- a\n+ b";

            // Act
            var result = await service.GitCompareReview(diff, warnings);

            // Assert
            Assert.NotNull(capturedPrompt);            
            var expectedWarnings = string.Join(Environment.NewLine, warnings);
            Assert.Contains(expectedWarnings, capturedPrompt!);
            Assert.Contains(diff, capturedPrompt!);
            Assert.Equal("ok", result);
        }

        private static (Mock<IAiResponseService> Mock, IConfiguration Config) AppSettingsSetup(string key, string template)
        {
            var mockAiResponse = new Mock<IAiResponseService>();

            var inMemory = new Dictionary<string, string?>
            {
                { key, template }
            };

            var config = new ConfigurationBuilder().AddInMemoryCollection(inMemory).Build();

            return (mockAiResponse, config);
        }
    }
}

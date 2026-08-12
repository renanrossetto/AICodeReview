using AICodeReview.Services;

namespace AICodeReview.Tests.ServicesUnitTests
{
    public class PromptBuilderTests
    {
        [Fact]
        public void Build_ReplacesWarningsAndPlaceholder()
        {
            // Arrange
            var builder = new PromptBuilder();
            var template = "WARNINGS:{WARNINGS};CODE:{CODE}";
            var warnings = new List<string> { "w1", "w2" };
            var code = "class C {}";

            // Act
            var result = builder.Build(template, "{CODE}", code, warnings);

            // Assert
            var expectedWarnings = string.Join(Environment.NewLine, warnings);
            Assert.Contains(expectedWarnings, result);
            Assert.Contains(code, result);
        }

        [Fact]
        public void Build_HandlesNullWarnings()
        {
            // Arrange
            var builder = new PromptBuilder();
            var template = "WARNINGS:{WARNINGS};CODE:{CODE}";
            string? code = null;

            // Act
            var result = builder.Build(template, "{CODE}", code ?? string.Empty, null);

            // Assert
            Assert.DoesNotContain("null", result, System.StringComparison.OrdinalIgnoreCase);
            Assert.Contains("CODE:", result);
        }
    }
}

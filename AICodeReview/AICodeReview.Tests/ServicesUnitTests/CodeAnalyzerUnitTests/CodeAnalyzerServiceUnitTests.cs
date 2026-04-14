using AICodeReview.Services.CodeAnalyser;

namespace AICodeReview.Tests.ServicesUnitTests.CodeAnalyzerUnitTests
{
    public class CodeAnalyzerServiceUnitTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Analyze_ReturnsEmpty_WhenCodeIsNullOrWhitespace(string code)
        {
            var svc = new CodeAnalyzerService();

            var result = svc.Analyze(code);

            Assert.Empty(result);
        }

        [Fact]
        public void Analyze_FlagsLongMethod_ShouldReturnLogMessage()
        {            
            var methodBody = string.Join('\n', Enumerable.Repeat("    var x = 0;", 41));
            var code = $"public class C {{ public void M()\n{{\n{methodBody}\n}} }}";

            var svc = new CodeAnalyzerService();

            var result = svc.Analyze(code);

            Assert.Contains(result, r => r.Contains("muito grande") && r.Contains("M"));
        }

        [Fact]
        public void Analyze_FlagsLargeClass_ShouldReturnLogMessage()
        {
            var members = string.Join('\n', Enumerable.Range(0, 16).Select(i => $"public void M{i}() {{ }}"));
            var code = $"public class C {{ {members} }}";

            var svc = new CodeAnalyzerService();

            var result = svc.Analyze(code);

            Assert.Contains(result, r => r.Contains("possui muitos membros") && r.Contains("C"));
        }

        [Fact]
        public void Analyze_FlagsUseOfInt_ShouldUseVar()
        {
            var code = "public class C { public void M() { int x = 0; } }";

            var svc = new CodeAnalyzerService();

            var result = svc.Analyze(code);

            Assert.Contains("Use var", result);
        }
    }
}

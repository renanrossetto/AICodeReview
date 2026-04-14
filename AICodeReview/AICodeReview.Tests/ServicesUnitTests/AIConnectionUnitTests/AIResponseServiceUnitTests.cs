using AICodeReview.Services.AIConnection;
using System.Net;

namespace AICodeReview.Tests.ServicesUnitTests.AIConnectionUnitTests
{
    public class AIResponseServiceUnitTests
    {
        private class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handler;

            public FakeHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler)
            {
                _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                => _handler(request, cancellationToken);
        }

        [Fact]
        public async Task AIResponse_ReturnsContent_OnSuccess()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{ \"message\": \"ok\" }")
            };

            var handler = new FakeHttpMessageHandler((req, ct) => Task.FromResult(response));
            var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost") };

            var service = new AIResponseService(client);

            var result = await service.AIResponse("prompt");

            Assert.Equal("{ \"message\": \"ok\" }", result);
        }

        [Fact]
        public async Task AIResponse_ReturnsErrorMessage_OnNonSuccess()
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("bad request body")
            };

            var handler = new FakeHttpMessageHandler((req, ct) => Task.FromResult(response));
            var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost") };

            var service = new AIResponseService(client);

            var result = await service.AIResponse("prompt");

            Assert.Equal("AI response error. 400 - bad request body", result);
        }

        [Fact]
        public async Task AIResponse_ReturnsExceptionMessage_OnException()
        {
            var handler = new FakeHttpMessageHandler((req, ct) => throw new InvalidOperationException("boom"));
            var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost") };

            var service = new AIResponseService(client);

            var result = await service.AIResponse("prompt");

            Assert.StartsWith("AIResponse exception:", result);
            Assert.Contains("boom", result);
        }
    }
}

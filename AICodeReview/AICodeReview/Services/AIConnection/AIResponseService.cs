using AICodeReview.Interfaces;
using System.Text;
using System.Text.Json;

namespace AICodeReview.Services.AIConnection
{
    public class AIResponseService : IAiResponseService
    {
        private readonly HttpClient _http;

        public AIResponseService(HttpClient http)
        {
            _http = http;
        }

        public async Task<string?> AIResponse(string prompt)
        {
            try
            {
                var body = new
                {
                    model = "deepseek-coder:6.7b",
                    prompt = prompt,
                    stream = false
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(body),
                    Encoding.UTF8,
                    "application/json");

                using var response = await _http.PostAsync("/api/generate", content);

                var respContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return ($"AI response error. {(int)response.StatusCode} - {respContent}");
                }

                return respContent;
            }
            catch (Exception ex)
            {
                return ($"AIResponse exception: {ex.Message}");
            }
        }
    }
}

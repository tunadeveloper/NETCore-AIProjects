using System.Text.Json;
using System.Text;

namespace _20_AIProject_RecipeSuggestionWithOpenAI.Models
{
    public class OpenAIService
    {
        private readonly HttpClient _httpClient;
        private const string OpenAIUrl = "https://api.openai.com/v1/chat/completions";
        private const string apiKey = "API_KEY";
        public OpenAIService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }

        public async Task<string> GetRecipeAsync(string ingredients)
        {
            var requestBody = new
            {
                model = "gpt-4",
                messages = new[]
                {
                    new {role="system",content="Sen profesyonel bir aşçısın. Kullanıcının elindeki malzemelere göre yemek tarifi öner."},
                    new {role="user",content=$"Elimde şu malzemeler var: {ingredients}. Ne yapabilirim?"}
                },
                temperature = 0.7
            };

            var jsonRequest = JsonSerializer.Serialize(requestBody);
            var response = await _httpClient.PostAsync(OpenAIUrl, new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
            var responseBody = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(responseBody);
            return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        }

    }
}

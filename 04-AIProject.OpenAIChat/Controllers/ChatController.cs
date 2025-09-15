using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace _04_AIProject.OpenAIChat.Controllers
{
    public class ChatController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                ViewBag.errorMessage = "Lütfen bir mesaj yazın.";
                return View();
            }

            var apiKey = "API_KEY";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new object[]
                {
                new { role = "system", content = "You are a helpful assistant." },
                new { role = "user", content = prompt }
                },
                max_tokens = 100
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<JsonElement>(responseString);
                    var answer = result.GetProperty("choices")[0]
                                       .GetProperty("message")
                                       .GetProperty("content")
                                       .GetString();
                    return View("Index", answer);
                }
                else
                {
                    ViewBag.errorMessage = $"Bir Hata Oluştu: {response.StatusCode}";
                    ViewBag.errorDetails = responseString;
                    return View("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.errorMessage = $"Bir Hata Oluştu: {ex.Message}";
                return View("Index");
            }
        }
    }

}

using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace _19_AIProject_CreateStoryWithAI.Controllers
{
    public class CreateStoryController : Controller
    {
        private readonly string apiKey = "API_KEY";

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string genre, string character, string setting, string length)
        {
            if (string.IsNullOrWhiteSpace(genre) || string.IsNullOrWhiteSpace(character)
                || string.IsNullOrWhiteSpace(setting) || string.IsNullOrWhiteSpace(length))
            {
                ViewBag.Error = "Lütfen tüm alanları doldurun.";
                return View();
            }

            string prompt = $"{genre} türünde bir hikaye yaz. Baş karakterin adı {character}. Hikaye {setting} bölgesinde geçiyor. {length} bir hikaye olsun. Giriş, gelişme ve sonuç içermeli.";

            try
            {
                string story = await GenerateStory(prompt);
                ViewBag.Story = story;
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Hikaye oluşturulurken bir hata oluştu: " + ex.Message;
            }

            return View();
        }

        private async Task<string> GenerateStory(string prompt)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new { role = "system", content = "Sen yaratıcı bir hikaye yazarı olarak Türkçe hikaye oluşturuyorsun." },
                    new { role = "user", content = prompt }
                },
                max_tokens = 1000
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", jsonContent);

            string responseContent = await response.Content.ReadAsStringAsync();
            JsonDocument doc = JsonDocument.Parse(responseContent);
            return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        }
    }
}

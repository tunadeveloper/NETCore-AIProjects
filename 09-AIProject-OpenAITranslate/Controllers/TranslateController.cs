using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace _09_AIProject_OpenAITranslate.Controllers
{
    public class TranslateController : Controller
    {
        private readonly string apiKey = "API_KEY";
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Translate(string inputText)
        {
            string translatedText = await TranslateTextToEnglish(inputText, apiKey);
            ViewBag.TranslatedText = translatedText;
            return View("Index");
        }

        private async Task<string> TranslateTextToEnglish(string text, string apiKey)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[] {
                        new { role = "system", content = "You are a helpful translator." },
                        new { role = "user", content = $"Please translate this text to English: {text}" }
                    }
                };
                string jsonBody = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                    string responseString = await response.Content.ReadAsStringAsync();

                    dynamic responseObject = JsonConvert.DeserializeObject(responseString);
                    string translation = responseObject.choices[0].message.content;

                    return translation;
                }
                catch (Exception ex)
                {
                    return $"Hata oluştu: {ex.Message}";
                }
            }
        }
    }
}


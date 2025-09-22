using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace _11_AIProject_TextToSpeechWithOpenAI.Controllers
{
    public class TextToSpeechAIController : Controller
    {
        private readonly string apiKey = "API_KEY";

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string input)
        {
            if (!string.IsNullOrEmpty(apiKey))
            {
                byte[] audioBytes = await GenerateSpeech(input);

                if (audioBytes != null)
                {
                    string base64Audio = Convert.ToBase64String(audioBytes);
                    ViewBag.AudioData = base64Audio;
                }
            }
            ViewBag.InputText = input;
            return View();
        }

        private async Task<byte[]> GenerateSpeech(string text)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var requesBody = new
                {
                    model = "tts-1",
                    input = text,
                    voice = "alloy"
                };

                string json = JsonConvert.SerializeObject(requesBody);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/audio/speech", content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }

                return null;
            }
        }
    }
}

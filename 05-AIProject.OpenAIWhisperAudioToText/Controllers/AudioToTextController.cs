using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace _05_AIProject.OpenAIWhisperAudioToText.Controllers
{
    public class AudioToTextController : Controller
    {
        private readonly string apiKey = "API_KEY";

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile audioFile)
        {
            if (audioFile == null || audioFile.Length == 0)
            {
                ViewBag.errorMessage = "Lütfen bir ses dosyası seçin!";
                return View();
            }

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                using var form = new MultipartFormDataContent();
                using var stream = audioFile.OpenReadStream();
                var audioContent = new StreamContent(stream);
                audioContent.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg");
                form.Add(audioContent, "file", audioFile.FileName);
                form.Add(new StringContent("whisper-1"), "model");

                var response = await client.PostAsync("https://api.openai.com/v1/audio/transcriptions", form);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();

                    var jsonObj = System.Text.Json.JsonDocument.Parse(jsonResult);
                    string text = jsonObj.RootElement.GetProperty("text").GetString();

                    return View("Index", text);
                }
                else
                {
                    ViewBag.errorMessage = $"Bir Hata Oluştu: {response.StatusCode}";
                    ViewBag.errorDetails = await response.Content.ReadAsStringAsync();
                    return View();
                }
            }
        }
    }
}

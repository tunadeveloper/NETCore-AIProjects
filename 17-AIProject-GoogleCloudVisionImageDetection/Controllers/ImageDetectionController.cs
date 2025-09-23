using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace _17_AIProject_GoogleCloudVisionImageDetection.Controllers
{
    public class ImageDetectionController : Controller
    {
        private readonly string googleApiKey = "API_KEY";

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                ViewBag.Error = "Lütfen bir görsel seçin.";
                return View();
            }

            string responseJson;
            using (var ms = new MemoryStream())
            {
                await imageFile.CopyToAsync(ms);
                var imageBytes = ms.ToArray();
                responseJson = await DetectObjects(imageBytes);
            }

            ViewBag.Result = responseJson;
            return View();
        }

        private async Task<string> DetectObjects(byte[] imageBytes)
        {
            using (var client = new HttpClient())
            {
                string apiUrl = $"https://vision.googleapis.com/v1/images:annotate?key={googleApiKey}";

                string base64Image = Convert.ToBase64String(imageBytes);

                var requestBody = new
                {
                    requests = new[]
                    {
                        new
                        {
                            image = new { content = base64Image },
                            features = new[] { new { type = "LABEL_DETECTION", maxResults = 10 } }
                        }
                    }
                };

                var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(apiUrl, jsonContent);
                string responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
        }
    }
}

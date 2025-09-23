using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using UglyToad.PdfPig;

namespace _16_AIProject_PdfAnalyzeWithOpenAI.Controllers
{
    public class PdfAnalyzeController : Controller
    {
        private readonly string apiKey = "API_KEY";

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile pdfFile)
        {
            if (pdfFile == null || pdfFile.Length == 0)
            {
                ViewBag.Error = "Lütfen bir PDF dosyası seçin.";
                return View();
            }

            string pdfText;
            using (var stream = pdfFile.OpenReadStream())
            {
                StringBuilder text = new StringBuilder();
                using (PdfDocument pdf = PdfDocument.Open(stream))
                {
                    foreach (var page in pdf.GetPages())
                    {
                        text.AppendLine(page.Text);
                    }
                }
                pdfText = text.ToString();
            }

            ViewBag.Text = await AnalyzeWithAI(pdfText, "PDF İçeriği");
            return View();
        }

        private async Task<string> AnalyzeWithAI(string text, string sourceType)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = "Sen bir yapay zeka asistanısın. Gönderilen metni analiz eder ve sadece Türkçe özetlersin." },
                        new { role = "user", content = $"Lütfen şu {sourceType} içeriğini analiz et ve özetle:\n\n{text}" }
                    }
                };

                string json = JsonConvert.SerializeObject(requestBody);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
                string responseJson = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<dynamic>(responseJson);
                    return result.choices[0].message.content.ToString();
                }
                else
                {
                    return $"Hata: {responseJson}";
                }
            }
        }
    }
}

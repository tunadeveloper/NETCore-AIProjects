using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace _15_AIProject_WebScrapingWithOpenAI.Controllers
{
    public class ScrapingController : Controller
    {
        private readonly string apiKey = "API_KEY";
        public async Task<IActionResult> Index(string inputUrl)
        {
            if (string.IsNullOrWhiteSpace(inputUrl))
            {
                ViewBag.Error = "Lütfen geçerli bir URL girin.";
                return View();
            }

            string webContent = ExtractTextFromWeb(inputUrl);
            await AnalyzeWithAI(webContent, "Web Sayfası İçeriği");
            return View();
        }


        private string ExtractTextFromWeb(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return "Geçersiz URL.";

            try
            {
                var web = new HtmlWeb();
                var doc = web.Load(url);
                var bodyText = doc.DocumentNode.SelectSingleNode("//body")?.InnerText;
                return bodyText ?? "Sayfa İçeriği Okunmadı.";
            }
            catch (Exception ex)
            {
                return $"Web sayfası yüklenirken hata oluştu: {ex.Message}";
            }
        }

        private async Task AnalyzeWithAI(string text, string sourceType)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[] {
                        new { role = "system", content = "Sen bir yapay zeka asistanısın. Kullanıcının gönderdiği metni analiz eder ve türkçe olarak özetlersin. Yanıtlarını sadece türkçe olarak ver." },
                        new { role = "user", content = $"Analyze amd summarize the following {sourceType}: \n\n {text}" }
                    }
                };

                string json = JsonConvert.SerializeObject(requestBody);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions\r\n", content);
                string responseJson = await response.Content.ReadAsStringAsync();
            
                if(response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<dynamic>(responseJson);
                    ViewBag.Text = $"AI Analizi ({sourceType}): \n {result.choices[0].message.content}";
                }
                else
                {
                    ViewBag.Error = $"Hata: {responseJson}";
                }
    }
    }
}
}

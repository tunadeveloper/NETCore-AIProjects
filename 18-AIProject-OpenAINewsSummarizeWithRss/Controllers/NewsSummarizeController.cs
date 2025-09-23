using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace _18_AIProject_OpenAINewsSummarizeWithRss.Controllers
{
    public class NewsSummarizeController : Controller
    {
        private readonly string apiKey = "API_KEY";

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string rssUrl)
        {
            if (string.IsNullOrWhiteSpace(rssUrl))
            {
                ViewBag.Error = "Lütfen geçerli bir RSS adresi giriniz.";
                return View();
            }

            try
            {
                List<string> articles = await FetchLatestNews(rssUrl, 5);

                var summaries = new List<(string Title, string Summary)>();
                foreach (var article in articles)
                {
                    string summary = await SummarizeArticle(article);
                    string title = article.Split('.')[0];
                    summaries.Add((title, summary));
                }

                ViewBag.Summaries = summaries;
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Bir hata oluştu: " + ex.Message;
            }

            return View();
        }

        private async Task<List<string>> FetchLatestNews(string rssUrl, int count)
        {
            using var client = new HttpClient();
            string rssContent = await client.GetStringAsync(rssUrl);

            XDocument doc = XDocument.Parse(rssContent);
            var items = doc.Descendants("item").Take(count);

            List<string> articles = items.Select(item =>
            {
                string title = item.Element("title")?.Value ?? "";
                string description = item.Element("description")?.Value ?? "";
                return $"{title}. {description}";
            }).ToList();

            return articles;
        }

        private async Task<string> SummarizeArticle(string articleText)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new { role = "system", content = "Sen profesyonel bir haber özetleyicisin. Yanıtlarını Türkçe ver." },
                    new { role = "user", content = "Bu haberi 3 cümlede özetle: " + articleText }
                },
                max_tokens = 300
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", jsonContent);
            string responseContent = await response.Content.ReadAsStringAsync();

            JsonDocument doc = JsonDocument.Parse(responseContent);
            return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        }
    }
}

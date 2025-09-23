using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace _14_AIProject_ArticleSummarizeAI.Controllers
{
    public class SummarizeController : Controller
    {
        private readonly string apiKey = "API_KEY";
        public async Task<IActionResult> Index(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                ViewBag.Input = input;
                ViewBag.ShortSummary = await SummarizeText(input, "short");
                ViewBag.MediumSummary = await SummarizeText(input, "medium");
                ViewBag.DetailedSummary = await SummarizeText(input, "detailed");
            }
            return View();
        }


        private async Task<string> SummarizeText(string text, string level)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                string instruction = level switch
                {
                    "short" => "Summarize this text in 1-2 sentences.",
                    "medium" => "Summarize this text in 3-5 sentences.",
                    "detailed" => "Summarize this text in a detailed but concise manner.",
                    _ => "summarize this text."
                };

                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[] {
                        new { role = "system", content = "You are an AI that summarize text info different levels: short, medium and detailed." },
                        new { role = "user", content = $"{instruction}\n\n{text}" }
                    }
                };

                string json = JsonConvert.SerializeObject(requestBody);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
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

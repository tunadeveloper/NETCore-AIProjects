using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace _12_AIProject_SentimentAIApp.Controllers
{
    public class SentimentController : Controller
    {
        private readonly string apiKey = "API_KEY";
        public async Task<IActionResult> Index(string input)
        {
            string sentiment = await AnalyzeSentiment(input);
            ViewBag.Sentiment = sentiment;
            return View();
        }


        private async Task<string> AnalyzeSentiment(string text)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new {role = "system", content = "You are an AI that analyzes sentiment. You categorize text as Positive, Negative or Neutral"},
                        new {role = "user", content=$"Analyze the sentiment of this text: \"{text}\" and return only Positive, Negative, or Neutral"}
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

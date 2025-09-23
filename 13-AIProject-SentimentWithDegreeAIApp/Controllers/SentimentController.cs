using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace _13_AIProject_SentimentWithDegreeAIApp.Controllers
{
    public class SentimentController : Controller
    {
        private readonly string apiKey = "API_KEY";
        public async Task<IActionResult> Index(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                string sentiment = await AdvancedSentimentalAnalysis(input);
                ViewBag.Sentiment = sentiment;
            }
            return View();
        }

        private async Task<string> AdvancedSentimentalAnalysis(string text)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = "You are an advanced AI that analyzes emotions in text. Your Response must be in JSON format. Identify the sentiment scores (0-100%) for the following emotions: Joy, Sadness, Anger, Fear, Surprise, and Neutral." },
                        new {role = "user", content = $"Analyze this text: \"{text}\" and return a JSON object with percentages for each emotions."}
                    }
                };

                string json = JsonConvert.SerializeObject(requestBody);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                string responseJson = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<dynamic>(responseJson);
                    string analysis = result.choices[0].message.content.ToString();
                    return analysis;
                }
                else
                {
                    return $"Hata: {responseJson}";
                }
            }
        }


    }
}

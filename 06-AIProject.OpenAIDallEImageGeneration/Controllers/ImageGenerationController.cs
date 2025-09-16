using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace _06_AIProject.OpenAIDallEImageGeneration.Controllers
{
    public class ImageGenerationController : Controller
    {
        public async Task<IActionResult> Index(string prompts)
        {
            string imageUrl = null;

            if (!string.IsNullOrEmpty(prompts))
            {
                string apikey = "API_KEY";

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apikey);
                    var requestBody = new
                    {
                        prompt = prompts,
                        n = 1,
                        size = "512x512"
                    };

                    string jsonBody = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/images/generations", content);
                    string responseString = await response.Content.ReadAsStringAsync();

                    var jsonObj = JObject.Parse(responseString);
                    imageUrl = jsonObj["data"]?[0]?["url"]?.ToString();
                }
            }

            return View((object)imageUrl);
        }
    }
}

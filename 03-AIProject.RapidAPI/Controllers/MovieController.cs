using _03_AIProject.RapidAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace _03_AIProject.RapidAPI.Controllers
{
    public class MovieController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://imdb-top-100-movies.p.rapidapi.com/series/"),
                Headers =
                {
                    { "x-rapidapi-key", "e71177d867mshfa37763bf5e2e10p1b4212jsn34b1dc08d068" },
                    { "x-rapidapi-host", "imdb-top-100-movies.p.rapidapi.com" },
                },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var allMovies = JsonConvert.DeserializeObject<List<ApiSeriesViewModel>>(body);

                var validMovies = new List<ApiSeriesViewModel>();

                foreach (var movie in allMovies)
                {
                    if (string.IsNullOrEmpty(movie.image))
                        continue;

                    try
                    {
                        var headRequest = new HttpRequestMessage(HttpMethod.Head, movie.image);
                        var headResponse = await client.SendAsync(headRequest);
                        if (headResponse.IsSuccessStatusCode)
                        {
                            validMovies.Add(movie);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                return View(validMovies);
            }
        }
    }
}

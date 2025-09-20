using Google.Cloud.Vision.V1;
using Microsoft.AspNetCore.Mvc;

namespace _08_AIProject_GoogleCloudVision.Controllers
{
    public class OcrController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(IFormFile imageFile)
        {
            string credentialPath = "C:\\Users\\tunah\\Desktop\\mystical-banner-472710-k7-0de4e124ec09.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);

            try
            {
                var client = ImageAnnotatorClient.Create();

                using (var stream = imageFile.OpenReadStream())
                {
                    var image = Image.FromStream(stream);
                    var response = client.DetectText(image);
                    foreach (var item in response)
                    {
                        if (!string.IsNullOrEmpty(item.Description))
                        {
                            ViewBag.description = item.Description;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ViewBag.error = "Hata: " + ex.Message;
            }
            return View();
        }
    }
}

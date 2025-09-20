using Microsoft.AspNetCore.Mvc;
using Tesseract;

namespace _07_AIProject.TesseractOcr.Controllers
{
    public class TesseractController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Index(IFormFile imageFile)
        {
            string tessDataPath = @"C:\tessdata";
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            string filePath = Path.Combine(uploadPath, imageFile.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                imageFile.CopyTo(stream);
            }

            try
            {
                using (var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default))
                {
                    using(var img = Pix.LoadFromFile(filePath))
                    {
                        using (var page = engine.Process(img))
                        {
                            string text = page.GetText();
                            ViewBag.Text = text;
                        }
                }
            }
            }
            catch (Exception ex)
            {
                ViewBag.error = $"Bir hata oluştu: {ex.Message}";
            }
            ViewBag.ImagePath = "/uploads/" + imageFile.FileName;
            return View();
        }
    }
}

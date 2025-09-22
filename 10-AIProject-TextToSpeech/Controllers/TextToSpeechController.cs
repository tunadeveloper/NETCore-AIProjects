using Microsoft.AspNetCore.Mvc;
using System.Speech.Synthesis;
namespace _10_AIProject_TextToSpeech.Controllers
{
    public class TextToSpeechController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(string input)
        {
            SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();

            speechSynthesizer.Volume = 100;
            speechSynthesizer.Rate = 0;
            
            if(!string.IsNullOrEmpty(input))
            {
                speechSynthesizer.Speak(input);
            }
            return View();
        }
    }
}

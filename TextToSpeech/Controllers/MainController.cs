using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Web;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Net.Http;

namespace TextToSpeech.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MainController : ControllerBase
    {
        [HttpPost("tts/{text}")]
        public async Task<ActionResult> TurnToSpeech(string text)
        {
            var config = SpeechConfig.FromSubscription("355dfe15905a4ee990f74ddef984a703", "brazilsouth");
            config.SpeechSynthesisLanguage = "pt-BR";
            config.SpeechSynthesisVoiceName = "pt-BR-AntonioNeural";

            var filePath = $"C:\\Users\\51917267827.INFOSCS\\source\\repos\\TextToSpeech\\file.wav";
            
            using var audioConfig = AudioConfig.FromWavFileOutput(filePath);
            {
                using var synthesizer = new SpeechSynthesizer(config, audioConfig);
                await synthesizer.SpeakTextAsync(text);
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, "audio/wav", Path.GetFileName(filePath));
        }

        [HttpPost("stt")]
        public async Task<IActionResult> TurnToText(IFormFile file)
        {
            if (file.ContentType != "audio/wav")
            {
                return BadRequest("Tipo de arquivo incorreto.");
            }

            var path = "C:\\Users\\51917267827.INFOSCS\\source\\repos\\TextToSpeech\\audio.wav";
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var config = SpeechConfig.FromSubscription("355dfe15905a4ee990f74ddef984a703", "brazilsouth");
            config.SpeechSynthesisLanguage = "pt-BR";
            config.SpeechSynthesisVoiceName = "pt-BR-AntonioNeural";

            using var audioConfig = AudioConfig.FromWavFileInput(path);
            using var recognizer = new SpeechRecognizer(config, "pt-BR", audioConfig);

            var result = await recognizer.RecognizeOnceAsync();
            return Ok(result.Text);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using WikstromIT.Services;

namespace WikstromIT.Controllers
{
    public class AIController : Controller
    {
        private readonly DeepSeekService _deepSeekService;
        //private readonly HuggingFaceService _huggingFaceService;

        public AIController(DeepSeekService deepSeekService)
        {
            _deepSeekService = deepSeekService;
        }

        //public AIController(HuggingFaceService huggingFaceService)
        //{
        //    _huggingFaceService = huggingFaceService;
        //}

        public IActionResult Index()
        {
            return PartialView();
        }


        [HttpPost]
        public async Task<IActionResult> SubmitText(string UserInput)
        {
            if (!string.IsNullOrEmpty(UserInput))
            {
                // Send the input to DeepSeek and retrieve the AI response
                var aiResponse = await _deepSeekService.GetAIResponse(UserInput);
                //var aiResponse = await _huggingFaceService.GetAIResponse(UserInput);
                ViewData["AIResponse"] = aiResponse;
            }

            return PartialView("Index");
        }
    }
}

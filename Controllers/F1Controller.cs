using Microsoft.AspNetCore.Mvc;
using WikstromIT.Services;

namespace WikstromIT.Controllers
{
    public class F1Controller : Controller
    {
        private readonly OpenF1Service _openF1Service;

        public F1Controller(OpenF1Service openF1Service)
        {
            _openF1Service = openF1Service;
        }

        public IActionResult Index()
        {
            return PartialView();
        }

        private static List<Meeting> _cachedRaceLocations = new List<Meeting>();

        [HttpGet]
        public async Task<IActionResult> GetRaceLocations()
        {
            if (!_cachedRaceLocations.Any())
            {
                _cachedRaceLocations = await _openF1Service.GetRaceLocations();
            }

            return Json(_cachedRaceLocations);
        }

        [HttpGet]
        public IActionResult GetRaceDetails(int meetingKey)
        {
            var raceDetails = _cachedRaceLocations.FirstOrDefault(r => r.MeetingKey == meetingKey);

            if (raceDetails == null)
            {
                return NotFound($"No race details found for meeting key {meetingKey}");
            }

            return Json(raceDetails);
        }

        [HttpGet]
        public async Task<IActionResult> GetPositions(int meetingKey)
        {
            var result = await _openF1Service.GetPositions(meetingKey);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetDriver(int driverNumber, int sessionKey)
        {
            var result = await _openF1Service.GetDriverData(driverNumber, sessionKey);
            return Json(result);
        }
    }
}

using Newtonsoft.Json;

namespace WikstromIT.Services
{
    public class OpenF1Service
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://api.openf1.org/v1/";

        public OpenF1Service(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
                
        public async Task<List<Meeting>> GetRaceLocations()
        {
            var requestUrl = $"{ApiUrl}meetings?year=2025";
            var response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var meetings = JsonConvert.DeserializeObject<List<Meeting>>(jsonResponse);

            return meetings;
        }

        public async Task<List<Position>> GetPositions(int meetingKey)
        {
            //var requestUrl = $"{ApiUrl}position?meeting_key=1217&driver_number=40&position<=3&session_key=latest";
            var requestUrl = $"{ApiUrl}position?meeting_key={meetingKey}&position=1";
            var response = await _httpClient.GetAsync(requestUrl); response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var positions = JsonConvert.DeserializeObject<List<Position>>(jsonResponse);

            return positions;
        }

        public async Task<string> GetDriverData(int driverNumber, int sessionKey)
        {
            var requestUrl = $"{ApiUrl}drivers?driver_number={driverNumber}&session_key={sessionKey}";
            var response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }

    public class Meeting
    {
        [JsonProperty("circuit_key")]
        public int CircuitKey { get; set; }

        [JsonProperty("circuit_short_name")]
        public string CircuitShortName { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("country_key")]
        public int CountryKey { get; set; }

        [JsonProperty("country_name")]
        public string CountryName { get; set; }

        [JsonProperty("date_start")]
        public DateTime DateStart { get; set; }

        [JsonProperty("gmt_offset")]
        public string GmtOffset { get; set; }

        [JsonProperty("location")]
        public string Location {  get; set; }

        [JsonProperty("meeting_key")]
        public int MeetingKey { get; set; }

        [JsonProperty("meeting_name")]
        public string MeetingName { get; set; }

        [JsonProperty("meeting_official_name")]
        public string MeetingOfficialName { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }
    }

    public class Position
    {
        [JsonProperty("date")]
        public DateTime RaceDate { get; set; }

        [JsonProperty("driver_number")]
        public int DriverNumber { get; set; }

        [JsonProperty("meeting_key")]
        public int MeetingKey { get; set; }

        [JsonProperty("position")]
        public int PositionNr { get; set; }

        [JsonProperty("session_key")]
        public int SessionKey { get; set; }
    }
}

using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WikstromIT.Services
{
    public class DeepSeekService
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "sk-4cbe446302844484a88cbeae109ccbd7"; // Replace with your actual DeepSeek API key
        //private const string ApiUrl = "https://platform.deepseek.com/api/v1/chat";
        private const string ApiUrl = "https://api.deepseek.com/chat/completions";


        public DeepSeekService(HttpClient httpClient)
        {
            var handler = new HttpClientHandler { UseCookies = true };
            _httpClient = new HttpClient(handler);
        }

        public async Task<string> GetAIResponse(string userMessage)
        {
            var requestData = new
            {
                messages = new[]
                {
                    new { content = "You are a helpful assistant", role = "system" },
                    new { content = userMessage, role = "user" }
                },
                model = "deepseek-chat",
                frequency_penalty = 0,
                max_tokens = 2048,
                presence_penalty = 0,
                response_format = new { type = "text" },
                stop = (string)null,
                stream = false,
                stream_options = (object)null,
                temperature = 1,
                top_p = 1,
                tools = (object)null,
                tool_choice = "none",
                logprobs = false,
                top_logprobs = (object)null
            };

            var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {ApiKey}");

            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
            request.Content = jsonContent;

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WikstromIT.Services
{
    public class HuggingFaceService
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "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";
        private const string ApiUrl = "https://api-inference.huggingface.co/models/deepseek-ai/DeepSeek-V3-0324";

        public HuggingFaceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetAIResponse(string userMessage)
        {
            var requestData = new { inputs = userMessage };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");

            var response = await _httpClient.PostAsync(ApiUrl, jsonContent);
            return await response.Content.ReadAsStringAsync();
        }
    }

}

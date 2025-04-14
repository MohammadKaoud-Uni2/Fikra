using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Fikra.Helper
{
    public   class PostGridAddressValidator
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "test_sk_vRG29i9NCFp35Tb9kHTgrc"; 
        private const string BaseUrl = "https://api.postgrid.com/v1/";

        public PostGridAddressValidator()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.DefaultRequestHeaders.Add("x-api-key",ApiKey);
        }

        public async Task<string> ValidateAddressAsync(string line1, string city, string postalCode, string country )
        {
            var requestBody = new
            {
                address=line1+city+postalCode+country,
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("addver/verifications?includeDetails=true", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"PostGrid API error: {response.StatusCode} - {responseContent}");
            }

            return responseContent;
        }
    }
}


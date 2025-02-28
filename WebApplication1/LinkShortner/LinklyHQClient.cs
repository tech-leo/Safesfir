namespace Safesfir.WebService
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class LinklyHQClient
    {
        private readonly HttpClient _httpClient;
       
        public LinklyHQClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://app.linklyhq.com/api/v1/")
            };
        }

        public async Task<string> ShortenUrlAsync(string destinationUrl)
        {
            var requestBody = new
            {
                url = destinationUrl,
                workspace_id= 268798
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync("link?api_key=srbJF/q3AsHKIpWdRqGHTw==", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                return destinationUrl;
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var responseJson = JsonSerializer.Deserialize<LinklyResponse>(responseBody);
            return responseJson?.full_url ?? throw new Exception("Failed to retrieve short URL.");
        }
    }

    public class LinklyResponse
    {
        public string full_url { get; set; }
    }

}

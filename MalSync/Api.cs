using System.Net.Http;
using System.Net.Http.Headers;

namespace MalSync
{
    public static class Api
    {
        private static readonly string UserAgent = "MalSync";
        public static readonly HttpClient Client = RequestClient();

        public const string KitsuBaseUri = "https://kitsu.io/api/edge";
        public const string KitsuBaseAuthUri = "https://kitsu.io/api/oauth";
        
        private static HttpClient RequestClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.api+json"));
            client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
            return client;
        }
    }
}
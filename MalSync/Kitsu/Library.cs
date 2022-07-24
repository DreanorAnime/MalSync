using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace MalSync.Kitsu
{
    public class Library
    {
        public static async Task<string> UpdateAnime(int animeId, int episode, int rating, Status status)
        {
            rating *= 2;
            
            string body = @"{  
               ""data"":{  
                  ""id"":ANIMEID,
                  ""type"": ""library-entries"",
                  ""attributes"":{ ""status"":""STATUS"", ""progress"":""EPISODE"", ""ratingTwenty"":""RATING"" }
               }
            }";
            
            if (rating == 0)
            {
                body = @"{  
               ""data"":{  
                  ""id"":ANIMEID,
                  ""type"": ""library-entries"",
                  ""attributes"":{ ""status"":""STATUS"", ""progress"":""EPISODE"" }
               }
            }";
            }

            body = body.Replace("ANIMEID", animeId.ToString())
                .Replace("EPISODE", episode.ToString())
                .Replace("RATING", rating.ToString())
                .Replace("STATUS", status.ToString());

            string responseData = string.Empty;
            using (var content = new StringContent(body, System.Text.Encoding.Default, "application/vnd.api+json"))
            {
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{Api.KitsuBaseUri}/library-entries/{animeId}") { Content = content };

                using (var response = await Api.Client.SendAsync(request))
                {
                    responseData = await response.Content.ReadAsStringAsync();
                }
            }

            return responseData;
        }
    }
}

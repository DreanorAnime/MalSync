using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MalSync.Kitsu
{
    public class User
    {
        public static async Task<dynamic> Get()
        {
            var json = await Api.Client.GetStringAsync($"{Api.KitsuBaseUri}/users?filter[self]=true");
            return JsonConvert.DeserializeObject(json);
        }
        
        public static async Task<dynamic> GetLibrary(int userId, int animeId)
        {
            var json = await Api.Client.GetStringAsync($"{Api.KitsuBaseUri}/users/{userId}/library-entries?filter[animeId]={animeId}");
            return JsonConvert.DeserializeObject(json);
        }
    }
}

using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MalSync.Kitsu
{
    public class User
    {
        public static async Task<dynamic> Get(string username)
        {
            var json = await Api.Client.GetStringAsync($"{Api.KitsuBaseUri}/users?filter[name]={username}");
            return JsonConvert.DeserializeObject(json);
        }
        
        public static async Task<dynamic> GetLibrary(int userId, int animeId)
        {
            var json = await Api.Client.GetStringAsync($"{Api.KitsuBaseUri}/users/{userId}/library-entries?filter[animeId]={animeId}");
            return JsonConvert.DeserializeObject(json);
        }
    }
}

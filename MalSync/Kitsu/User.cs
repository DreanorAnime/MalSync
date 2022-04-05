using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MalSync.Kitsu
{
    public class User
    {
        public static async Task<dynamic> GetUserAsync(string username)
        {
            var json = await Api.Client.GetStringAsync($"{Api.KitsuBaseUri}/users?filter[name]={username}");
            return JsonConvert.DeserializeObject(json);
        }
    }
}

using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MalSync.Mal
{
    public static class User
    {
        public static async Task<dynamic> GetUserUpdates(string username)
        {
            var json = await Api.Client.GetStringAsync($"{Api.JikanBaseUri}/users/{username}/userupdates");
            return JsonConvert.DeserializeObject(json);
        }
    }
}


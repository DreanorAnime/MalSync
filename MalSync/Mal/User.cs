using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MalSync.Mal
{
    public static class User
    {
        public static async Task<dynamic> GetUserUpdates(string username)
        {
            var json = await Api.Client.GetStringAsync($"https://myanimelist.net/animelist/{username}/load.json?offset=0&status=7&order=5");
            return JsonConvert.DeserializeObject(json);
        }
    }
}


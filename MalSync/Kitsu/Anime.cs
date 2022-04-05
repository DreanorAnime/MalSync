using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MalSync.Kitsu
{
    public static class Anime
    {
        public static async Task<dynamic> FindAnime(string name)
        {
            var json = await Api.Client.GetStringAsync($"{Api.KitsuBaseUri}/anime?filter[text]={name}");
            return JsonConvert.DeserializeObject(json);
        }
    }
}


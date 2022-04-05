using System;
using System.Threading.Tasks;
using MalSync.Mal;
using Newtonsoft.Json;
using Authentication = MalSync.Kitsu.Authentication;
using Library = MalSync.Kitsu.Library;
using User = MalSync.Kitsu.User;

namespace MalSync
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Program p = new Program();
            try
            {
                Console.Write("user: ");
                string user = Console.ReadLine();
                
                Console.Write("password: ");
                string password = null;
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                        break;
                    password += key.KeyChar;
                }
                Console.WriteLine();
                
                p.Run(user, password);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadLine();
        }

        private async Task Run(string username, string password)
        {
            //auth
            var auth = await Authentication.Authenticate(username, password);
            int kitsuUserId = await User.GetUserAsync(username);
            Console.WriteLine("authenticated.");

            //get userupdates
            var historyJson = await Api.Client.GetStringAsync($"https://api.jikan.moe/v4/users/{username}/userupdates");
            UserUpdates userUpdates = JsonConvert.DeserializeObject<UserUpdates>(historyJson);

            foreach (var userUpdate in userUpdates.data.anime)
            {
                var kitsuJson = await Api.Client.GetStringAsync($"https://kitsu.io/api/edge/anime?filter[text]={userUpdate.entry.title}");
                dynamic kitsuObject = JsonConvert.DeserializeObject(kitsuJson);
                var kitsuData = kitsuObject.data[0];
                int id = kitsuData.id;
                
                await UpdateKitsu(userUpdate.entry.title, kitsuUserId, id, userUpdate.episodes_seen, userUpdate.score, userUpdate.status);
            }
            
            Console.WriteLine("Done. Press any key to exit...");
        }
        private async Task UpdateKitsu(string entryTitle, int kitsuUserId, int animeId, int episodeNumber,
            int userUpdateScore, string userUpdateStatus)
        {
            //check for update
            var kitsuJson = await Api.Client.GetStringAsync($"{Api.KitsuBaseUri}/users/{kitsuUserId}/library-entries?filter[animeId]={animeId}");
            dynamic kitsuObject = JsonConvert.DeserializeObject(kitsuJson);

            if (kitsuObject.data[0].attributes.progress == episodeNumber)
            {
                Console.WriteLine($"Already exists. (anime: {entryTitle} episode: {episodeNumber})");
                return;
            }

            int id = kitsuObject.data[0].id;
            var status = StatusConverter.ConvertMalToKitsuStatus(userUpdateStatus);
            var result = await Library.UpdateAnime(id, episodeNumber, userUpdateScore, status);

            if (!string.IsNullOrEmpty(result) && result.Contains("errors"))
            {
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine($"Updated anime. ({entryTitle} episode: {episodeNumber} score: {userUpdateScore} status: {status})");
            }
        }
    }
}
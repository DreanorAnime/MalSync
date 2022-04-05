using System;
using System.Threading.Tasks;
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
            Console.Write("Kitsu User: ");
            string user = Console.ReadLine();

            Console.Write("Kitsu Password: ");
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
            Console.ReadLine();
        }

        private async Task Run(string username, string password)
        {
            try
            {
                //auth
                Console.WriteLine("Authenticating...");
                var auth = await Authentication.Authenticate(username, password);
                Console.WriteLine("Authenticated.");

                Console.WriteLine("Retrieving UserId...");
                var userData = await User.Get(username);
                Console.WriteLine("UserId returned.");

                //get userupdates
                var userUpdates = await Mal.User.GetUserUpdates(username);

                Console.WriteLine("Checking for updates...");
                foreach (var userUpdate in userUpdates.data.anime)
                {
                    var animeObject = await Kitsu.Anime.FindAnime((string)userUpdate.entry.title);
                    var kitsuData = animeObject.data[0];

                    await UpdateKitsu((string)userUpdate.entry.title, (int)userData.data[0].id, (int)kitsuData.id,
                        (int)userUpdate.episodes_seen, (int)userUpdate.score, (string)userUpdate.status);
                }

                Console.WriteLine("Done!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                Console.WriteLine("Press any key to exit...");
            }
        }

        private async Task UpdateKitsu(string entryTitle, int kitsuUserId, int animeId, int episodeNumber,
            int userUpdateScore, string userUpdateStatus)
        {
            //check for update
            var kitsuObject = await User.GetLibrary(kitsuUserId, animeId);

            if (kitsuObject.data[0].attributes.progress == episodeNumber)
            {
                Console.WriteLine($"Already exists. ({entryTitle} episode: {episodeNumber})");
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
                Console.WriteLine($"Updated {entryTitle} episode: {episodeNumber} score: {userUpdateScore} status: {status}");
            }
        }
    }
}
using System;
using System.Linq;
using System.Net.Http.Headers;
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
            if (args.Length == 4)
            {
                new Program().Run(args[0], args[1], args[2], Convert.ToInt32(args[3]));
            }

            Console.ReadLine();
        }

        private async Task Run(string kitsuUsername, string malUsername, string password, int changes)
        {
            try
            {
                //auth
                Console.WriteLine("Authenticating...");
                var auth = await Authentication.Authenticate(kitsuUsername, password);
                var user = await User.Get();
                int userId = Convert.ToInt32((string)user.data[0].id);
                Console.WriteLine("Authenticated.");

                //get userupdates
                var malContent = await Mal.User.GetUserUpdates(malUsername);

                Console.WriteLine($"Checking last {changes} changes...");
                var count = 0;
                foreach (var entry in malContent)
                {
                    var animeObject = await Kitsu.Anime.FindAnime((string)entry.anime_title);

                    Console.WriteLine($"Checking: {(string)entry.anime_title}");
                    dynamic kitsuData = null;
                    foreach (var obj in animeObject.data)
                    {
                        string malJP = (string)entry.anime_title;
                        string malEN = (string)entry.anime_title_eng;
                        string malStartDate = (string)entry.anime_start_date_string; //14-07-22
                        
                        string kitsuJP = (string)obj.attributes.titles.en_jp;
                        string kitsuEN = (string)obj.attributes.titles.en;
                        string ktisuStartDate = (string)obj.attributes.startDate; // "2022-07-14"
                        
                        if (!string.IsNullOrWhiteSpace(malEN) && !string.IsNullOrWhiteSpace(kitsuEN) && (malEN.Any(char.IsDigit) || kitsuEN.Any(char.IsDigit)))
                        {
                            malEN = ReplaceRoman(malEN);
                            kitsuEN = ReplaceRoman(kitsuEN);
                        }

                        DateTime malDateParsed;
                        DateTime kitsuDateParsed;

                        var malParsable = DateTime.TryParse(malStartDate, out malDateParsed);
                        var kitsuParsable = DateTime.TryParse(ktisuStartDate, out kitsuDateParsed);
                        
                        var levJP = !string.IsNullOrWhiteSpace(malJP) && !string.IsNullOrWhiteSpace(kitsuJP)
                            ? LevenshteinDistance.Calculate(malJP, kitsuJP)
                            : 0;
                        
                        var levEN = !string.IsNullOrWhiteSpace(malEN) && !string.IsNullOrWhiteSpace(kitsuEN)
                            ? LevenshteinDistance.Calculate(malEN, kitsuEN)
                            : 0;
                        
                        if (malJP == kitsuJP 
                            || !string.IsNullOrWhiteSpace(malEN) && !string.IsNullOrWhiteSpace(kitsuEN) && malEN == kitsuEN
                            || malParsable && kitsuParsable && malDateParsed==kitsuDateParsed && (levJP < 3 || levEN < 3))
                        {
                            kitsuData = obj;
                            break;
                        }
                    }

                    if (kitsuData == null)
                    {
                        Console.WriteLine($"Unable to detect {(string)entry.anime_title}");
                        continue;
                    }
                    
                    await UpdateKitsu((string)entry.anime_title, userId, (int)kitsuData.id,
                        (int)entry.num_watched_episodes, (int)entry.score, (int)entry.status);
                    
                    count++;
                    if (count == changes)
                    {
                        break;
                    }
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

        private string ReplaceRoman(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return string.Empty;
            }
            
            return title.Replace("1", "I")
                    .Replace("2", "II")
                    .Replace("3", "III")
                    .Replace("4", "IV")
                    .Replace("5", "V")
                    .Replace("6", "VI")
                    .Replace("7", "VII");
        }

        private async Task UpdateKitsu(string entryTitle, int kitsuUserId, int animeId, int episodeNumber,
            int userUpdateScore, int userUpdateStatus)
        {
            //check for update
            var kitsuObject = await User.GetLibrary(kitsuUserId, animeId);

            if (kitsuObject.meta.count == 0)
            {
                Console.WriteLine($"Not found. ({entryTitle})");
                return;
            }
            
            var rating = (decimal)kitsuObject.data[0].attributes.rating;
            var progress = (int)kitsuObject.data[0].attributes.progress;
            var oldStatus = (string)kitsuObject.data[0].attributes.status;
            if (oldStatus == "completed" || (progress == episodeNumber && rating*2 == userUpdateScore))
            {
                Console.WriteLine($"Already exists. ({entryTitle} Episode: {episodeNumber} Rating: {userUpdateScore})");
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
                Console.WriteLine($"Updated {entryTitle} Episode: {progress} > {episodeNumber} Rating: {rating*2} > {userUpdateScore} Status: {oldStatus} > {status}");
            }
        }
    }
}
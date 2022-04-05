using System;
using System.Collections.Generic;

namespace MalSync.Mal
{
    public class UserUpdates
    {
        public Data data { get; set; }
    }
    
    public class Jpg
    {
        public string image_url { get; set; }
        public string small_image_url { get; set; }
        public string large_image_url { get; set; }
    }

    public class Webp
    {
        public string image_url { get; set; }
        public string small_image_url { get; set; }
        public string large_image_url { get; set; }
    }

    public class Images
    {
        public Jpg jpg { get; set; }
        public Webp webp { get; set; }
    }

    public class Entry
    {
        public int mal_id { get; set; }
        public string url { get; set; }
        public Images images { get; set; }
        public string title { get; set; }
    }

    public class Anime
    {
        public Entry entry { get; set; }
        public int score { get; set; }
        public string status { get; set; }
        public int episodes_seen { get; set; }
        public int episodes_total { get; set; }
        public DateTime date { get; set; }
    }

    public class Manga
    {
        public Entry entry { get; set; }
        public int score { get; set; }
        public string status { get; set; }
        public object chapters_read { get; set; }
        public object chapters_total { get; set; }
        public DateTime date { get; set; }
    }

    public class Data
    {
        public List<Anime> anime { get; set; }
        public List<Manga> manga { get; set; }
    }
}
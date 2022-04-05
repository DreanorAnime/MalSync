using System;
using MalSync.Kitsu;

namespace MalSync
{
    public static class StatusConverter
    {
        public static Status ConvertMalToKitsuStatus(string userUpdateStatus)
        {
            Status status = Status.Invalid;
            switch (userUpdateStatus.ToLower())
            {
                case "watching":
                    status = Status.current;
                    break;
                case "completed":
                    status = Status.completed;
                    break;
                case "on_hold":
                    status = Status.on_hold;
                    break;
                case "dropped":
                    status = Status.dropped;
                    break;
                case "plan_to_watch":
                    status = Status.planned;
                    break;
            }

            if (status == Status.Invalid)
            {
                Console.WriteLine("Error wrong Status.");
            }
            
            return status;
        }
    }
}

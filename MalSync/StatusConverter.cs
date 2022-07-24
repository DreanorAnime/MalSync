using System;
using MalSync.Kitsu;

namespace MalSync
{
    public static class StatusConverter
    {
        public static Status ConvertMalToKitsuStatus(int userUpdateStatus)
        {
            Status status = Status.Invalid;
            switch (userUpdateStatus)
            {
                case 1:
                    status = Status.current;
                    break;
                case 2:
                    status = Status.completed;
                    break;
                case 3:
                    status = Status.on_hold;
                    break;
                case 4:
                    status = Status.dropped;
                    break;
                case 6:
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

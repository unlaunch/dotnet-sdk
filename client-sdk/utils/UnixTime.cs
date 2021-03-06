﻿using System;

namespace io.unlaunch.utils
{
    public class UnixTime
    {
        public static long Get()
        {
            var span = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return (long) span.TotalMilliseconds;
        }

        public static long Get(DateTime utcDateTime)
        {
            var span = utcDateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return (long)span.TotalMilliseconds;
        }

        public static DateTime GetUtcDateTime(long unixTimeMilliseconds)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTime.AddMilliseconds(unixTimeMilliseconds);
        }
    }
}

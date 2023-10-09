using System;

namespace Common.Libs
{
    public static class DateTimeHelper
    {
        public static long GetTimeStamp()
        {
            return DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        }
    }
}

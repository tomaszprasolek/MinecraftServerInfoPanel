using System;

namespace MinecraftServerInfoPanel.Helpers
{
    public static class TimeSpanExtenstions
    {
        public static string ToFriendlyString(this TimeSpan time)
        {
            if (time.TotalDays >= 1)
                return $"{time:%d} dni {time:hh\\:mm\\:ss}";
            return $"{time:hh\\:mm\\:ss}";
        }
    }
}

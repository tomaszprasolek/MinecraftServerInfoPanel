using System;

namespace MinecraftServerInfoPanel.ViewModels
{
    public class UserTimeViewModel
    {
        public string UserName { get; set; }

        public TimeSpan PlayTime { get; set; }

        public string PlayTimeFriendly
        {
            get
            {
                if (PlayTime.TotalDays >= 1)
                    return $"{PlayTime:%d} dni {PlayTime:hh\\:mm\\:ss}";
                return $"{PlayTime:hh\\:mm\\:ss}";
            }
        }
    }
}

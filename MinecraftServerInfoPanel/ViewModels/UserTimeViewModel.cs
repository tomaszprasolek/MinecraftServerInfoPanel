using MinecraftServerInfoPanel.Helpers;
using System;

namespace MinecraftServerInfoPanel.ViewModels
{
    public class UserTimeViewModel
    {
        public string UserName { get; set; }

        public TimeSpan PlayTime { get; set; }

        public string PlayTimeFriendlyString => PlayTime.ToFriendlyString();
    }
}

using System;

namespace MinecraftServerInfoPanel.BL.PlayTimeCalculator
{
    public interface IPlayTimeCalculator
    {
        TimeSpan CalculateUserAllPlayTime(string userName);
        TimeSpan CalculateUserPlayTime(string userName, TimePeriod period, DateTime? date);
    }
}

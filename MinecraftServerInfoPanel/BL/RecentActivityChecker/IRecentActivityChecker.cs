using System.Threading.Tasks;

namespace MinecraftServerInfoPanel.BL.RecentActivityChecker
{
    public interface IRecentActivityChecker
    {
        Task<bool> CheckAsync();
    }
}

using System.Threading.Tasks;

namespace MinecraftServerInfoPanel.BL.RecentActivityEmailSender
{
    public interface IRecentActivityEmailSender
    {
        Task Send();
    }
}

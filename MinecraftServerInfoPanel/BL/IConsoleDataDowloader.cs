using System.Collections.Generic;
using System.Threading.Tasks;

namespace MinecraftServerInfoPanel.BL
{
    public interface IConsoleDataDowloader
    {
        Task<List<ConsoleLog>> Download();
    }
}

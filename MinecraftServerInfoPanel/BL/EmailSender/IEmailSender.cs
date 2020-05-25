using System.Threading.Tasks;

namespace MinecraftServerInfoPanel.BL.EmailSender
{
    public interface IEmailSender
	{
		Task SendEmailAsync(string to, string subject, string body);
	}
}

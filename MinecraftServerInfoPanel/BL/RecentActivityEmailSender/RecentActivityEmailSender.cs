using MinecraftServerInfoPanel.BL.EmailSender;
using MinecraftServerInfoPanel.Database;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerInfoPanel.BL.RecentActivityEmailSender
{

    public class RecentActivityEmailSender : IRecentActivityEmailSender
    {
        private readonly MinecraftDbContext dbContext;
        private readonly IEmailSender emailSender;

        public RecentActivityEmailSender(MinecraftDbContext dbContext, IEmailSender emailSender)
        {
            this.dbContext = dbContext;
            this.emailSender = emailSender;
        }

        public async Task Send()
        {
            var logs = dbContext.ConsoleLogs
                .Where(x => x.IsNeededToSendEmail && x.SendEmail == false)
                .ToList();

            if (logs.Count == 0)
                return;

            var htmlBody = new StringBuilder(392);
            htmlBody.AppendLine(@"<table class=""table table-striped"">");
            htmlBody.AppendLine(@"    <thead>");
            htmlBody.AppendLine(@"        <tr>");
            htmlBody.AppendLine(@"            <th scope=""col"">Data</th>");
            htmlBody.AppendLine(@"            <th scope=""col"">Informacja</th>");
            htmlBody.AppendLine(@"        </tr>");
            htmlBody.AppendLine(@"    </thead>");
            htmlBody.AppendLine(@"    <tbody>");

            for (int i = 0; i < logs.Count; i++)
            {
                htmlBody.AppendLine(@"            <tr>");
                htmlBody.AppendLine(@$"                <td>{logs[i].Date}</td>");
                htmlBody.AppendLine(@$"                <td>{logs[i].Information}</td>");
                htmlBody.AppendLine(@"            </tr>");
            }

            htmlBody.AppendLine(@"    </tbody>");
            htmlBody.AppendLine(@"</table>");

            await emailSender
                .SendEmailAsync("tomasz.prasolek@gmail.com", "Ostatnia aktywność na serwerze", htmlBody.ToString());
        }
    }
}

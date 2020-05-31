using Microsoft.EntityFrameworkCore;
using MinecraftServerInfoPanel.BL.EmailSender;
using MinecraftServerInfoPanel.Database;
using System.Collections.Generic;
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
            var logs = GetLogsWhichNeedToSendEmail();
            if (logs.Count == 0)
                return;

            await SendEmailWithRecentActivity(logs);

            MarkLogsAsSended(logs);
        }

        private List<DbConsoleLog> GetLogsWhichNeedToSendEmail()
        {
            return dbContext.ConsoleLogs
                .Where(x => x.IsNeededToSendEmail && x.SendEmail == false)
                .ToList();
        }

        private async Task SendEmailWithRecentActivity(List<DbConsoleLog> logs)
        {
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


            var emails = dbContext.Emails.Select(x => x.EmailAddress).ToList();

            for (int i = 0; i < emails.Count; i++)
            {
                await emailSender
                    .SendEmailAsync(emails[i], "Ostatnia aktywność na serwerze", htmlBody.ToString());
            }
        }

        private void MarkLogsAsSended(List<DbConsoleLog> logs)
        {
            for (int i = 0; i < logs.Count; i++)
            {
                logs[i].SendEmail = true;
                dbContext.Entry(logs[i]).State = EntityState.Modified;
            }

            dbContext.SaveChanges();
        }
    }
}

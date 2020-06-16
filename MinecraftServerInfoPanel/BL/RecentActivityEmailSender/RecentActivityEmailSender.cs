using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using MinecraftServerInfoPanel.BL.EmailSender;
using MinecraftServerInfoPanel.Database;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerInfoPanel.BL.RecentActivityEmailSender
{

    public class RecentActivityEmailSender : IRecentActivityEmailSender
    {
        private readonly MinecraftDbContext dbContext;
        private readonly IEmailSender emailSender;
        private readonly IWebHostEnvironment webHostEnvironment;

        public RecentActivityEmailSender(MinecraftDbContext dbContext, IEmailSender emailSender,
            IWebHostEnvironment webHostEnvironment)
        {
            this.dbContext = dbContext;
            this.emailSender = emailSender;
            this.webHostEnvironment = webHostEnvironment;
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
            var tableBody = new StringBuilder(392);

            for (int i = 0; i < logs.Count; i++)
            {
                tableBody.AppendLine(@"            <tr>");
                tableBody.AppendLine(@$"                <td>{logs[i].Date}</td>");
                tableBody.AppendLine(@$"                <td>{logs[i].Information}</td>");
                tableBody.AppendLine(@"            </tr>");
            }
           
            string htmlBody = GetEmailTemplate()
                .Replace("[TableBody]", tableBody.ToString());

            var emails = dbContext.Emails.Select(x => x.EmailAddress).ToList();

            for (int i = 0; i < emails.Count; i++)
            {
                await emailSender
                    .SendEmailAsync(emails[i], "Ostatnia aktywność na serwerze", htmlBody);
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

        private string GetEmailTemplate()
        {
            string emailTemplatePath = Path.Combine(webHostEnvironment.WebRootPath, @"emailTemplates\RecentActivity.html");
            return File.ReadAllText(emailTemplatePath);
        }
    }
}

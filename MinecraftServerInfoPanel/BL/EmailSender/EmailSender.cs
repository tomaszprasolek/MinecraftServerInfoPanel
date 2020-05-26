using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace MinecraftServerInfoPanel.BL.EmailSender
{
    public class EmailSender : IEmailSender
	{
        private readonly IConfiguration configuration;

        public EmailSender(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

		public async Task SendEmailAsync(string to, string subject, string body)
		{
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress("Minecraft Server Info Panel", configuration["EmailConfiguration:From"]));
			message.To.Add(new MailboxAddress(to));
			message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };

            message.Body = bodyBuilder.ToMessageBody();

			using (var client = new MailKit.Net.Smtp.SmtpClient())
			{
                try
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Connect(configuration["EmailConfiguration:SmtpServer"], Convert.ToInt32(configuration["EmailConfiguration:Port"]), false);
                    client.Authenticate(configuration["EmailConfiguration:Username"], configuration["EmailConfiguration:Password"]);

                    await client.SendAsync(message);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
			}
		}
	}
}

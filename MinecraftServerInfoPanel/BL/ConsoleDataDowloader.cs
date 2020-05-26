using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MinecraftServerInfoPanel.BL
{
    public class ConsoleDataDowloader : IConsoleDataDowloader
    {
        private readonly ILogger<ConsoleDataDowloader> logger;
        private readonly IConfiguration configuration;

        private HttpClient client = new HttpClient();

        public ConsoleDataDowloader(ILogger<ConsoleDataDowloader> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        public async Task<List<ConsoleLog>> Download()
        {
            var consoleData = await GetConsole(client);
            if (consoleData == null)
            {
                await Login(client);
                consoleData = await GetConsole(client);
            }

            return consoleData.Values.ToList();
        }

        private async Task<Dictionary<string, ConsoleLog>> GetConsole(HttpClient client)
        {
            HttpResponseMessage consoleResponse = await client.GetAsync(configuration["MinecraftServer:ConsoleUrl"]);

            var contentType = consoleResponse.Content.Headers.ContentType;

            if (contentType.MediaType == "text/html")
            {
                return null;
            }

            var content = await consoleResponse.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Dictionary<string, ConsoleLog>>(content);
        }

        private async Task<HttpStatusCode> Login(HttpClient client)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                 new KeyValuePair<string, string>("akcja", "logowanie"),
                 new KeyValuePair<string, string>("email", configuration["MinecraftServer:Email"]),
                 new KeyValuePair<string, string>("haslo", configuration["MinecraftServer:Password"]),
            });

            HttpResponseMessage responseMessage = await client.PostAsync(configuration["MinecraftServer:LoginUrl"], formContent);
            if (responseMessage.StatusCode == HttpStatusCode.OK)
                logger.LogInformation("Poprawnie zalogowano do serwera.");
            else
                logger.LogInformation("Wystąpił błąd podczas logowania do serwera.");
            return responseMessage.StatusCode;
        }
    }
}

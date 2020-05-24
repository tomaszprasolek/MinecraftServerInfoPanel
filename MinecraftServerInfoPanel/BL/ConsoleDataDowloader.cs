using System;
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
        HttpClient client = new HttpClient();

        public ConsoleDataDowloader()
        {
            client.BaseAddress = new Uri("");
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
            HttpResponseMessage consoleResponse = await client.GetAsync("");
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
                 new KeyValuePair<string, string>("email", ""),
                 new KeyValuePair<string, string>("haslo", ""),
            });

            HttpResponseMessage responseMessage = await client.PostAsync("account/signin/", formContent);
            if (responseMessage.StatusCode == HttpStatusCode.OK)
                Console.WriteLine("Poprawnie zalogowano do serwera.");
            else
                Console.WriteLine("Wystąpił błąd podczas logowania do serwera.");
            return responseMessage.StatusCode;
        }
    }
}

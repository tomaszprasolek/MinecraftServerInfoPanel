using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MinecraftServerInfoPanel.BL;
using MinecraftServerInfoPanel.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinecraftServerInfoPanel.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConsoleDataDowloader consoleDataDowloader;
        private readonly MinecraftDbContext dbContext;

        public List<DbConsoleLog> Logs { get; set; }

        [TempData]
        public string Message { get; set; }

        public IndexModel(ILogger<IndexModel> logger,
            IConsoleDataDowloader consoleDataDowloader,
            MinecraftDbContext dbContext)
        {
            _logger = logger;
            this.consoleDataDowloader = consoleDataDowloader;
            this.dbContext = dbContext;
        }

        public void OnGet()
        {
            Logs = dbContext.ConsoleLogs.OrderByDescending(x => x.Date).ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            List<ConsoleLog> result = await consoleDataDowloader.Download();

            var maxDateInDb = dbContext.ConsoleLogs.Max(x => x.Date);

            var dbEntities = result
                .Where(r => r.text.Contains("Running AutoCompaction...") == false)
                .Select(r => new DbConsoleLog
                {
                    Date = Convert.ToDateTime(r.text.Substring(1, 19)),
                    Information = r.text[26..].Trim(),
                    IsNeededToSendEmail = IsNeededToSendEmail(r.text)
                })
                .Where(r => r.Date > maxDateInDb)
                .ToList();

            if (dbEntities.Count > 0)
            {
                foreach (var entity in dbEntities)
                {
                    dbContext.Entry(entity).State = EntityState.Added;
                    dbContext.ConsoleLogs.Add(entity);
                }
                dbContext.SaveChanges();
                TempData["Message"] = "Pobrano nowe wpisy z serwera.";
            }
            else
                TempData["Message"] = "Nie pobrano żadnych danych z serwera.";

            return RedirectToPage("Index");
        }

        private bool IsNeededToSendEmail(string text) => text.Contains("connected");

    }
}

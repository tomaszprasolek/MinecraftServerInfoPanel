using System;
using System.ComponentModel.DataAnnotations;

namespace MinecraftServerInfoPanel.Domain.Entities
{
    public class DbConsoleLog
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public string Information { get; set; }

        /// <summary>
        /// Czy trzeba wysłać maila z informacją o tym zdarzeniu
        /// </summary>
        public bool IsNeededToSendEmail { get; set; }

        /// <summary>
        /// Czy wysłano emaila
        /// </summary>
        public bool SendEmail { get; set; }
    }
}

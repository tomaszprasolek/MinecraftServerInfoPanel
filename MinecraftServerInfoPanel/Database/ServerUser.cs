using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MinecraftServerInfoPanel.Database
{
    public class ServerUser
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string UserName { get; set; }

        [MaxLength(16)]
        public string Xuid { get; set; }
    }
}

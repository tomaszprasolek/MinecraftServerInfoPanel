using System.ComponentModel.DataAnnotations;

namespace MinecraftServerInfoPanel.Domain.Entities
{
    public class ServerUser
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string UserName { get; set; }

        [MaxLength(16)]
        public string Xuid { get; set; }

        public string Description { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace MinecraftServerInfoPanel.Domain.Entities
{
    public class Email
    {
        [Key]
        public int Id { get; set; }
        public string EmailAddress { get; set; }
    }
}

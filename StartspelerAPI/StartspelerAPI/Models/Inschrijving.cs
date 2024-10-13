using System.ComponentModel.DataAnnotations;

namespace StartspelerAPI.Models
{
    public class Inschrijving
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int EventId { get; set; }
        public Event? Event { get; set; }
        [Required]
        public string GebruikerId { get; set; }
        public Gebruiker? Gebruiker { get; set; }
        [Required]
        public DateTime TimestampInschrijving { get; set; }
    }
}

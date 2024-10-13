using System.ComponentModel.DataAnnotations;

namespace StartspelerAPI.Models
{
    public class Community
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Maximaal 50 karakters opgeven mesieur.")]
        public string Naam { get; set; }
        public List<Event> Events { get; set; }
    }
}

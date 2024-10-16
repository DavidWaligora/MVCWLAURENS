using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace StartspelerAPI.Models
{
    public class Event
    {

        [Required]
        public int Id { get; set; }
        [Required]
        [Length(5, 50, ErrorMessage = "Moet tussen 5 en 50 pik")]
        public string Naam { get; set; }

        [MaxLength(200, ErrorMessage = "niet te lang 200 max karakters")]
        public string? Beschrijving { get; set; }
        [Required]
<<<<<<< HEAD
        public DateTime Startmoment { get; set; } = DateTime.Now;
=======

        public DateTime StartMoment { get; set; } = DateTime.Now;
>>>>>>> 4b01c60c9093216d3c9e1c4ddd42fffb084d5578
        public double? Prijs { get; set; }
        [Range(4, 32, ErrorMessage = "4-32 doeme e")]
        public int? MaxDeelnemers { get; set; }

        public int? CommunityId { get; set; }
        public Community? Community { get; set; }
        public List<Inschrijving>? Inschrijvingen { get; set; }

        

    }
}
